#region Licence

// Copyright (C) 2012 by Jakub Bartkowiak (Gralin)
// 
// MIT Licence
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

namespace OneCore.Hardware.Radio.nRF24LPlus
{
	using System;
	using System.Threading;

	/// <summary>
	///   Driver class for Nordic nRF24L01+ tranceiver
	/// </summary>
	public class NRF24L01Plus
	{
		#region Delegates

		public delegate void EventHandler();

		public delegate void OnDataRecievedHandler(byte[] data);

		public delegate void OnInterruptHandler(Status status);

		#endregion

		private byte[] _slot0Address;
		private bool _initialized;

		private IOneInterruptPort _irqPin;
		private IOneSpiInterface _spiPort;
		private IOneOutputPort _cePin;

		private bool _enabled;
		private readonly ManualResetEvent _transmitSuccessFlag;
		private readonly ManualResetEvent _transmitFailedFlag;

		/// <summary>
		///   Gets a value indicating whether module is enabled (RX or TX mode).
		/// </summary>
		public bool IsEnabled
		{
			get { return this._cePin.Read(); }
		}

		public NRF24L01Plus()
		{
			this._transmitSuccessFlag = new ManualResetEvent(false);
			this._transmitFailedFlag = new ManualResetEvent(false);
		}

		/// <summary>
		///   Enables the module
		/// </summary>
		public void Enable()
		{
			this._enabled = true;
			this.SetEnabled();
		}

		/// <summary>
		///   Disables the module
		/// </summary>
		public void Disable()
		{
			this._enabled = false;
			this.SetDisabled();
		}

		/// <summary>
		///   Initializes SPI connection and control pins
		/// </summary>
		public void Initialize(IOneSpiInterface spi, IOneOutputPort chipEnable, IOneInterruptPort interrupt)
		{
			// Chip Select : Active Low
			// Clock : Active High, Data clocked in on rising edge
			//this._config = new SPI.Configuration(chipSelectPin, false, 0, 0, false, true, 2000, spi);
			this._spiPort = spi;

			// Initialize IRQ Port
			//this._irqPin = new InterruptPort(interruptPin, false, Port.ResistorMode.PullUp,
			//							Port.InterruptMode.InterruptEdgeLow);

			this._irqPin = interrupt;
			this._irqPin.OnInterrupt += this.HandleInterrupt;

			// Initialize Chip Enable Port
			//this._cePin = new OutputPort(chipEnablePin, false);
			this._cePin = chipEnable;

#if MF_FRAMEWORK_VERSION_V4_3

			// Module reset time
			Thread.Sleep(100);
#endif
			this._initialized = true;
		}

		/// <summary>
		/// Configure the module basic settings. Module needs to be initiaized.
		/// </summary>
		/// <param name="address">RF address (3-5 bytes). The width of this address determins the width of all addresses used for sending/receiving.</param>
		/// <param name="channel">RF channel (0-127)</param>
		public void Configure(byte[] address, byte channel)
		{
			this.Configure(address, channel, NRFDataRate.DR2Mbps);
		}

		/// <summary>
		/// Configure the module basic settings. Module needs to be initiaized.
		/// </summary>
		/// <param name="address">RF address (3-5 bytes). The width of this address determins the width of all addresses used for sending/receiving.</param>
		/// <param name="channel">RF channel (0-127)</param>
		/// <param name="dataRate">Data Rate to use</param>
		public void Configure(byte[] address, byte channel, NRFDataRate dataRate)
		{
			this.CheckIsInitialized();
			AddressWidth.Check(address);

			// Set radio channel
			this.Execute(Commands.W_REGISTER, Registers.RF_CH,
					new[]
						{
							(byte) (channel & 0x7F) // channel is 7 bits
						});

			// Set Data rate
			var regValue = this.Execute(Commands.R_REGISTER, Registers.RF_SETUP, new byte[1])[1];

			switch (dataRate)
			{
				case NRFDataRate.DR1Mbps:
					regValue &= (byte)~(1 << Bits.RF_DR_LOW);  // 0
					regValue &= (byte)~(1 << Bits.RF_DR_HIGH); // 0
					break;

				case NRFDataRate.DR2Mbps:
					regValue &= (byte)~(1 << Bits.RF_DR_LOW);  // 0
					regValue |= (byte)(1 << Bits.RF_DR_HIGH);  // 1
					break;

				case NRFDataRate.DR250kbps:
					regValue |= (byte)(1 << Bits.RF_DR_LOW);   // 1
					regValue &= (byte)~(1 << Bits.RF_DR_HIGH); // 0
					break;

				default:
					throw new ArgumentOutOfRangeException("dataRate");
			}

			this.Execute(Commands.W_REGISTER, Registers.RF_SETUP, new[] { regValue });

			// Enable dynamic payload length
			this.Execute(Commands.W_REGISTER, Registers.FEATURE,
					new[]
						{
							(byte) (1 << Bits.EN_DPL)
						});

			// Set auto-ack
			this.Execute(Commands.W_REGISTER, Registers.EN_AA,
					new[]
						{
							(byte) (1 << Bits.ENAA_P0 |
									1 << Bits.ENAA_P1)
						});

			// Set dynamic payload length for pipes
			this.Execute(Commands.W_REGISTER, Registers.DYNPD,
					new[]
						{
							(byte) (1 << Bits.DPL_P0 |
									1 << Bits.DPL_P1)
						});

			// Flush RX FIFO
			this.Execute(Commands.FLUSH_RX, 0x00, new byte[0]);

			// Flush TX FIFO
			this.Execute(Commands.FLUSH_TX, 0x00, new byte[0]);

			// Clear IRQ Masks
			this.Execute(Commands.W_REGISTER, Registers.STATUS,
					new[]
						{
							(byte) (1 << Bits.MASK_RX_DR |
									1 << Bits.MASK_TX_DS |
									1 << Bits.MAX_RT)
						});

			// Set default address
			this.Execute(Commands.W_REGISTER, Registers.SETUP_AW,
					new[]
						{
							AddressWidth.Get(address)
						});

			// Set module address
			this._slot0Address = address;
			this.Execute(Commands.W_REGISTER, (byte)AddressSlot.Zero, address);

			// Set retransmission values
			this.Execute(Commands.W_REGISTER, Registers.SETUP_RETR,
					new[]
						{
							(byte) (0x0F << Bits.ARD |
									0x0F << Bits.ARC)
						});

			// Setup, CRC enabled, Power Up, PRX
			this.SetReceiveMode();
		}

		/// <summary>
		/// Set one of 6 available module addresses
		/// </summary>
		public void SetAddress(AddressSlot slot, byte[] address)
		{
			this.CheckIsInitialized();
			AddressWidth.Check(address);
			this.Execute(Commands.W_REGISTER, (byte)slot, address);

			if (slot == AddressSlot.Zero)
			{
				this._slot0Address = address;
			}
		}

		/// <summary>
		/// Read 1 of 6 available module addresses
		/// </summary>
		public byte[] GetAddress(AddressSlot slot, int width)
		{
			this.CheckIsInitialized();
			AddressWidth.Check(width);
			var read = this.Execute(Commands.R_REGISTER, (byte)slot, new byte[width]);
			var result = new byte[read.Length - 1];
			Array.Copy(read, 1, result, 0, result.Length);
			return result;
		}

		/// <summary>
		///   Executes a command in NRF24L01+ (for details see module datasheet)
		/// </summary>
		/// <param name = "command">Command</param>
		/// <param name = "addres">Register to write to</param>
		/// <param name = "data">Data to write</param>
		/// <returns>Response byte array. First byte is the status register</returns>
		public byte[] Execute(byte command, byte addres, byte[] data)
		{
			this.CheckIsInitialized();

			// This command requires module to be in power down or standby mode
			if (command == Commands.W_REGISTER)
				this.SetDisabled();

			// Create SPI Buffers with Size of Data + 1 (For Command)
			var writeBuffer = new byte[data.Length + 1];
			var readBuffer = new byte[data.Length + 1];

			// Add command and adres to SPI buffer
			writeBuffer[0] = (byte)(command | addres);

			// Add data to SPI buffer
			Array.Copy(data, 0, writeBuffer, 1, data.Length);

			// Do SPI Read/Write            
			//_spiPort.WriteRead(writeBuffer, readBuffer);
			//SpiManager.LockWriteRead(this._config, writeBuffer, readBuffer);
			this._spiPort.WriteRead(writeBuffer, readBuffer);

			// Enable module back if it was disabled
			if (command == Commands.W_REGISTER && this._enabled)
				this.SetEnabled();

			// Return ReadBuffer
			return readBuffer;
		}

		/// <summary>
		///   Gets module basic status information
		/// </summary>
		public Status GetStatus()
		{
			this.CheckIsInitialized();

			var readBuffer = new byte[1];

			//_spiPort.WriteRead(new[] {Commands.NOP}, readBuffer);
			//SpiManager.LockWriteRead(this._config, new[] { Commands.NOP }, readBuffer);

			this._spiPort.WriteRead(new[] { Commands.NOP }, readBuffer);

			return new Status(readBuffer[0]);
		}

		/// <summary>
		///   Reads the current rf channel value set in module
		/// </summary>
		/// <returns></returns>
		public byte GetChannel()
		{
			this.CheckIsInitialized();

			var result = this.Execute(Commands.R_REGISTER, Registers.RF_CH, new byte[1]);
			return (byte)(result[1] & 0x7F);
		}

		/// <summary>
		///   Gets the module radio frequency [MHz]
		/// </summary>
		/// <returns>Frequency in MHz</returns>
		public int GetFrequency()
		{
			return 2400 + this.GetChannel();
		}

		/// <summary>
		///   Sets the rf channel value used by all data pipes
		/// </summary>
		/// <param name="channel">7 bit channel value</param>
		public void SetChannel(byte channel)
		{
			this.CheckIsInitialized();

			var writeBuffer = new[] { (byte)(channel & 0x7F) };
			this.Execute(Commands.W_REGISTER, Registers.RF_CH, writeBuffer);
		}

		/// <summary>
		///   Send <param name = "bytes">bytes</param> to given <param name = "address">address</param>
		///   This is a non blocking method.
		/// </summary>
		public void SendTo(byte[] address, byte[] bytes, Acknowledge acknowledge = Acknowledge.Yes)
		{
			// Chip enable low
			this.SetDisabled();

			// Setup PTX (Primary TX)
			this.SetTransmitMode();

			// Write transmit adres to TX_ADDR register. 
			this.Execute(Commands.W_REGISTER, Registers.TX_ADDR, address);

			// Write transmit adres to RX_ADDRESS_P0 (Pipe0) (For Auto ACK)
			this.Execute(Commands.W_REGISTER, Registers.RX_ADDR_P0, address);

			// Send payload
			this.Execute(acknowledge == Acknowledge.Yes ? Commands.W_TX_PAYLOAD : Commands.W_TX_PAYLOAD_NO_ACK, 0x00, bytes);

			// Pulse for CE -> starts the transmission.
			this.SetEnabled();
		}

		/// <summary>
		///   Sends <param name = "bytes">bytes</param> to given <param name = "address">address</param>
		///   This is a blocking method that returns true if data was received by the recipient or false if timeout occured.
		/// </summary>
		public bool SendTo(byte[] address, byte[] bytes, int timeout)
		{
			var startTime = DateTime.Now;

			while (true)
			{
				this._transmitSuccessFlag.Reset();
				this._transmitFailedFlag.Reset();

				this.SendTo(address, bytes);
#if MF_FRAMEWORK_VERSION_V4_3
				if (WaitHandle.WaitAny(new[] { this._transmitSuccessFlag, this._transmitFailedFlag }, 200, true) == 0)
					return true;
#else
                if (WaitHandle.WaitAny(new[] { this._transmitSuccessFlag, this._transmitFailedFlag }, 200) == 0)
                    return true;
#endif
                if (DateTime.Now.CompareTo(startTime.AddMilliseconds(timeout)) > 0)
					return false;

				//Debug.Print("Retransmitting packet...");
			}
		}

		private void HandleInterrupt(uint data1, uint data2, DateTime dateTime)
		{
			if (!this._initialized)
				return;

			if (!this._enabled)
			{
				// Flush RX FIFO
				this.Execute(Commands.FLUSH_RX, 0x00, new byte[0]);
				// Flush TX FIFO 
				this.Execute(Commands.FLUSH_TX, 0x00, new byte[0]);
				return;
			}

			// Disable RX/TX
			this.SetDisabled();

			// Set PRX
			this.SetReceiveMode();

			// there are 3 rx pipes in rf module so 3 arrays should be enough to store incoming data
			// sometimes though more than 3 data packets are received somehow
			var payloads = new byte[6][];

			var status = this.GetStatus();
			byte payloadCount = 0;
			var payloadCorrupted = false;

			this.OnInterrupt(status);

			if (status.DataReady)
			{
				while (!status.RxEmpty)
				{
					// Read payload size
					var payloadLength = this.Execute(Commands.R_RX_PL_WID, 0x00, new byte[1]);

					// this indicates corrupted data
					if (payloadLength[1] > 32)
					{
						payloadCorrupted = true;

						// Flush anything that remains in buffer
						this.Execute(Commands.FLUSH_RX, 0x00, new byte[0]);
					}
					else
					{
						if (payloadCount >= payloads.Length)
						{
							//Debug.Print("Unexpected payloadCount value = " + payloadCount);
							this.Execute(Commands.FLUSH_RX, 0x00, new byte[0]);
						}
						else
						{
							// Read payload data
							payloads[payloadCount] = this.Execute(Commands.R_RX_PAYLOAD, 0x00, new byte[payloadLength[1]]);
							payloadCount++;
						}
					}

					// Clear RX_DR bit 
					var result = this.Execute(Commands.W_REGISTER, Registers.STATUS, new[] { (byte)(1 << Bits.RX_DR) });
					status.Update(result[0]);
				}
			}

			if (status.ResendLimitReached)
			{
				// Flush TX FIFO 
				this.Execute(Commands.FLUSH_TX, 0x00, new byte[0]);

				// Clear MAX_RT bit in status register
				this.Execute(Commands.W_REGISTER, Registers.STATUS, new[] { (byte)(1 << Bits.MAX_RT) });
			}

			if (status.TxFull)
			{
				// Flush TX FIFO 
				this.Execute(Commands.FLUSH_TX, 0x00, new byte[0]);
			}

			if (status.DataSent)
			{
				// Clear TX_DS bit in status register
				this.Execute(Commands.W_REGISTER, Registers.STATUS, new[] { (byte)(1 << Bits.TX_DS) });
			}

			// Enable RX
			this.SetEnabled();

			if (payloadCorrupted)
			{
				//Debug.Print("Corrupted data received");
			}
			else if (payloadCount > 0)
			{
				if (payloadCount > payloads.Length)
					//Debug.Print("Unexpected payloadCount value = " + payloadCount);

					for (var i = 0; i < System.Math.Min(payloadCount, payloads.Length); i++)
					{
						var payload = payloads[i];
						var payloadWithoutCommand = new byte[payload.Length - 1];
						Array.Copy(payload, 1, payloadWithoutCommand, 0, payload.Length - 1);
						this.OnDataReceived(payloadWithoutCommand);
					}
			}
			else if (status.DataSent)
			{
				this._transmitSuccessFlag.Set();
				this.OnTransmitSuccess();
			}
			else
			{
				this._transmitFailedFlag.Set();
				this.OnTransmitFailed();
			}
		}

		private void SetEnabled()
		{
			this._irqPin.EnableInterrupt();
			this._cePin.Write(true);
		}

		private void SetDisabled()
		{
			this._cePin.Write(false);
			this._irqPin.DisableInterrupt();
		}

		private void SetTransmitMode()
		{
			this.Execute(Commands.W_REGISTER, Registers.CONFIG,
					new[]
						{
							(byte) (1 << Bits.PWR_UP |
									1 << Bits.CRCO)
						});
		}

		private void SetReceiveMode()
		{
			this.Execute(Commands.W_REGISTER, Registers.RX_ADDR_P0, this._slot0Address);

			this.Execute(Commands.W_REGISTER, Registers.CONFIG,
					new[]
						{
							(byte) (1 << Bits.PWR_UP |
									1 << Bits.CRCO |
									1 << Bits.PRIM_RX)
						});
		}

		private void CheckIsInitialized()
		{
			if (!this._initialized)
			{
				throw new InvalidOperationException("Initialize method needs to be called before this call");
			}
		}

		/// <summary>
		///   Called on every IRQ interrupt
		/// </summary>
		public event OnInterruptHandler OnInterrupt = delegate { };

		/// <summary>
		///   Occurs when data packet has been received
		/// </summary>
		public event OnDataRecievedHandler OnDataReceived = delegate { };

		/// <summary>
		///   Occurs when ack has been received for send packet
		/// </summary>
		public event EventHandler OnTransmitSuccess = delegate { };

		/// <summary>
		///   Occurs when no ack has been received for send packet
		/// </summary>
		public event EventHandler OnTransmitFailed = delegate { };
	}
}