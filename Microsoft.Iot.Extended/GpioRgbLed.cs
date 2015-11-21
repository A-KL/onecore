namespace Microsoft.Iot.Extended
{
    using System;
    using Windows.UI;
    using Microsoft.Iot.Extended.Pwm;

    public class GpioRgbLed : IDisposable
    {
        #region Members

        private static readonly TimeSpan Period = TimeSpan.FromMilliseconds(5);

        private readonly IPwmChannel redPwm;
        private readonly IPwmChannel greenPwm;
        private readonly IPwmChannel bluePwm;

        private Color color;

        #endregion


        public GpioRgbLed(int redPin, int greenPin, int bluePin)
        {
            this.redPwm = new SoftwarePwm(redPin, Period);
            this.greenPwm = new SoftwarePwm(greenPin, Period);
            this.bluePwm = new SoftwarePwm(bluePin, Period);

            this.redPwm.Start();
            this.greenPwm.Start();
            this.bluePwm.Start();            
        }

        public Color Color
        {
            get
            {
                return this.color;                
            }
            set
            {
                this.color = value;
                this.ApplyColor();
            }
        }

        public void Dispose()
        {
            this.redPwm.Dispose();
            this.greenPwm.Dispose();
            this.bluePwm.Dispose();
        }

        private void ApplyColor()
        {
            this.redPwm.DutyCycle = this.color.R / (double)byte.MaxValue;
            this.greenPwm.DutyCycle = this.color.G / (double)byte.MaxValue;
            this.bluePwm.DutyCycle = this.color.B / (double)byte.MaxValue;
        }
    }
}
