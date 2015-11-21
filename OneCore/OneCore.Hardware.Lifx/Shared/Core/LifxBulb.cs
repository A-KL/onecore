namespace OneCore.Hardware.Lifx.Core
{
    using System;
    using System.Text;

    using OneCore.Hardware.Lifx.Colors;
    using OneCore.Hardware.Lifx.Contracts;

    public class LifxBulb : ILifxBulb
    {
        #region Members

        private string label;

        private long dim = 0;

        private ushort power = 65535;
        private LifxRgbColor colorRgb;
        private LifxHsbkColor colorHsbk;
        private readonly IPhysicalLed led;

        #endregion

        public LifxBulb(byte[] mac, IPhysicalLed led)
        {
            this.led = led;

            this.label = "Demo";

            this.Tags = new byte[LifxConst.BulbTagsLength];

            this.TagLabels = string.Empty;

            this.Address = BitHelper.ReadUInt64(mac, 0);

            this.colorHsbk = new LifxHsbkColor
            {
                Brightness = 1,
                Hue = 300,
                Saturation = 0,
                Kelvin = 2000
            };

            this.colorRgb = this.colorHsbk.ToRgb();

            this.Power = 65535;

            this.Location = "Home";
        }

        #region Properties

        public string Location
        {
            get; set;
        }

        public string Label
        {
            get
            {
                return this.label;
            }
            set
            {
                if (value.Length > LifxConst.BulbLabelLength)
                {
                    throw new ArgumentOutOfRangeException("Lable must be no more than" + LifxConst.BulbLabelLength);
                }
                this.label = value;
            }
        }

        public byte[] Tags { get; }

        public string TagLabels { get; }

        public bool IsOn { get; set; }

        public ulong Address { get; }

        public ushort Power
        {
            get
            {
                return this.power;
            }
            set
            {
                this.power = value;

                if (this.power > 0)
                {
                    this.led.ApplyColor(this.colorRgb.Red, this.colorRgb.Green, this.colorRgb.Blue);
                }
                else
                {
                    this.led.ApplyColor(0, 0, 0);

                }
            }
        }

        public LifxHsbkColor Color
        {
            get
            {
                return this.colorHsbk;
            }
            set
            {
                this.colorHsbk = value;
                this.colorRgb = value.ToRgb();

                this.led.ApplyColor(this.colorRgb.Red, this.colorRgb.Green, this.colorRgb.Blue);
            }
        }

        #endregion

        #region Methods

        public void ToByteArray(byte[] data, uint start)
        {
            var index = start;

            var hue = (int)ArduinoHelper.Map((float)this.colorHsbk.Hue, 0, 359, 0, ushort.MaxValue);
            var saturation = (int)ArduinoHelper.Map((float)this.colorHsbk.Saturation, 0, 1, 0, ushort.MaxValue);
            var brightness = (int)ArduinoHelper.Map((float)this.colorHsbk.Brightness, 0, 1, 0, ushort.MaxValue);

            // Color - 10 bytes
            data[index++] = BitHelper.LowByte(hue);
            data[index++] = BitHelper.HightByte(hue);

            data[index++] = BitHelper.LowByte(saturation);
            data[index++] = BitHelper.HightByte(saturation);

            data[index++] = BitHelper.LowByte(brightness);
            data[index++] = BitHelper.HightByte(brightness);

            data[index++] = BitHelper.LowByte(this.colorHsbk.Kelvin);
            data[index++] = BitHelper.HightByte(this.colorHsbk.Kelvin);

            data[index++] = BitHelper.LowByte(this.dim);
            data[index++] = BitHelper.HightByte(this.dim);

            // Power - 2 bytes
            var pwr = this.Power > 0 ? ushort.MaxValue : ushort.MinValue;

            data[index++] = BitHelper.LowByte(pwr);
            data[index++] = BitHelper.HightByte(pwr);

            // Label - 32 bytes
            var labelRaw = Encoding.UTF8.GetBytes(this.Label);
            Array.Copy(labelRaw, 0, data, (int)index, labelRaw.Length);
            index += LifxConst.BulbLabelLength;

            //  Tags - 8 byes
            Array.Copy(this.Tags, 0, data, (int)index, 8);
        }

        #endregion
    }
}