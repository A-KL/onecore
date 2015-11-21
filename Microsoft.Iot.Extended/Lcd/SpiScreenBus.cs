namespace Microsoft.Iot.Extended.Lcd
{
    using System;
    using System.Threading.Tasks;

    using Windows.Devices.Enumeration;
    using Windows.Devices.Gpio;
    using Windows.Devices.Spi;

    using Microsoft.Iot.Extended.Graphics;

    public static class SpiModule
    {
        public const string Spi0 = "SPI0";
    }

    public class SpiScreenBus : IScreenBus
    {
        private readonly int line;
        private SpiDevice display;
        private readonly string module;        
        private readonly GpioController io;
        private readonly GpioPin dataCommandPin;
        private readonly GpioPin resetPin;

        public SpiScreenBus(string module, int chipSelectLine, int dataCommandPin, int resetPin)
        {
            this.line = chipSelectLine;
            this.module = module;

            this.io = GpioController.GetDefault(); /* Get the default GPIO controller on the system */

            if (this.io == null)
            {
                throw new Exception("GPIO does not exist on the current system.");
            }

            /* Initialize a pin as output for the Data/Command line on the display  */
            this.dataCommandPin = this.io.OpenPin(dataCommandPin);
            this.dataCommandPin.Write(GpioPinValue.High);
            this.dataCommandPin.SetDriveMode(GpioPinDriveMode.Output);

            /* Initialize a pin as output for the hardware Reset line on the display */
            this.resetPin = this.io.OpenPin(resetPin);
            this.resetPin.Write(GpioPinValue.High);
            this.resetPin.SetDriveMode(GpioPinDriveMode.Output);
        }

        public async Task Initialize()
        {
            var settings = new SpiConnectionSettings(this.line)
            {
                ClockFrequency = 10000000,
                Mode = SpiMode.Mode3
            };

            var spiAqs = SpiDevice.GetDeviceSelector(this.module);
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);

            this.display = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);
        }

        public async Task Reset()
        {
            this.resetPin.Write(GpioPinValue.Low);
            await Task.Delay(1);
            this.resetPin.Write(GpioPinValue.High);
            await Task.Delay(100);
        }

        public void SendCommand(byte[] command)
        {
            this.dataCommandPin.Write(GpioPinValue.Low);
            this.display.Write(command);
        }

        public void SendCommands(byte[] commands)
        {
            this.dataCommandPin.Write(GpioPinValue.Low);
            var commandArray = new byte[1];
    
            foreach (var command in commands)
            {
                commandArray[0] = command;
                this.display.Write(commandArray);
            }
        }

        public void SendCommand(byte command)
        {
            this.dataCommandPin.Write(GpioPinValue.Low);
            this.display.Write(new[] { command });
        }

        public void SendData(byte[] data)
        {
            this.dataCommandPin.Write(GpioPinValue.High);
            this.display.Write(data);
        }

        public void SendData(byte data)
        {
            this.dataCommandPin.Write(GpioPinValue.High);
            this.display.Write(new [] { data });
        }

        public void Dispose()
        {
            this.display.Dispose();
            this.resetPin.Dispose();
            this.dataCommandPin.Dispose();
        }
    }
}
