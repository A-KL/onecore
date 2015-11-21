namespace OneCore.Hardware.MicroFramework.Ports
{
    using System;
    using Microsoft.SPOT.Hardware;

    public class OneCoreInterruptPort : IOneInterruptPort
    {
        private readonly InterruptPort port;

        public OneCoreInterruptPort(int cpuPort, bool initialState, Port.ResistorMode resistorMode, Port.InterruptMode interruptMode)
        {
            this.port = new InterruptPort((Cpu.Pin)cpuPort, initialState, resistorMode, interruptMode);
            this.port.OnInterrupt += this.InvokeOnOnInterrupt;
        }

        public void EnableInterrupt()
        {
            throw new NotImplementedException();
        }

        public void DisableInterrupt()
        {
            throw new NotImplementedException();
        }

        public event OneInterruptPortHandler OnInterrupt;

        private void InvokeOnOnInterrupt(uint data1, uint data2, DateTime time)
        {
            if (this.OnInterrupt != null)
            {
                this.OnInterrupt(data1, data2, time);
            }
        }

        public void Dispose()
        {
            this.port.Dispose();
        }
    }
}
