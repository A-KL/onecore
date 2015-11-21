namespace OneCore.Hardware.Lifx.Contracts
{
    public interface INetworkClient
    {
        void MessageReceived(INetworkService service, IStream stream);
    }
}
