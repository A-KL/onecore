namespace OneCore.Hardware.Lifx.Messages
{
    using System;
    using OneCore.Hardware.Lifx.Core;

    public abstract class LifxRequest
    {
        #region Public

        public static LifxRequest Create(ILifxHub hub, byte[] data)
        {
            return Create(hub, LifxPacket.Parse(data));
        }

        public static LifxRequest Create(ILifxHub hub, LifxPacket packet)
        {
            switch (packet.Info.PacketType)
            {
                case LifxPacketType.GetStateService:
                    return new LifxServiceStateRequest(packet);

                case LifxPacketType.GetWiFiFirmwareState:
                    return new LifxWiFiFirmwareRequest(packet);

                case LifxPacketType.GetMeshFirmwareState:
                    return new LifxMeshFirmwareRequest(packet);

                case LifxPacketType.GetVersion:
                    return new LifxVersionRequest(packet);

                case LifxPacketType.GetLightState:
                    return new LifxLightStateRequest(packet);

                case LifxPacketType.SetLightState:


                    return new LifxLightStateRequest(packet);

                default:
                    return null;
                // throw new ArgumentOutOfRangeException("Protocol");
            }
        }

        public virtual LifxResponse GetResponse()
        {
            throw new Exception("No response");
        }

        #endregion

        #region Protected

        protected LifxRequest(LifxPacket packet)
        {
            this.Packet = packet;
        }

        protected LifxPacket Packet { get; private set; }

        #endregion
    }
}
