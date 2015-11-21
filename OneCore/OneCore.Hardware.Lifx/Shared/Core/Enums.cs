namespace OneCore.Hardware.Lifx.Core
{
    public enum LifxModes
    {
        Fix,
        RandomHue,
        RainbowHue,
        Red,
        Blue,
        Green,
        Fire
    };

    public enum LifxPowerState
    {
        On = 1,
        Off = 0,
        Unknown = -1
    }
    public enum LifxProtocol
    {
        AllBulbsResponse = 0x5400,
        AllBulbsRequest = 0x3400,
        BulbCommand = 0x1400
    }

    public enum LifxService : byte
    {
        Udp = 0x01,
        Tcp = 0x02
    }

    public enum LifxPacketType
    {
        GetStateService = 0x02,
        StateService = 0x03,

        GetWiFiFirmwareState = 0x12,
        WiFiFirmwareState = 0x13,

        GetPowerState = 0x14,
        SetPowerState = 0x15,
        PowerState = 0x16,

        GetBulbLabel = 0x17,
        SetBulbLabel = 0x18,
        BulbLabel = 0x19,

        GetLocation = 48,
        StateLocation = 50,

        GetGroup = 51,
        StateGroup = 53,

        GetVersion = 0x20,
        Version = 0x21,

        GetBulbTags = 0x1a,
        SetBulbTags = 0x1b,
        BulbTags = 0x1c,

        GetBulbTagLabels = 0x1d,
        SetBulbTagLabels = 0x1e,
        BulbTagLabels = 0x1f,

        GetLightState = 0x65,
        SetLightState = 0x66,
        LightState = 0x6b,

        GetMeshFirmwareState = 0x0e,
        MeshFirmwareState = 0x0f
    }

    public static class LifxConst
    {
        public const uint PacketSize = 36;
        public const uint Port = 56700;  // local port to listen on
        public const uint BulbLabelLength = 32;
        public const uint BulbTagsLength = 8;
        public const uint BulbTagLabelsLength = 32;
    }
}
