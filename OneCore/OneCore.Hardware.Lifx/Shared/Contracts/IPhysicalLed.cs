namespace OneCore.Hardware.Lifx.Contracts
{
    public interface IPhysicalLed
    {
        void ApplyColor(byte red, byte green, byte blue);
    }
}