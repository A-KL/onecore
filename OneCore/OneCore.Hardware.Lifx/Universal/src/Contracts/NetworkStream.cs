namespace OneCore.Hardware.Lifx.Universal.Contracts
{
    using System;

    using Windows.Storage.Streams;

    using OneCore.Hardware.Lifx.Contracts;

    public class NetworkStream : IStream
    {
        private readonly IDataReader reader;
        private readonly IDataWriter writer;

        public NetworkStream(IDataReader reader, IDataWriter writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        public void WriteBytes(byte[] data)
        {
            this.writer.WriteBytes(data);
        }

        public void Store()
        {
            this.writer.StoreAsync().AsTask().Wait();
        }

        public void ReadBytes(byte[] data)
        {
            this.reader.ReadBytes(data);
        }
    }
}
