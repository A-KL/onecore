namespace OneCore.Hardware
{
    using System;

    public interface IOneOutputPort : IDisposable
    {
        bool Read();

        void Write(bool state);
    }
}