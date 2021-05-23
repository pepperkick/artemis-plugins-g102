using Artemis.Core.DeviceProviders;
using Artemis.Core.Services;
using RgbNetDeviceProvider;
using Serilog;

namespace G102
{ 
    public class PluginDeviceProvider : DeviceProvider
    {
        private readonly IRgbService _rgbService;
        private readonly ILogger _logger;

        public PluginDeviceProvider(IRgbService rgbService, ILogger logger) : base(G102DeviceProvider.Instance)
        {
            _rgbService = rgbService;
            _logger = logger;
        }

        public override void Disable()
        {
            _logger.Information("Disabled G102 Plugin");
            _rgbService.RemoveDeviceProvider(RgbDeviceProvider);
        }

        public override void Enable()
        {
            _logger.Information("Enabled G102 Plugin");
            _rgbService.AddDeviceProvider(RgbDeviceProvider);
        }
    }
}