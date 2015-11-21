namespace Microsoft.Iot.Extended
{
    using System;
    using Pwm;

    public class Servo
    {
        private const float Period = 20; // ms
        private const float DurationMin = 0.3f; // ms
        private const float DurationMax = 2.4f; // ms
        private readonly IPwmChannel pwmChannel;

        public Servo(int pin, int possition = 0)
        {
            var defaultDuration = TimeSpan.FromMilliseconds(Helpers.Map(possition, 180f, 0f, DurationMax, DurationMin));

            this.pwmChannel = new SoftwarePwm(pin, TimeSpan.FromMilliseconds(Period), defaultDuration);
            this.pwmChannel.Start();
        }

        public int Possition
        {
            get
            {
                return (int)Helpers.Map(pwmChannel.Duration.Milliseconds, DurationMax, DurationMin, 180f, 0f);
            }
            set
            {
                pwmChannel.Duration = TimeSpan.FromMilliseconds(Helpers.Map(value, 180f, 0f, DurationMax, DurationMin));
            }
        }


    }
}
