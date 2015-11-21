namespace Microsoft.IoT.Bulbs.UI
{
    using Microsoft.IoT.Bulbs;
    using Microsoft.Iot.Extended;

    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    using OneCore.Hardware.Lifx.Contracts;
    using OneCore.Hardware.Lifx.Core;
    using OneCore.Hardware.Lifx.Universal.Contracts;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IPhysicalLed
    {
        private readonly INetworkService networkService;
        private readonly byte[] mac = { 0xDE, 0xAD, 0xDE, 0xAD, 0xDE, 0xA5 };
        private readonly byte[] mac2 = { 0xDE, 0xAD, 0xDE, 0xAD, 0xDE, 0xA7 };

        public MainPage()
        {
            this.InitializeComponent();            

            this.networkService = new NetworkService();

            var hub = new LifxHub();

            var bulb1 = new LifxBulb(mac2, new GpioLedWrapper(new GpioRgbLed(26, 13, 06)))
            {
                Label = "RGB Led"
            };

            var bulb2 = new AnimatedBulb(new LifxBulb(this.mac, this))
            {
                Label = "Virtual Bulb"
            };

            hub.AddBulb(bulb1);
            hub.AddBulb(bulb2);
            
            this.networkService.AddClient(hub);
            this.networkService.Open();
        }

        public void ApplyColor(byte red, byte green, byte blue)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    this.Led.Fill = new SolidColorBrush(Color.FromArgb(255, red, green, blue));
                });

        }
    }
}
