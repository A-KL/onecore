namespace Microsoft.Iot.Extended.Graphics
{
    using System;
    using System.Threading.Tasks;

    public interface IScreenBus : IDisposable
    {
        Task Reset();

        void SendCommand(byte[] command);

        // One-byte commands
        void SendCommands(byte[] commands);

        // One-byte command
        void SendCommand(byte command);


        void SendData(byte[] data);

        void SendData(byte data);
    }
}
