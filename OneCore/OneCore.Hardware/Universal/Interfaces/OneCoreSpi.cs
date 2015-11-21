namespace OneCore.Hardware.Universal.Interfaces
{
    using System;
    using Windows.Devices.Spi;
    using Windows.Devices.Enumeration;
    using Windows.Devices.Gpio;

    public class OneCoreSpi : IOneSpiInterface
    {
        private readonly SpiDevice spi;

        public OneCoreSpi(string portName, int chipSelect, int speed)
        {
            var controller = GpioController.GetDefault();
            if (controller == null)
            {
                throw new InvalidOperationException("GpioController");
            }

            var settings = new SpiConnectionSettings(chipSelect);
            settings.ClockFrequency = speed;
            /* The display expects an idle-high clock polarity, we use Mode3    
             * to set the clock polarity and phase to: CPOL = 1, CPHA = 1  */
            settings.Mode = SpiMode.Mode3;

            var spiAqs = SpiDevice.GetDeviceSelector(portName);
            var devicesInfo = DeviceInformation.FindAllAsync(spiAqs).GetResults();

            this.spi = SpiDevice.FromIdAsync(devicesInfo[0].Id, settings).GetResults();
        }

        public void WriteRead(byte[] bytesToWrite, byte[] bytesToRead)
        {
            spi.Write(bytesToWrite);
            spi.Read(bytesToRead);
        }

        public void Dispose()
        {
            this.spi.Dispose();
        }
    }
}
