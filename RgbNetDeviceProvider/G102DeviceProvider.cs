using RGB.NET.Core;
using System;
using System.Collections.Generic;
using WebSocketSharp;

namespace RgbNetDeviceProvider
{
    public class G102DeviceProvider : AbstractRGBDeviceProvider
    {
        private static G102DeviceProvider _instance;
        public static G102DeviceProvider Instance => _instance ?? new G102DeviceProvider();

        private WebSocket _client;

        private G102DeviceProvider()
        {
            if (_instance != null) throw new InvalidOperationException($"There can be only one instance of type {nameof(G102DeviceProvider)}");
            _instance = this;
        }

        protected override void InitializeSDK()
        {
            var ws = new WebSocket("ws://localhost:8700");
            _client = ws;
        }

        protected override IEnumerable<IRGBDevice> LoadDevices()
        {
            G102RgbUpdateQueue updateQueue = new G102RgbUpdateQueue(GetUpdateTrigger(), _client);
            yield return new G102Device(new G102DeviceInfo(RGBDeviceType.Mouse, "Logitech G102", "Logitech", "G102"), updateQueue);
        }

        public override void Dispose()
        {
        }
    }

    public class G102RgbUpdateQueue : UpdateQueue
    { 
        private readonly WebSocket _client;

        public G102RgbUpdateQueue(IDeviceUpdateTrigger updateTrigger, WebSocket client)
            : base(updateTrigger)
        {
            _client = client;
        }

        protected override void Update(in ReadOnlySpan<(object key, Color color)> dataSet)
        {
            foreach ((object key, Color color) in dataSet)
            {
                int[] data = new int[4];
                data[0] = (int) key;
                data[1] = color.GetR();
                data[2] = color.GetG();
                data[3] = color.GetB();
                string str = string.Join(":", data);

                _client.Connect();
                _client.Send(str);
                _client.Close();
            }
        }
    }
}
