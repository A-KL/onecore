namespace Microsoft.Iot.Extended
{
    public static class Helpers
    {
        public static float Map(float x, float xMax, float xMin, float outMax, float outMin)
        {
            return (x - xMin) * (outMax - outMin) / (xMax - xMin) + outMin;
        }
    }
}
