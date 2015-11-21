namespace OneCore.Hardware.Lifx.Core
{
    using System;
    using System.Threading;

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

    public class AnimatedBulb : ILifxBulb
    {
        #region Members

        private readonly ILifxBulb bulb;
        private readonly Timer timer;
        private int fadeStepDuration;

        private int fadingStep;
        private int fadeSteps;

        private LifxHsbkColor fadeFromColor;
        private LifxHsbkColor fadeToColor;

        #region Dimm

        private static readonly byte[] DimmValuesArray =
        {
            0, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3,
            3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            4, 4, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 6, 6,
            6, 6, 6, 6, 6, 7, 7, 7, 7, 7, 7, 7, 8, 8, 8, 8,
            8, 8, 9, 9, 9, 9, 9, 9, 10, 10, 10, 10, 10, 11, 11, 11,
            11, 11, 12, 12, 12, 12, 12, 13, 13, 13, 13, 14, 14, 14, 14, 15,
            15, 15, 16, 16, 16, 16, 17, 17, 17, 18, 18, 18, 19, 19, 19, 20,
            20, 20, 21, 21, 22, 22, 22, 23, 23, 24, 24, 25, 25, 25, 26, 26,
            27, 27, 28, 28, 29, 29, 30, 30, 31, 32, 32, 33, 33, 34, 35, 35,
            36, 36, 37, 38, 38, 39, 40, 40, 41, 42, 43, 43, 44, 45, 46, 47,
            48, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62,
            63, 64, 65, 66, 68, 69, 70, 71, 73, 74, 75, 76, 78, 79, 81, 82,
            83, 85, 86, 88, 90, 91, 93, 94, 96, 98, 99, 101, 103, 105, 107, 109,
            110, 112, 114, 116, 118, 121, 123, 125, 127, 129, 132, 134, 136, 139, 141, 144,
            146, 149, 151, 154, 157, 159, 162, 165, 168, 171, 174, 177, 180, 183, 186, 190,
            193, 196, 200, 203, 207, 211, 214, 218, 222, 226, 230, 234, 238, 242, 248, 255,
        };

        #endregion

        #endregion

        #region Public

        public AnimatedBulb(ILifxBulb bulb)
        {
            this.bulb = bulb;

            this.fadeSteps = 70;
            this.fadingStep = 0;

            this.fadeStepDuration = (int)TimeSpan.FromMilliseconds(15).TotalMilliseconds;

            this.timer = new Timer(this.OnFadeTimerTick, null, Timeout.Infinite, Timeout.Infinite);
        }

        #region Properties

        public string Label
        {
            get { return this.bulb.Label; }
            set { this.bulb.Label = value; }
        }

        public string Location
        {
            get { return this.bulb.Location; }
            set { this.bulb.Location = value; }
        }

        public LifxHsbkColor Color
        {
            get
            {
                return this.bulb.Color;
            }
            set
            {
                if (this.fadeToColor == value)
                {
                    return;
                }

                this.StopFading();

                this.fadeFromColor = this.bulb.Color;
                this.fadeToColor = value;

                this.ResetFading();
            }
        }

        public ulong Address
        {
            get { return this.bulb.Address; }
        }

        public ushort Power
        {
            get { return this.bulb.Power; }
            set { this.bulb.Power = value; }
        }

        #endregion

        public void ToByteArray(byte[] data, uint start)
        {
            this.bulb.ToByteArray(data, start);
        }

        #endregion

        private void ResetFading()
        {
            this.StopFading();
            this.timer.Change(0, Timeout.Infinite);
        }

        //private void StartFading()
        //{
        //    this.StartFading(TimeSpan.Zero);
        //}

        //private void StartFading(TimeSpan delay)
        //{            
        //    this.timer.Change(delay, this.fadeStepDuration);
        //    this.isFading = true;
        //    this.fadingStep = 0;
        //}

        private void StopFading()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            this.fadingStep = 0;
        }

        private void OnFadeTimerTick(object state)
        {
            this.bulb.Color = Fade(this.fadeFromColor, this.fadeToColor, this.fadingStep, this.fadeSteps);
            //this.colorHsv = Fade(this.fadeFrom, this.fadeTo, this.fadingStep, this.FadeSteps)

            this.fadingStep++;

            if (this.fadeSteps == this.fadingStep)
            {
                this.StopFading();

                //switch (this.mode)
                //{
                //    case LifxBulbMode.RandomHue:
                //        this.FadeTo(new LifxRgbColor(), this.FadeDelay);
                //        break;

                //    case LifxBulbMode.Fixed:
                //        return;
                //}
            }
            else
            {
                this.timer.Change(this.fadeStepDuration, Timeout.Infinite);
            }
        }

        #region Static Methods

        private static LifxHsbkColor Fade(LifxHsbkColor from, LifxHsbkColor to, int step, int steps)
        {
            var hue = Fade(from.Hue, to.Hue, step, steps);
            var saturation = Fade(from.Saturation, to.Saturation, step, steps);
            var value = Fade(from.Brightness, to.Brightness, step, steps);

            return new LifxHsbkColor { Hue = hue, Saturation = saturation, Brightness = value, Kelvin = to.Kelvin };
        }

        //private static LifxRgbColor Fade(LifxRgbColor from, LifxRgbColor to, int step, int steps)
        //{
        //    var red = (byte)Fade(from.Red, to.Red, step, steps);
        //    var green = (byte)Fade(from.Red, to.Red, step, steps);
        //    var blue = (byte)Fade(from.Red, to.Red, step, steps);

        //    return new LifxRgbColor(red, green, blue);
        //}

        private static double Fade(double from, double to, int step, int steps)
        {
            return from - (step * ((from - to) / steps));
        }

        //private static float Fade(byte from, byte to, int step, int steps)
        //{
        //    return from - (step * ((from - to) / steps));
        //}

        #endregion
    }
}
