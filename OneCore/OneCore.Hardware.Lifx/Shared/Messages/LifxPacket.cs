using OneCore.Hardware.Lifx.Core;

namespace OneCore.Hardware.Lifx.Messages
{
    using System;

    public class LifxPacketInfo
    {
        #region Members

        public const ushort Size = 36;

        #endregion

        #region Public

        public static LifxPacketInfo Parse(byte[] data, int start)
        {
            var info = new LifxPacketInfo();
            info.ParseToInstance(data, start);

            return info;
        }

        public LifxProtocol Protocol { get; set; }

        public uint Reserved1 { get; set; }

        public ulong BulbAddress { get; set; }

        public ushort Reserved2 { get; set; }

        public byte[] Site { get; set; }

        public ushort Reserved3 { get; set; }

        public ulong Timestamp { get; set; }

        public LifxPacketType PacketType { get; set; }

        public ushort Reserved4 { get; set; }

        #endregion

        #region Protected

        public virtual int ToByteArray(byte[] mac, byte[] result, int start)
        {
            var index = start;

            // Protocol
            result[index++] = BitHelper.LowByte((ushort)this.Protocol);
            result[index++] = BitHelper.HightByte((ushort)this.Protocol);

            // Reserved 1
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;

            //var address = BitHelper.

            // BulbAddress mac address
            Array.Copy(BitConverter.GetBytes(this.BulbAddress),0, result, index, 6);
            index += 6;

            // Reserved 2
            result[index++] = 0;
            result[index++] = 0;

            // Site mac address
            for (var i = 0; i < mac.Length; i++)
            {
                result[index++] = mac[i];
            }

            // Reserved 3
            result[index++] = 0;
            result[index++] = 0;

            // Timestamp
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;
            result[index++] = 0;

            // Packet type
            result[index++] = BitHelper.LowByte((ushort)this.PacketType);
            result[index++] = BitHelper.HightByte((ushort)this.PacketType);

            // Reserved 4
            result[index++] = 0;
            result[index] = 0;

            return index;
        }

        private void ParseToInstance(byte[] data, int start)
        {
            if (data == null || (data.Length) < Size)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            var index = start;

            this.Protocol = (LifxProtocol)(data[index++] + (data[index++] << 8));

            this.Reserved1 = (ushort)(data[index++] + data[index++] + data[index++] + data[index++]);

            this.BulbAddress = BitConverter.ToUInt64(data, index);

            index += 6;

            this.Reserved2 = (ushort)(data[index++] + data[index++]);

            this.Site = new[] { data[index++], data[index++], data[index++], data[index++], data[index++], data[index++] };

            this.Reserved3 = (ushort)(data[index++] + data[index++]);

            this.Timestamp = 0;
            for (var i = 0; i < 8; i++)
            {
                this.Timestamp += data[index++];
            }

            this.PacketType = (LifxPacketType)(data[index++] + (data[index++] << 8));

            this.Reserved4 = (ushort)(data[index++] + data[index]);
        }

        #endregion        
    }

    public class LifxPacket
    {
        #region Public

        public LifxPacket()
        {
            this.Info = new LifxPacketInfo();
        }

        public ushort Size { get; set; }

        public LifxPacketInfo Info { get; private set; }

        public byte[] Payload { get; set; }

        public ushort PayloadSize
        {
            get { return (ushort)(this.Payload == null ? 0 : this.Payload.Length); }
            set
            {
                if (value <= 0)
                {
                    this.Payload = null;
                    return;
                }

                if (this.Payload != null && Payload.Length == value)
                {
                    return;
                }

                this.Payload = new byte[value];
            }
        }

        public static LifxPacket Parse(byte[] data)
        {
            var result = new LifxPacket();

            if (result.TryParse(data))
            {
                return result;
            }

            return null;
        }

        public ushort ToByteArray(byte[] mac, byte[] result)
        {
            var payloadSize = this.PayloadSize;
            var index = 0;

            // Size
            result[index++] = BitHelper.LowByte(this.Size);
            result[index++] = BitHelper.HightByte(this.Size);

            var packetSize = this.Info.ToByteArray(mac, result, index);

            if (payloadSize > 0)
            {
                Array.Copy(this.Payload, 0, result, packetSize + 1, payloadSize);
            }

            return (ushort)(packetSize + payloadSize);
        }

        #endregion

        private bool TryParse(byte[] data)
        {
            var index = 0;

            this.Size = (ushort)(data[index++] + (data[index++] << 8));

            this.Info = LifxPacketInfo.Parse(data, index);

            var payloadSize = this.Size - LifxPacketInfo.Size;

            this.PayloadSize = (ushort)payloadSize;

            //if (payloadSize <= 0)
            //{
            //    return true;
            //}

            //Array.Copy(data, 0, this.Payload, LifxPacketInfo.Size, payloadSize);

            return true;
        }
    }
}
