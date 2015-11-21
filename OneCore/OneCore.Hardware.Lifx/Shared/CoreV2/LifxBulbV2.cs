namespace OneCore.Hardware.Lifx.CoreV2
{
    using System;
    using System.Threading;

    using OneCore.Hardware.Lifx.Core;
    using OneCore.Hardware.Lifx.Colors;

    public enum LifxBulbMode
    {
        Fixed,
        RandomHue,
        RainbowHue,
        Red,
        Blue,
        Green,
        Fire
    };

    public abstract class LifxBulbV2 : ILifxBulb
    {
        #region Public

        #region Properties

        public bool IsOn
        {
            get;
            set;
        }

        public string Label
        {
            get;
            set;
        }

        public string Location { get; set; }

        public Version Version
        {
            get;
            private set;
        }

        public LifxHsbkColor Color { get; set; }

        public UInt64 Address
        {
            get;
            private set;
        }

        public ushort Power { get; set; }

        public void ToByteArray(byte[] data, uint start)
        {
            throw new NotImplementedException();
        }

        public bool IsColorStill
        {
            get
            {
                return !this.IsColorFading;
            }
        }

        public bool IsColorFading
        {
            get;
            private set;
        }

        public LifxRgbColor CurrentColor
        {
            get
            {
                return this.color;
            }
            set
            {
                if (this.color == value)
                {
                    return;
                }

                if (this.IsColorFading)
                {
                    this.StopFading();
                }

                this.color = value;
                this.ApplyColor(this.color);
            }
        }

        public LifxBulbMode Mode
        {
            get;
            set;
        }

        /// <summary>
        /// The number of ms to hold color before fading again. (when cycling i.e. mode not Fixed)
        /// </summary>
        public TimeSpan FadeDelay
        {
            get; set;
        }

        public TimeSpan FadeStepDuration
        {
            get; set;
        }

        public int FadeSteps
        {
            get; set;
        }

        ulong ILifxBulb.Address
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        public void FadeTo(LifxRgbColor toColor)
        {
            this.FadeTo(toColor, TimeSpan.Zero);
        }

        public void FadeTo(LifxHsbkColor toColor)
        {
            
        }

        public void Dispose()
        {
            this.dimTimer.Dispose();
        }

        public byte[] GetData()
        {
            return null;
        }

        #endregion

        #endregion

        #region Protected

        protected LifxBulbV2()
        {
            this.IsOn = false;

            this.FadeSteps = 200;
            this.FadeStepDuration = new TimeSpan(0, 0, 0, 0, 50); // 50 msec

            this.FadeDelay = new TimeSpan(0, 0, 0, 1); // one second

            this.IsColorFading = false;

            this.Mode = LifxBulbMode.Fixed;

            this.fadingStep = 0;

            this.Label = this.label;

            this.Address = BitHelper.ReadUInt64(new byte[] { 10, 11, 12, 13, 14, 15 } , 0);

            this.dimTimer = new Timer(this.Callback, null, Timeout.Infinite, 0);
        }

        protected abstract void ApplyColor(LifxRgbColor color);

        #endregion

        #region Private

        #region Members

        #region Dimm

        private readonly static byte[] DimmValuesArray = 
        {
            0,   1,   1,   2,   2,   2,   2,   2,   2,   3,   3,   3,   3,   3,   3,   3,
            3,   3,   3,   3,   3,   3,   3,   4,   4,   4,   4,   4,   4,   4,   4,   4,
            4,   4,   4,   5,   5,   5,   5,   5,   5,   5,   5,   5,   5,   6,   6,   6,
            6,   6,   6,   6,   6,   7,   7,   7,   7,   7,   7,   7,   8,   8,   8,   8,
            8,   8,   9,   9,   9,   9,   9,   9,   10,  10,  10,  10,  10,  11,  11,  11,
            11,  11,  12,  12,  12,  12,  12,  13,  13,  13,  13,  14,  14,  14,  14,  15,
            15,  15,  16,  16,  16,  16,  17,  17,  17,  18,  18,  18,  19,  19,  19,  20,
            20,  20,  21,  21,  22,  22,  22,  23,  23,  24,  24,  25,  25,  25,  26,  26,
            27,  27,  28,  28,  29,  29,  30,  30,  31,  32,  32,  33,  33,  34,  35,  35,
            36,  36,  37,  38,  38,  39,  40,  40,  41,  42,  43,  43,  44,  45,  46,  47,
            48,  48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,
            63,  64,  65,  66,  68,  69,  70,  71,  73,  74,  75,  76,  78,  79,  81,  82,
            83,  85,  86,  88,  90,  91,  93,  94,  96,  98,  99,  101, 103, 105, 107, 109,
            110, 112, 114, 116, 118, 121, 123, 125, 127, 129, 132, 134, 136, 139, 141, 144,
            146, 149, 151, 154, 157, 159, 162, 165, 168, 171, 174, 177, 180, 183, 186, 190,
            193, 196, 200, 203, 207, 211, 214, 218, 222, 226, 230, 234, 238, 242, 248, 255,
        };

        #endregion

        private string label = "SmartHome Bulb";

        private const int LifxLabelMaxLength = 32;
        private const int LifxBulbTagsMaxLength = 8;

        private readonly Timer dimTimer;

        private int fadingStep;

        private LifxHsbkColor fadeFromHsv;
        private LifxHsbkColor fadeToHsv;
        private LifxHsbkColor colorHsv;

        private LifxRgbColor fadeFrom;
        private LifxRgbColor fadeTo;
        private LifxRgbColor color;

        private LifxBulbMode mode;

        private static Random random = new Random();

        // initial bulb values - warm white!
        private int power_status = 65535;
        private int hue = 0;
        private int sat = 0;
        private int bri = 65535;
        private int kel = 2000;
        private int dim = 0;

        #endregion

        #region Methods

        private void Callback(object state)
        {
            this.color = Fade(this.fadeFrom, this.fadeTo, this.fadingStep, this.FadeSteps);
            //this.colorHsv = Fade(this.fadeFrom, this.fadeTo, this.fadingStep, this.FadeSteps)

            this.ApplyColor(this.color);

            this.fadingStep++;

            if (this.FadeSteps == this.fadingStep)
            {
                this.StopFading();

                switch (this.mode)
                {
                    case LifxBulbMode.RandomHue:
                        this.FadeTo(new LifxRgbColor(), this.FadeDelay);
                        break;

                    case LifxBulbMode.Fixed:
                        return;
                }
            }
        }

        private void FadeTo(LifxRgbColor toColor, TimeSpan delay)
        {
            this.fadingStep = 0;
            this.fadeFrom = this.CurrentColor;
            this.fadeTo = toColor;

            this.StartFading(this.FadeDelay);
        }

        public void FadeTo(LifxHsbkColor toColor, TimeSpan delay)
        {
            this.fadingStep = 0;
            this.fadeFromHsv = this.colorHsv;
            this.fadeToHsv = toColor;

            this.StartFading(this.FadeDelay);
        }

        private void StartFading()
        {
            this.StartFading(TimeSpan.Zero);
        }

        private void StartFading(TimeSpan delay)
        {
            this.IsColorFading = true;
            this.dimTimer.Change(delay, this.FadeStepDuration);
        }

        private void StopFading()
        {
            this.dimTimer.Change(Timeout.Infinite, 0);
            this.IsColorFading = false;
            this.fadingStep = 0;
        }

        #endregion

        #region Static Methods

        private static LifxRgbColor Fade(LifxHsbkColor from, LifxHsbkColor to, int step, int steps)
        {
            var hue = Fade(from.Hue, to.Hue, step, steps);
            var saturation = Fade(from.Saturation, to.Saturation, step, steps);
            var value = Fade(from.Brightness, to.Brightness, step, steps);

            return null;// ToRgbColor(hue, saturation, value);
        }

        private static LifxRgbColor Fade(LifxRgbColor from, LifxRgbColor to, int step, int steps)
        {
            var red = (byte)Fade(from.Red, to.Red, step, steps);
            var green = (byte)Fade(from.Red, to.Red, step, steps);
            var blue = (byte)Fade(from.Red, to.Red, step, steps);

            return new LifxRgbColor(red, green, blue);
        }

        private static double Fade(double from, double to, int step, int steps)
        {
            return from - (step * ((from - to) / steps));
        }

        private static float Fade(byte from, byte to, int step, int steps)
        {
            return from - (step * ((from - to) / steps));
        }

        #endregion

        #endregion
    }
}