namespace OneCore.Hardware.Lifx.Messages
{
    using System;
    using System.Text;

    using OneCore.Hardware.Lifx.Core;

    public class LifxServiceStateResponse : LifxResponse
    {
        public LifxServiceStateResponse()
            : base(ServiceStatePayloadSize, LifxPacketType.StateService, LifxProtocol.AllBulbsResponse)
        { }

        public LifxService Service
        {
            get
            {
                return (LifxService)BitHelper.ReadByte(this.Payload, 0);
            }
            set
            {
                BitHelper.Write(this.Payload, 0, (byte)value);
            }
        }

        public uint Port
        {
            get
            {
                return BitHelper.ReadUInt32(this.Payload, 1);
            }
            set
            {
                BitHelper.Write(this.Payload, 1, value);
            }
        }

        private const int ServiceStatePayloadSize = 5;
    }

    public class LifxWiFiFirmwareResponse : LifxResponse
    {
        public LifxWiFiFirmwareResponse()
            : base(LifxVersion.WifiFirmware.Length, LifxPacketType.WiFiFirmwareState, LifxProtocol.AllBulbsResponse)
        {
            this.Payload = LifxVersion.WifiFirmware;
        }
    }

    public class LifxMeshFirmwareResponse : LifxResponse
    {
        public LifxMeshFirmwareResponse()
            : base(LifxVersion.MeshFirmware.Length, LifxPacketType.MeshFirmwareState, LifxProtocol.AllBulbsResponse)
        {
            Payload = LifxVersion.MeshFirmware;
        }
    }

    public class LifxVersionResponse : LifxResponse
    {
        public LifxVersionResponse()
            : base(LifxVersion.Version.Length, LifxPacketType.Version, LifxProtocol.AllBulbsResponse)
        {
            Payload = LifxVersion.Version;
        }
    }

    public class LifxLightStateResponse : LifxResponse
    {
        public LifxLightStateResponse(ILifxBulb bulb) 
            : this()
        {            
             bulb.ToByteArray(Payload, 0);
        }

        public LifxLightStateResponse()
            : base(LightStatePayloadSize, LifxPacketType.LightState, LifxProtocol.AllBulbsResponse)
        {
        }

        private const int LightStatePayloadSize = 12 + 32 + 8;
    }

    public class LifxPowerStateResponse : LifxResponse
    {
        public LifxPowerStateResponse(ILifxBulb bulb) 
            : this()
        {
            this.Payload[0] = BitHelper.LowByte(bulb.Power);
            this.Payload[1] = BitHelper.HightByte(bulb.Power);
        }

        public LifxPowerStateResponse()
            : base(2, LifxPacketType.PowerState, LifxProtocol.AllBulbsResponse)
        {
        }
    }

    public class LifxLocationResponse : LifxResponse
    {
        public LifxLocationResponse(ILifxBulb bulb)
            : this()
        {
            var location = Encoding.UTF8.GetBytes(bulb.Location);
            var label = Encoding.UTF8.GetBytes(bulb.Label);

            Array.Copy(location, 0, this.Payload, 0, location.Length);

            Array.Copy(label, 0, this.Payload, LocationDataSize, label.Length);            
        }

        public LifxLocationResponse()
            : base(2, LifxPacketType.StateLocation, LifxProtocol.AllBulbsResponse)
        {
        }

        private const int LocationDataSize = 16;
        private const int LabelDataSize = 32;

        private const int LightStatePayloadSize = LocationDataSize + LabelDataSize + 8;
    }
}
