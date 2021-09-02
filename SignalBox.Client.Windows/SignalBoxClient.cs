using Newtonsoft.Json;
using SignalBox.Models;
using SignalBox.Models.CAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Foundation;
using Windows.Web.Http;

namespace SignalBox.Client.Windows
{
    internal static class SignalBoxClient
    {
        public static string BaseUrl { get; set; }
        private static string SignalBoxUrl { get => $"{BaseUrl}/SignalBoxes"; }
        private static string SignalBoxCANConfigUrl { get => $"{SignalBoxUrl}/config/CAN"; }

        public static async Task<List<Models.SignalBox>> GetSignalBoxesAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri(SignalBoxUrl));

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<List<Models.SignalBox>>(await response.Content.ReadAsStringAsync());
            }
        }

        internal static async Task<Switch> GetSwitchAsync(Switch @switch)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri($"{SignalBoxUrl}/{@switch.SignalBox.Id}/Switches/{@switch.Id}"));

                return JsonConvert.DeserializeObject<Switch>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
        }

        internal static async Task<CANSignalBox> GetCANSignalBoxConfigAsync(string id)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri($"{SignalBoxCANConfigUrl}/{HttpUtility.UrlEncode(id)}"));

                if (!response.IsSuccessStatusCode)
                    return null;

                return JsonConvert.DeserializeObject<CANSignalBox>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects });
            }
        }

        internal static async Task<Tuple<bool, HttpStatusCode>> PostNewCANSignalBoxAsync(string id, string name, string baseUrl)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    new Uri($"{SignalBoxCANConfigUrl}?key={HttpUtility.UrlEncode(id)}&name={HttpUtility.UrlEncode(name)}&baseUrl={HttpUtility.UrlEncode(baseUrl)}"), new HttpStringContent(""));

                return new Tuple<bool, HttpStatusCode>(response.IsSuccessStatusCode, response.StatusCode);
            }
        }

        internal static async Task<bool> PatchSwitchDirectionAsync(Switch @switch, bool isStraight)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PatchAsync(
                    new Uri(
                        $"{SignalBoxUrl}/{HttpUtility.UrlEncode(@switch.SignalBox.Id)}/Switches/{HttpUtility.UrlEncode(@switch.Id)}/Direction?isNewDirectionStraight={isStraight}"),
                    new HttpStringContent("")
                    );

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> GetCANSwitchesAsync(CANSignalBox signalBox)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri($"{SignalBoxCANConfigUrl}/{signalBox.Id}/Switches"));

                signalBox.ReplaceSwitches(JsonConvert.DeserializeObject<IDictionary<string, CANSwitch>>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> GetCANSignalsAsync(CANSignalBox signalBox)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(new Uri($"{SignalBoxCANConfigUrl}/{signalBox.Id}/Signals"));

                signalBox.ReplaceSignals(JsonConvert.DeserializeObject<IDictionary<string, CANSignal>>(await response.Content.ReadAsStringAsync(), new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.Objects }));

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> RequestControllersAsync(CANSignalBox signalBox)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(new Uri($"{SignalBoxCANConfigUrl}/{signalBox.Id}/FetchController"), new HttpStringContent(""));

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> PostNewCANSignalAsync(CANSignalBox signalBox, I2CController i2cController, string signalId, SignalFunction function, Signal nextSignal = null)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    new Uri(
                        $"{SignalBoxCANConfigUrl}/{HttpUtility.UrlEncode(signalBox.Id)}/CANControllers/{i2cController.Master.Id}/slaves/{i2cController.Id}" +
                        $"/CreateSignal?function={function}&signalId={HttpUtility.UrlEncode(signalId)}&nextSignalId={HttpUtility.UrlEncode(nextSignal?.Id ?? string.Empty)}"),
                    new HttpStringContent("")
                    );

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> PostNewCANSwitchAsync(CANSignalBox signalBox, I2CController i2cController, string switchId, byte straightPin, byte divergingPin)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    new Uri(
                        $"{SignalBoxCANConfigUrl}/{HttpUtility.UrlEncode(signalBox.Id)}/CANControllers/{i2cController.Master.Id}/slaves/{i2cController.Id}" +
                        $"/CreateSwitch?switchId={HttpUtility.UrlEncode(switchId)}&straightPin={straightPin}&divergingPin={divergingPin}"),
                    new HttpStringContent("")
                    );

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> PatchSignalStateAsync(Signal signal, SignalState newState, SignalState newNextState = SignalState.Unknown)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PatchAsync(
                    new Uri(
                        $"{SignalBoxUrl}/{Uri.EscapeUriString(signal.SignalBox.Id)}/Signals/{Uri.EscapeUriString(signal.Id)}/State?newState={newState}&newNextState={newNextState}"),
                    new HttpStringContent("")
                    );

                return response.IsSuccessStatusCode;
            }
        }

        internal static async Task<bool> PatchIdentifySignal(CANSignalBox signalBox, I2CController i2CController)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    new Uri(
                        $"{SignalBoxCANConfigUrl}/{HttpUtility.UrlEncode(signalBox.Id)}/CANControllers/{i2CController.Master.Id}/slaves/{i2CController.Id}/IdentifySignal"),
                    new HttpStringContent("")
                    );

                return response.IsSuccessStatusCode;
            }
        }
    }

    internal static class HttpClientExtension
    {
        public static IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> PatchAsync(this HttpClient client, Uri requestUri, IHttpContent content)
        {
            return client.SendRequestAsync(new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = content
            });
        }
    }
}
