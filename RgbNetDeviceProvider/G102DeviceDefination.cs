using RGB.NET.Core;

namespace RgbNetDeviceProvider
{
    public class G102DeviceInfo : IRGBDeviceInfo
    {
        public RGBDeviceType DeviceType { get; protected set; }
        public string DeviceName { get; protected set; } = null!;
        public string Manufacturer { get; protected set; } = null!;
        public string Model { get; protected set; } = null!;
        public object? LayoutMetadata { get; set; }
        
        public G102DeviceInfo(RGBDeviceType deviceType, string deviceName, string manufacturer, string model)
        {
            DeviceType = deviceType;
            DeviceName = deviceName;
            Manufacturer = manufacturer;
            Model = model;
        }
    }

    public class G102Device : AbstractRGBDevice<G102DeviceInfo>
    {
        public G102Device(G102DeviceInfo info, IUpdateQueue updateQueue)
            : base(info, updateQueue)
        {
            InitializeLayout();
        }

        private void InitializeLayout()
        {
            LedId initial = LedId.Mouse1;

            int y = 0;
            Size ledSize = new Size(3);
            const int ledSpacing = 20;

            for (int i = 0; i < 3; i++)
            {
                LedId ledId = initial++;
                while (AddLed(ledId, new Point(ledSpacing * i, y), new Size(20, 20), i) == null)
                    ledId = initial++;
            }
        }
    }
}