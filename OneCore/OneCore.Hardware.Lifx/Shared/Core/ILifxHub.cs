namespace OneCore.Hardware.Lifx.Core
{
    public interface ILifxHub
    {
        byte[] Mac { get; }

        ILifxBulb Bulbs(int index);

        void AddBulb(ILifxBulb bulb);

        void RemoveBulb(ILifxBulb bulb);

        int BulbsCount { get; }
    }
}
