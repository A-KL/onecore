namespace Microsoft.IoT.Bulbs
{
    using Windows.UI;
    using Microsoft.Iot.Extended;
    using OneCore.Hardware.Lifx.Contracts;

    public class GpioLedWrapper : IPhysicalLed
    {
        private readonly GpioRgbLed led;

        public GpioLedWrapper(GpioRgbLed led)
        {
            this.led = led;
        }

        public void ApplyColor(byte red, byte green, byte blue)
        {
            this.led.Color = Color.FromArgb(255, red, green, blue);
        }
    }
}
