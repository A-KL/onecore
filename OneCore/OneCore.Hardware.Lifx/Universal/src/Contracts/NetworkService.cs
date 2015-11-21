namespace OneCore.Hardware.Lifx.Universal.Contracts
{
    using System;
    using System.Collections.Generic;

    using Windows.Networking;
    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using OneCore.Hardware.Lifx.Contracts;

    public class NetworkService : INetworkService
    {
        #region Members

        private readonly List<INetworkClient> clients = new List<INetworkClient>();

        private const uint LifxPort = 56700;

        private readonly StreamSocketListener tcpServer;

        private readonly DatagramSocket udpClient;

        #endregion

        #region Public

        public NetworkService()
        {
            this.udpClient = new DatagramSocket();
            this.tcpServer = new StreamSocketListener();
        }

        public uint Port
        {
            get { return LifxPort; }
        }

        public void Open()
        {
            this.tcpServer.BindServiceNameAsync(LifxPort.ToString()).AsTask().Wait();
            this.udpClient.BindServiceNameAsync(LifxPort.ToString()).AsTask().Wait();

            this.tcpServer.ConnectionReceived += this.OnConnectionReceived;
            this.udpClient.MessageReceived += this.OnMessageReceived;
        }

        public void AddClient(INetworkClient client)
        {
            this.clients.Add(client);
        }

        public void RemoveClient(INetworkClient client)
        {
            this.clients.Remove(client);
        }

        #endregion

        private async void OnMessageReceived(
            DatagramSocket sender,
            DatagramSocketMessageReceivedEventArgs args)
        {
            var senderHostname = new HostName(args.RemoteAddress.RawName);

            var writer = new DataWriter(await sender.GetOutputStreamAsync(senderHostname, args.RemotePort));

            var stream = new NetworkStream(args.GetDataReader(), writer);

            this.clients.ForEach(x => x.MessageReceived(this, stream));
        }

        private async void OnConnectionReceived(
            StreamSocketListener sender,
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
        }

        //private async void OnConnectionReceived(
        //    StreamSocketListener sender,
        //    StreamSocketListenerConnectionReceivedEventArgs args)
        //{
        //    var reader = new DataReader(args.Socket.InputStream);

        //    var packet = ReceivePacket(reader);

        //    var response = GetResponse(packet, LifxService.Tcp);

        //    var writer = new DataWriter(args.Socket.OutputStream);

        //    var packetRaw = new byte[packet.Size];

        //    response.ToByteArray(this.bulb.Address, this.Mac, packetRaw);

        //    writer.WriteBytes(packetRaw);

        //    await writer.StoreAsync();
        //}
    }
}