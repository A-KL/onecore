using OneCore.Hardware.Lifx.Colors;

namespace OneCore.Hardware.Lifx.Messages
{
    using System;

    using OneCore.Hardware.Lifx.Core;

    public class LifxServiceStateRequest : LifxRequest
    {
        private const uint Port = 56700;

        public LifxServiceStateRequest(LifxPacket packet)
            : base(packet)
        {
        }

        public override LifxResponse GetResponse()
        {
            var response = new LifxServiceStateResponse()
            {
                Port = Port,
                Service = LifxService.Udp
            };

            return response;
        }
    }

    public class LifxWiFiFirmwareRequest : LifxRequest
    {
        public LifxWiFiFirmwareRequest(LifxPacket packet)
            : base(packet)
        { }

        public override LifxResponse GetResponse()
        {
            return new LifxWiFiFirmwareResponse();
        }
    }

    public class LifxMeshFirmwareRequest : LifxRequest
    {
        public LifxMeshFirmwareRequest(LifxPacket packet)
            : base(packet)
        { }

        public override LifxResponse GetResponse()
        {
            return new LifxMeshFirmwareResponse();
        }
    }

    public class LifxVersionRequest : LifxRequest
    {
        public LifxVersionRequest(LifxPacket packet)
            : base(packet)
        { }

        public override LifxResponse GetResponse()
        {
            return new LifxVersionResponse();
        }
    }

    public class LifxLightStateRequest : LifxRequest
    {
        private readonly LifxHsbkColor color;

        public LifxLightStateRequest(LifxPacket packet)
            : base(packet)
        {
            this.color = LifxHsbkColor.FromBytes(this.Packet.Payload, 1);
            this.Duration = BitHelper.ReadUInt16(this.Packet.Payload, 1 + LifxHsbkColor.Size);
        }

        public LifxHsbkColor Color
        {
            get { return this.color; }
        }

        public Int32 Duration
        {
            get; private set;
        }
    }

    public class LifxPowerStateRequest : LifxRequest
    {
        private readonly ushort power;

        public LifxPowerStateRequest(LifxPacket packet)
            : base(packet)
        {
            this.power = BitHelper.ReadUInt16(this.Packet.Payload, 0);
        }

        public ushort Power
        {
            get { return this.power; }
        }
    }
}