namespace OneCore.Hardware.Lifx.Messages
{
    public class LifxVersion
    {
        private const byte LifxBulbVendor = 1;
        private const byte LifxBulbProduct = 1;
        private const byte LifxBulbVersion = 1;
        private const byte LifxFirmwareVersionMajor = 1;
        private const byte LifxFirmwareVersionMinor = 5;

        // timestamp data comes from observed packet from a LIFX v1.5 bulb

        private static readonly byte[] wifiFirmware =
        {
            0x00, 0xc8, 0x5e, 0x31, 0x99, 0x51, 0x86, 0x13, //build timestamp
            0xc0, 0x0c, 0x07, 0x00, 0x48, 0x46, 0xd9, 0x43, //install timestamp
            BitHelper.LowByte(LifxFirmwareVersionMinor),
            BitHelper.HightByte(LifxFirmwareVersionMinor),
            BitHelper.LowByte(LifxFirmwareVersionMajor),
            BitHelper.HightByte(LifxFirmwareVersionMajor)
        };

        private static readonly byte[] meshFirmware =
        {
            0x00, 0x2e, 0xc3, 0x8b, 0xef, 0x30, 0x86, 0x13, //build timestamp
            0xe0, 0x25, 0x76, 0x45, 0x69, 0x81, 0x8b, 0x13, //install timestamp
            BitHelper.LowByte(LifxFirmwareVersionMinor),
            BitHelper.HightByte(LifxFirmwareVersionMinor),
            BitHelper.LowByte(LifxFirmwareVersionMajor),
            BitHelper.HightByte(LifxFirmwareVersionMajor)
        };

        private static readonly byte[] version =
        {
            BitHelper.LowByte(LifxBulbVendor),
            BitHelper.HightByte(LifxBulbVendor),
            0x00,
            0x00,
            BitHelper.LowByte(LifxBulbProduct),
            BitHelper.HightByte(LifxBulbProduct),
            0x00,
            0x00,
            BitHelper.LowByte(LifxBulbVersion),
            BitHelper.HightByte(LifxBulbVersion),
            0x00,
            0x00
        };

        public static byte[] Version
        {
            get { return version; }
        }

        public static byte[] WifiFirmware
        {
            get { return wifiFirmware; }
        }

        public static byte[] MeshFirmware
        {
            get { return meshFirmware; }
        }
    }
}
