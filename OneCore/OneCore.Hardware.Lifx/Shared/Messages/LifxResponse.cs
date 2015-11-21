using OneCore.Hardware.Lifx.Core;

namespace OneCore.Hardware.Lifx.Messages
{
    public abstract class LifxResponse : LifxPacket
    {
        protected LifxResponse(int payloadSize, LifxPacketType type, LifxProtocol protocol = LifxProtocol.AllBulbsResponse)
        {
            this.Size = (ushort)(payloadSize + LifxPacketInfo.Size);
            this.PayloadSize = (ushort)payloadSize;
            this.Info.PacketType = type;
            this.Info.Protocol = protocol;
        }
    }
}
