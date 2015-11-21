namespace Microsoft.Iot.Extended.Pwm
{
    using System;

    public interface IPwmChannel : IDisposable
    {
        TimeSpan Duration
        {
            get;
            set;
        }

        double DutyCycle
        {
            get; set;
        }

        void Start();

        void Stop();
    }
}
