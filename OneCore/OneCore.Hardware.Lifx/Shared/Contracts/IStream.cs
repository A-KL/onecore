namespace OneCore.Hardware.Lifx.Contracts
{
    public interface IStream
    {
        void ReadBytes(byte[] data);
        void WriteBytes(byte[] data);

        void Store();
    }
}
