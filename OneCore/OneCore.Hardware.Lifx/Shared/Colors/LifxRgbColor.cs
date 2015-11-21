namespace OneCore.Hardware.Lifx.Colors
{
    public class LifxRgbColor
    {
        #region Constructor

        public LifxRgbColor()
        { }

        public LifxRgbColor(int value)
        {
            this.Red = (byte) ((value & 0xFF0000) >> 16);
            this.Green = (byte) ((value & 0x00FF00) >> 8);
            this.Blue = (byte) ((value & 0x0000FF));
        }

        public LifxRgbColor(byte red, byte green, byte blue)
        {
            this.Red = red;
            this.Blue = blue;
            this.Green = green;
        }


        public LifxRgbColor(int red, int green, int blue)
            :this((byte)red, (byte)green,(byte)blue)
        { }

        #endregion

        #region Properties

        public byte Red { get; set; }

        public byte Green { get; set; }

        public byte Blue { get; set; }

        #endregion

        #region Implicit cast

        public static implicit operator LifxRgbColor(int value)
        {
            return new LifxRgbColor(value);
        }

        #endregion

        #region Methods

        public LifxHsbkColor ToHsv()
        {
            return LifxColorConverter.ToHsbk(this);
        }

        #endregion     
    }
}
