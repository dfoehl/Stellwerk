using Newtonsoft.Json;
using ModelCAN = SignalBox.Models.CAN;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SignalBox.Models;
using System.Runtime.Serialization;

namespace SignalBox.Server.Models.CANController
{
    public class CANSignalBox : ModelCAN.CANSignalBox
    {
        private Uri canUri;
        private ConcurrentDictionary<string, CANSignal> signals;
        [JsonIgnore]
        private ConcurrentDictionary<string, CANSwitch> switches;

        public override IDictionary<string, Signal> Signals { get => signals.ToDictionary(k => k.Key, v => (Signal)v.Value); set => throw new NotSupportedException(); }
        [JsonProperty]
        public override IDictionary<string, Switch> Switches { get => switches.ToDictionary(k => k.Key, v => (Switch)v.Value); set => throw new NotSupportedException(); }

        public override string Name { get; set; }

        public CANSignalBox(string baseUrl)
        {
            signals = new ConcurrentDictionary<string, CANSignal>();
            switches = new ConcurrentDictionary<string, CANSwitch>();
            canUri = new Uri($"{baseUrl}/Can");
        }

        public void AddSignal(CANSignal signal)
        {
            signals.TryAdd(signal.Id, signal);
        }

        internal void AddSwitch(CANSwitch @switch)
        {
            switches.TryAdd(@switch.Id, @switch);
            base.Switches.Add(@switch.Id, @switch);
        }

        internal async Task<bool> SendFrameAsync(CANFrame frame)
        {
            using var client = new HttpClient();

            var reqContent = new StringContent(JsonConvert.SerializeObject(frame), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(canUri, reqContent);

            return response.IsSuccessStatusCode;
        }

        #region Frame Handling
        private void HandleFrame(CANFrame frame)
        {
            if (frame.Address == 0x7FF || frame.RemoteRequest)
                return;

            if (!CANControllers.Any(c => c.Id == frame.Address))
            {
                var controller = new ModelCAN.CANController() { Id = frame.Address };
                CANControllers.Add(controller);
                _ = StartScanForSlavesAsync(controller);
            }

            if (frame.DLC == 0)
                return;

            switch (frame.Data[0])
            {
                case 0xFF:
                    Handle0xffFrame(frame);
                    break;
                case 0xDD:
                case 0xDE:
                case 0xEE:
                default:
                    break;
            }
        }

        private void Handle0xffFrame(CANFrame frame)
        {
            switch (frame.DLC)
            {
                case 2:
                    if (frame.Data[1] != 255)
                        CANControllers.FirstOrDefault(c => c.Id == frame.Address)?.AddSlave((byte)(frame.Data[1]));
                    break;
                default:
                    break;
            }
        }
        #endregion

        public async override Task UpdateSignalBoxAsync()
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(canUri);
            var respContent = await response.Content.ReadAsStringAsync();
            var frames = JsonConvert.DeserializeObject<List<CANFrame>>(respContent);

            foreach (var frame in frames)
            {
                HandleFrame(frame);
            }
        }

        public async Task<bool> StartScanForControllerAsync()
        {
            using var client = new HttpClient();
            var reqFrame = new CANFrame();
            reqFrame.Address = 0x7FF;
            reqFrame.DLC = 1;
            reqFrame.Data = new byte[] { 0xFF };

            CANControllers.Clear();

            var reqContent = new StringContent(JsonConvert.SerializeObject(reqFrame), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(canUri, reqContent);

            return response.IsSuccessStatusCode;
        }

        private async Task<bool> StartScanForSlavesAsync(ModelCAN.CANController controller)
        {
            using var client = new HttpClient();
            var reqFrame = new CANFrame();
            reqFrame.Address = controller.Id;
            reqFrame.DLC = 1;
            reqFrame.RemoteRequest = true;

            controller.Slaves.Clear();

            var reqContent = new StringContent(JsonConvert.SerializeObject(reqFrame), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(canUri, reqContent);

            return response.IsSuccessStatusCode;
        }

        public async Task IdentifySignalAsync(ModelCAN.I2CController i2cController)
        {
            var signal = new CANSignal(this, "IDENTIFY", i2cController, SignalFunction.Entry);

            for (int i = 0; i < 2; i++)
            {
                await signal.SetStateAsync(SignalState.Stop);
                await Task.Delay(1500);
                await signal.SetStateAsync(SignalState.Go);
                await Task.Delay(1500);
            }

            signal = new CANSignal(this, "IDENTIFY", i2cController, SignalFunction.Shunting);

            for (int i = 0; i < 2; i++)
            {
                await signal.SetStateAsync(SignalState.Stop);
                await Task.Delay(1500);
                await signal.SetStateAsync(SignalState.Shunting);
                await Task.Delay(1500);
            }

            await signal.SetStateAsync(SignalState.Stop);
        }
    }
}
