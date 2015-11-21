namespace OneCore.Hardware.Lifx.Colors
{
    using System;

    public static class LifxColorConverter
    {
        public static LifxHsbkColor ToHsbk(LifxRgbColor color)
        {
            var result = new LifxHsbkColor();

            int max = Math.Max(color.Red, Math.Max(color.Green, color.Blue));
            int min = Math.Min(color.Red, Math.Min(color.Green, color.Blue));

            result.Brightness = max; // v

            var delta = max - min;

            if (max > 0.0)
            {
                result.Saturation = delta / max; // s
            }
            else
            {
                // r = g = b = 0                        // s = 0, v is undefresulted
                result.Saturation = 0;
                result.Hue = -1; // NAN;                            // its now undefresulted
                return result;
            }

            if (color.Red >= max) // > is bogus, just keeps compilor happy
            {
                result.Hue = (color.Green - color.Blue) / delta; // between yellow & magenta
            }
            else if (color.Green >= max)
            {
                result.Hue = (ushort)(2.0 + (color.Blue - color.Red) / delta); // between cyan & yellow
            }
            else
            {
                result.Hue = (ushort)(4.0 + (color.Red - color.Green) / delta); // between magenta & cyan
            }

            result.Hue *= 60.0; // degrees

            if (result.Hue < 0.0)
            {
                result.Hue += 360.0;
            }

            return result;
        }

        public static LifxRgbColor ToRgb(long kelvin)
        {
            var result = new LifxRgbColor();
            var temperature = kelvin / 100;

            if (temperature <= 66)
            {
                result.Red = 255;
            }
            else
            {
                var red = (int)temperature - 60;
                red = (int)(329.698727446 * Math.Pow(red, -0.1332047592));

                if (red < 0)
                {
                    result.Red = 0;
                }
                else if (red > 255)
                {
                    result.Red = 255;
                }
                else
                {
                    result.Red = (byte)red;
                }
            }

            if (temperature <= 66)
            {
                var green = (int)temperature;
                green = (int)(99.4708025861 * Math.Log(green) - 161.1195681661);
                if (green < 0)
                {
                    result.Green = 0;
                }
                else if (result.Green > 255)
                {
                    result.Green = 255;
                }
                else
                {
                    result.Green = (byte)green;
                }
            }
            else
            {
                var green = (int)(temperature - 60);
                green = (int)(288.1221695283 * Math.Pow(green, -0.0755148492));
                if (green < 0)
                {
                    result.Green = 0;
                }
                else if (result.Green > 255)
                {
                    result.Green = 255;
                }
                else
                {
                    result.Green = (byte)green;
                }
            }

            if (temperature >= 66)
            {
                result.Blue = 255;
            }
            else
            {
                if (temperature <= 19)
                {
                    result.Blue = 0;
                }
                else
                {
                    var blue = (int)(temperature - 10);
                    blue = (int)(138.5177312231 * Math.Log(blue) - 305.0447927307);
                    if (blue < 0)
                    {
                        result.Blue = 0;
                    }
                    else if (blue > 255)
                    {
                        result.Blue = 255;
                    }
                    else
                    {
                        result.Blue = (byte)blue;
                    }
                }
            }

            return result;
        }

        public static LifxRgbColor ToRgb(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * byte.MaxValue;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return new LifxRgbColor(v, t, p);

            if (hi == 1)
                return new LifxRgbColor(q, v, p);

            if (hi == 2)
                return new LifxRgbColor(p, v, t);

            if (hi == 3)
                return new LifxRgbColor(p, q, v);

            if (hi == 4)
                return new LifxRgbColor(t, p, v);

            return new LifxRgbColor(v, p, q);
        }

        public static LifxRgbColor ToRgb(LifxHsbkColor color)
        {
            return ToRgb(color.Hue, color.Saturation, color.Brightness);
        }
    }
}
