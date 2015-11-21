namespace OneCore.Hardware.Lifx.CoreV2
{
    using System;

    public interface ILifxBulbV2
    {
        bool IsOn { get; set; }

        string Label { get; set; }

        Version Version { get; }

        ulong Address { get; }
    }
}
