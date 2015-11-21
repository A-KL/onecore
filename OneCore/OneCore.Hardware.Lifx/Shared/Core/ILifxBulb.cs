namespace OneCore.Hardware.Lifx.Core
{
    using OneCore.Hardware.Lifx.Colors;

    public interface ILifxBulb
    {
        string Label { get; set; }

        string Location { get; set; }

        LifxHsbkColor Color { get; set; }

        ulong Address { get; }

        ushort Power { get; set; }

        void ToByteArray(byte[] data, uint start);
    }
}
