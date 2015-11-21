namespace Microsoft.Iot.Extended.Graphics
{
    using System.Threading.Tasks;

    public interface IScreen : IGraphics
    {
        Task Initialize();

        Task Reset();

        void Clear();
    }
}
