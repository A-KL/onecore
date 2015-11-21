using Windows.Devices.Gpio;

namespace OneCore.Hardware.Universal.Ports
{
    public class OneCoreInterruptPort : OneCoreBasePort, IOneInterruptPort
    {
        public OneCoreInterruptPort(int cpuPin, bool initialState) 
            : base(cpuPin, initialState, GpioPinDriveMode.Input)
        {
            
        }
    }
}
