namespace OneCore.Hardware.Universal.Ports
{
    using System;
    using Windows.Devices.Gpio;

    public class OneCoreBasePort
    {
        private readonly GpioPin pin;

        protected OneCoreBasePort(int cpuPin, bool initialState, GpioPinDriveMode mode)
        {
            var controller = GpioController.GetDefault();

            if (controller == null)
            {
                throw new InvalidOperationException("GpioController");
            }

            this.pin = controller.OpenPin(cpuPin);
            this.pin.Write(initialState ? GpioPinValue.High : GpioPinValue.Low);
            this.pin.SetDriveMode(mode);

            this.pin.ValueChanged += this.OnValueChanged;
        }

        private void OnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            this.InvokeInterrupt(args.Edge);
        }

        public bool Read()
        {
            return this.pin.Read() == GpioPinValue.High;
        }

        public void Write(bool state)
        {
            this.pin.Write(state ? GpioPinValue.High : GpioPinValue.Low);
        }

        public void EnableInterrupt()
        {
            
        }

        public void DisableInterrupt()
        {
            
        }

        public event OneInterruptPortHandler OnInterrupt;

        private void InvokeInterrupt(GpioPinEdge edge)
        {
            if (this.OnInterrupt != null)
            {
                this.OnInterrupt((uint)this.pin.PinNumber, (uint)(edge == GpioPinEdge.RisingEdge ? 1 : 0), DateTime.Now);
            }
        }

        public void Dispose()
        {
            this.pin.Dispose();
        }
    }
}
