namespace OneCore.Hardware.MicroFramework.Ports
{
    using Microsoft.SPOT.Hardware;

    public class OneCoreOutputPort : IOneOutputPort
    {
        private readonly OutputPort port;

        public OneCoreOutputPort(int cpuPort, bool initialState)
        {
            this.port = new OutputPort((Cpu.Pin)cpuPort, initialState);            
        }

        public bool Read()
        {
            return this.port.Read();
        }

        public void Write(bool state)
        {
            this.port.Write(state);
        }

        public void Dispose()
        {
            this.port.Dispose();
        }
    }
}
