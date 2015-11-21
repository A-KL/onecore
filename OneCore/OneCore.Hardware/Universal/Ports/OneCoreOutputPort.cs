namespace OneCore.Hardware.Universal.Ports
{
    using Windows.Devices.Gpio;

    public class OneCoreOutputPort : OneCoreBasePort, IOneOutputPort
    {
        public OneCoreOutputPort(int cpuPin, bool initialState) 
            : base(cpuPin, initialState, GpioPinDriveMode.Output)
        { }
    }
}
