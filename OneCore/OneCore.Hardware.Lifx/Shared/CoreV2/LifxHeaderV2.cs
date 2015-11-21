namespace OneCore.Hardware.Lifx.CoreV2
{
    using System;
    using OneCore.Hardware.Lifx.Core;

    public class LifxHeaderV2
    {
        public static LifxHeaderV2 Parse(byte[] data)
        {
            if (data == null || data.Length < 36)
            {
                throw new ArgumentException("Wrong size of data");
            }

            var header = new LifxHeaderV2();
            var index = 0;

            /* frame */

            header.Size = (ushort)(data[index++] + (data[index++] << 8));

            header.Protocol = (ushort)(data[index++] + (data[index++]));

            header.Addressable = data[index++];

            header.Tagged = data[index++];

            header.Origin = data[index++];

            header.Source =  data[index++] + data[index++] + data[index++] + data[index++];

            /* frame address */
            for (var i = 0; i < 8; i++)
                header.Target[i] = data[index++];

            for (var i = 0; i < 6; i++)
                header.Reserved1[i] = data[index++];

            header.ResRequired = data[index++];

            header.AckRequired = data[index++];

            header.Reserved2 = data[index++];

            header.Sequence = data[index++];

            /* protocol header */
            header.Reserved3 = 0;
            for (var i = 0; i < 8; i++)
            {
                header.Reserved3 += data[index++];
            }

            header.PacketType = (LifxPacketType)(data[index++] + (data[index++] << 8));

            header.Reserved4 = (ushort)(data[index++] + data[index]);

            return header;
        }

        public ushort Size { get; set; }

        public ushort Protocol { get; set; }

        public byte Addressable { get; set; }

        public byte Tagged { get; set; }

        public byte Origin { get; set; }

        public int Source { get; set; } // 4


        public byte[] Target { get; set; } // 8

        public byte[] Reserved1 { get; set; } // 6

        public byte ResRequired { get; set; }

        public byte AckRequired { get; set; }

        public byte Reserved2 { get; set; }

        public byte Sequence { get; set; }


        public long Reserved3 { get; set; }

        public LifxPacketType PacketType { get; set; }

        public ushort Reserved4 { get; set; }
    }
}
