namespace OneCore.Hardware.Lifx.Contracts
{
    public interface INetworkService
    {
        uint Port { get; }

        void Open();

        void AddClient(INetworkClient client);

        void RemoveClient(INetworkClient client);
    }
}
