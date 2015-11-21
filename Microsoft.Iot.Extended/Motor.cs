namespace Microsoft.Iot.Extended
{
    using System;
    using Microsoft.Iot.Extended.Pwm;

    public class Motor
    {
        private const float Period = 200; // ms
        private readonly IPwmChannel pwmChannelA;
        private readonly IPwmChannel pwmChannelB;

        public Motor(int pinA, int pinB)
        {
            this.speed = 0;

            this.pwmChannelA = new SoftwarePwm(pinA, TimeSpan.FromMilliseconds(Period), 0);
            this.pwmChannelB = new SoftwarePwm(pinB, TimeSpan.FromMilliseconds(Period), 0);

            this.pwmChannelA.Start();
            this.pwmChannelB.Start();
        }

        public float Speed
        {
            get { return this.speed; }
            set
            {
                this.speed = value;
                this.ApplySpeed();
            }
        }

        private float speed;

        private void ApplySpeed()
        {
            if (this.speed == 0f)
            {
                this.pwmChannelA.Duration = TimeSpan.Zero;
                this.pwmChannelB.Duration = TimeSpan.Zero;
                return;
            }            

            var channel = this.speed > 0 ? this.pwmChannelA : this.pwmChannelB;
            var channelToStop = this.speed < 0 ? this.pwmChannelA : this.pwmChannelB;

            channelToStop.Duration = TimeSpan.Zero;
            channel.Duration = TimeSpan.FromMilliseconds(Helpers.Map(this.speed, 1f, -1f, Period, 0));
        }
    }
}
