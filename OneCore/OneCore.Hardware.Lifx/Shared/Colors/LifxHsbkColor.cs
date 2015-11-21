namespace OneCore.Hardware.Lifx.Colors
{
    public class LifxHsbkColor
    {
        public const int Size = 8;

        public double Hue { get; set; }

        public double Saturation { get; set; }

        public double Brightness { get; set; }

        public int Kelvin { get; set; }

        public LifxRgbColor ToRgb()
        {
            if (this.Saturation < 0.2)
            {
                return LifxColorConverter.ToRgb(this.Kelvin);
            }

            return LifxColorConverter.ToRgb(this);
        }

        public static LifxHsbkColor FromBytes(byte[] data, int start = 0)
        {
            var hue = BitHelper.ReadUInt16(data, start);
            var saturation = BitHelper.ReadUInt16(data, start + 2);
            var brightness = BitHelper.ReadUInt16(data, start + 4);
            var kelvin = BitHelper.ReadUInt16(data, start + 6);

            return new LifxHsbkColor
            {
                Hue = ArduinoHelper.Map(hue, 0, ushort.MaxValue, 0, 359),
                Saturation = ArduinoHelper.Map(saturation, 0, ushort.MaxValue, 0, 1),
                Brightness = ArduinoHelper.Map(brightness, 0, ushort.MaxValue, 0, 1),
                Kelvin = kelvin
            };
        }       
    }
}
