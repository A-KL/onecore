namespace Microsoft.Iot.Extended.Pwm
{
    using System;
    using System.Diagnostics;

    using Windows.Devices.Gpio;
    using Windows.Foundation;
    using Windows.System.Threading;

    public class SoftwarePwm : IPwmChannel
    {
        private readonly GpioPin gpioPin;
        private IAsyncAction action;
        private bool runPwm;
        private TimeSpan period;
        private TimeSpan duration;

        private Stopwatch stopwatch;

        // TODO: Implement constr with frequency parameter
        //public SoftwarePwm(GpioPin gpioPin, int frequency, int dutyCycle)
        //{
        //}

        public SoftwarePwm(int pin, TimeSpan period)
            : this(pin, period, TimeSpan.Zero)
        {
        }

        public SoftwarePwm(int pin, TimeSpan period, double dutyCycle)
            : this(pin, period, TimeSpan.FromTicks((long)(period.Ticks * dutyCycle)))
        {
        }

        public SoftwarePwm(int pin, TimeSpan period, TimeSpan duration)
        {
            var gpioController = GpioController.GetDefault();

            if (null == gpioController)
            {
                throw new Exception("No GpioController");
            }

            this.gpioPin = gpioController.OpenPin(pin);
            this.gpioPin.SetDriveMode(GpioPinDriveMode.Output);

            if (duration >= period)
            {
                throw new Exception("duration < period");
            }
            
            this.duration = duration;
            this.period = period;
        }

        public TimeSpan Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
            }
        }

        public double DutyCycle
        {
            get
            {
                return this.period.TotalMilliseconds / this.Duration.TotalMilliseconds;                
            }
            set
            {
                this.Duration = TimeSpan.FromMilliseconds(this.period.TotalMilliseconds * value);
            }
        }

        public void Start()
        {
            this.runPwm = true;
            this.stopwatch = Stopwatch.StartNew();
            this.action = ThreadPool.RunAsync(this.BackgroundTask, WorkItemPriority.High);            
        }

        public void Stop()
        {
            this.runPwm = false;
            this.stopwatch.Stop();
            this.action.Cancel();
        }
        
        public void Dispose()
        {
            this.action.Cancel();
            this.action.Close();
        }

        private void BackgroundTask(IAsyncAction a)
        {
            while (this.runPwm)
            {
                var lowDuration = this.period - this.duration;

                this.gpioPin.Write(GpioPinValue.High);
                this.Wait(this.duration.TotalMilliseconds);

                this.gpioPin.Write(GpioPinValue.Low);
                this.Wait(lowDuration.TotalMilliseconds);
            }
        }

        private void Wait(double milliseconds)
        {
            var initialTick = this.stopwatch.ElapsedTicks;
            long initialElapsed = this.stopwatch.ElapsedMilliseconds;
            var desiredTicks = milliseconds / 1000.0 * Stopwatch.Frequency;
            var finalTick = initialTick + desiredTicks;

            while (this.stopwatch.ElapsedTicks < finalTick)
            { }
        }
    }
}
