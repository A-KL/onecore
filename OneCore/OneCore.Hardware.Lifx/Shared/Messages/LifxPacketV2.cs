using OneCore.Hardware.Lifx.Core;

namespace OneCore.Hardware.Lifx.Messages
{
    using System;

    using OneCore.Hardware.Lifx;

    public class LifxPacketV2
    {
        #region Public

        #region Static

        public static LifxPacketV2 Parse(byte[] data)
        {
            if (data == null || data.Length < LifxPacketSize)
            {
                throw new ArgumentException("Wrong size of data");
            }

            return new LifxPacketV2(data);
        }

        //public static LifxPacket ParseOld(byte[] data)
        //{
        //    if (data == null || data.Length < LifxPacketSize)
        //    {
        //        throw new ArgumentException("Wrong size of data");
        //    }

        //    var packet = new LifxPacket();
        //    var index = 0;

        //    packet.size = BitConverter.ToUInt16(data, index);
        //    index += 2;

        //    packet.protocol = BitConverter.ToUInt16(data, index);
        //    index += 2;

        //    packet.reserved1 = BitConverter.ToUInt32(data, index);
        //    index += 4;

        //    Array.Copy(data, index, packet.target_mac_address, 0, 6);
        //    index += 6;

        //    packet.reserved2 = BitConverter.ToUInt16(data, index);
        //    index += 2;

        //    Array.Copy(data, index, packet.site, 0, 6);
        //    index += 6;

        //    packet.reserved3 = BitConverter.ToUInt16(data, index);
        //    index += 2;

        //    packet.timestamp = BitConverter.ToUInt32(data, index);
        //    index += 4;

        //    packet.type = BitConverter.ToUInt16(data, index);
        //    index += 2;

        //    packet.reserved4 = BitConverter.ToUInt16(data, index);

        //    var payloadSize = packet.size - index;
        //    packet.payload = new byte[payloadSize];

        //    Array.Copy(data, index, packet.payload, 0, payloadSize);

        //    return packet;
        //}

        public static byte[] ToBytes(LifxPacketV2 packet)
        {
            var data = new byte[packet.payload.Length + LifxPacketSize];

            Array.Copy(packet.rawData, data, LifxPacketSize);
            Array.Copy(packet.payload, 0, data, LifxPacketSize, packet.payload.Length);

            return data;
        }

        #endregion

        #region Properties

        public int ActualSize
        {
            get
            {
                if (this.payload == null)
                {
                    return LifxPacketSize;
                }
                return this.payload.Length + LifxPacketSize;
            }
        }

        private UInt16 PacketSize
        {
            get
            {
                return BitHelper.ReadUInt16(this.rawData, 0);
            }
            set
            {
                BitHelper.Write(this.rawData, 0, value);
            }
        }

        public LifxProtocol Protocol
        {
            get
            {
                return (LifxProtocol)BitHelper.ReadUInt16(this.rawData, 2);
            }
            protected set
            {
                BitHelper.Write(this.rawData, 2, (UInt16)value);
            }
        }

        // reserved 1 - 4 bytes

        public UInt64 BulbAddress
        {
            get
            {
                return BitHelper.ReadUInt64(this.rawData, 8);
            }
            protected set
            {
                BitHelper.Write(this.rawData, 8, value);
            }
        }

        // reserved 2 - 2 bytes

        public UInt64 PanGatewayAddress
        {
            get
            {
                return BitHelper.ReadUInt64(this.rawData, 16);
            }
            set
            {
                BitHelper.Write(this.rawData, 16, value);
            }
        }

        // reserved 3

        // TimeStamp

        public LifxPacketType Type
        {
            get
            {
                return (LifxPacketType)BitHelper.ReadUInt16(this.rawData, 32);
            }
            protected set
            {
                BitHelper.Write(this.rawData, 32, (UInt16)value);
            }
        }

        // reserved 4

        #endregion

        public byte[] ToBytes()
        {
            return ToBytes(this);
        }

        #endregion

        #region Protected

        protected LifxPacketV2(int payloadSize)
        {
            this.payload = new byte[payloadSize];

            this.PacketSize = (UInt16)this.ActualSize;
        }

        protected byte[] Payload
        {
            get
            {
                return this.payload;
            }
        }

        #endregion

        #region Private

        #region Constructor

        private LifxPacketV2()
        {
            this.rawData = new byte[LifxPacketSize];
        }

        private LifxPacketV2(byte[] data)
            : this()
        {
            if (data == null || data.Length < LifxPacketSize)
            {
                throw new ArgumentException("Wrong size of data");
            }

            Array.Copy(data, this.rawData, LifxPacketSize);

            var payloadSize = data.Length - LifxPacketSize;

            if (payloadSize > 0)
            {
                this.payload = new byte[payloadSize];
                Array.Copy(data, LifxPacketSize, this.payload, 0, payloadSize);
            }
        }

        #endregion

        //private UInt16 protocol;
        //private UInt32 reserved1;
        //private byte[] target_mac_address = new byte[6];
        //private UInt16 reserved2; // Always 0x00
        //private byte[] site = new byte[6];  // MAC address of gateway PAN controller bulb
        //private UInt16 reserved3; // Always 0x00
        //private UInt64 timestamp;
        //private UInt16 type; // LE
        //private UInt16 reserved4; // Always 0x0000

        private const int LifxPacketSize = 36;

        // Header
        private readonly byte[] rawData = new byte[LifxPacketSize];

        // Payload
        private readonly byte[] payload;

        #endregion
    }
}
