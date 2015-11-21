// 74HC595

namespace Microsoft.Iot.Extended
{
    using System;
    using System.Threading.Tasks;

    using Windows.Devices.Enumeration;
    using Windows.Devices.Spi;

    public class ShiftRegisterRgbModule
    {
        private SpiDevice shiftRegister;

        public const byte Red1 = (1 << 2);
        public const byte Red2 = (1 << 5);
        public const byte AllRed = Red1 | Red2;

        public const byte Green1 = (1 << 1);
        public const byte Green2 = (1 << 4);
        public const byte AllGreen = Green1 | Green2;

        public const byte Blue1 = (1 << 3);
        public const byte Blue2 = (1 << 6);
        public const byte AllBlue = Blue1 | Blue2;

        public void Write(params byte[] data)
        {
            this.shiftRegister.Write(data);
        }

        public async Task WriteWithDelay(byte[] data, int millisecondsDelay)
        {
            var color = new byte[1];

            foreach (var b in data)
            {
                color[0] = b;
                this.shiftRegister.Write(color);
                await Task.Delay(millisecondsDelay);
            }            
        }

        public async Task Init(string spi)
        {

            var settings = new SpiConnectionSettings(0)
            {
                ClockFrequency = 8000000,
                Mode = SpiMode.Mode0
            };

            var spiAqs = SpiDevice.GetDeviceSelector(spi);
            var devicesInfo = await DeviceInformation.FindAllAsync(spiAqs);

            this.shiftRegister = await SpiDevice.FromIdAsync(devicesInfo[0].Id, settings);        
        }
    }
}
