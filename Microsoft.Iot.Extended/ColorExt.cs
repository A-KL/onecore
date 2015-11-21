namespace Microsoft.Iot.Extended
{
    using Windows.UI;

    public static class ColorExt
    {
        public static byte[] ToRgb565(this Color color)
        {            
            var c = color.R >> 3;
            c <<= 6;
            c |= color.G >> 2;
            c <<= 5;
            c |= color.B >> 3;

            return new [] { (byte)(c >> 8), (byte)c };
        }
    }
}
