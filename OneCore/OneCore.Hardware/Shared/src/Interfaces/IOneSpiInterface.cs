namespace OneCore.Hardware
{
    using System;

    public interface IOneSpiInterface : IDisposable
    {
        void WriteRead(byte[] bytesToWrite, byte[] bytesToRead);
    }
}
