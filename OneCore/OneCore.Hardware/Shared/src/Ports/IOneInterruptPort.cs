namespace OneCore.Hardware
{
    using System;

    public delegate void OneInterruptPortHandler(uint data1, uint data2, DateTime dateTime);

    public interface IOneInterruptPort : IDisposable
    {
        void EnableInterrupt();
        void DisableInterrupt();

        event OneInterruptPortHandler OnInterrupt;
    }
}