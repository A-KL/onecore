namespace OneCore.Hardware.Lifx
{
    public static class ArduinoHelper
    {
        public static float Map(float x, float xMax, float xMin, float outMax, float outMin)
        {
            return (x - xMin) * (outMax - outMin) / (xMax - xMin) + outMin;
        }

        public static int Map(int x, int xMax, int xMin, int outMax, int outMin)
        {
            return (x - xMin) * (outMax - outMin) / (xMax - xMin) + outMin;
        }
    }
}
