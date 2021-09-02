using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SignalBox.Models;
using SignalBox.Server.Models;
using SignalBox.Server.Models.CANController;
using ModelCAN = SignalBox.Models.CAN;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalBox.Server.Controllers
{
    [Route("api/SignalBoxes/config/CAN")]
    [ApiController]
    public class CANSignalBoxConfigController : ControllerBase
    {
        // GET: api/<SignalBoxConfigController>
        [HttpGet]
        public IEnumerable<CANSignalBox> Get()
        {
            return ModelServer.Instance.SignalBoxes.Values.Where(sb => sb.GetType() == typeof(CANSignalBox)).Cast<CANSignalBox>();
        }

        // GET api/<SignalBoxConfigController>/5
        [HttpGet("{key}")]
        public CANSignalBox Get(string key)
        {
            var element = ModelServer.Instance.SignalBoxes[key];
            return element.GetType() == typeof(CANSignalBox) ? element as CANSignalBox : null;
        }

        // POST api/<SignalBoxConfigController>
        [HttpPost]
        public void Post([FromQuery] string key, [FromQuery] string name, [FromQuery] string baseUrl)
        {
            if (ModelServer.Instance.SignalBoxes.TryAdd(key, new CANSignalBox(baseUrl) { Id = key, Name = name }))
                Ok();

            Problem();
        }

        // DELETE api/<SignalBoxConfigController>/5
        [HttpDelete("{key}")]
        public void Delete(string key)
        {
            ModelServer.Instance.SignalBoxes.Remove(key, out _);
        }

        [HttpPost("{key}/FetchController")]
        public async void FetchController(string key)
        {
            var controller = (CANSignalBox)Get(key);

            if (controller == null)
            {
                NotFound(key);
                return;
            }

            if (await controller.StartScanForControllerAsync())
            {
                NoContent();
                return;
            }

            Problem();
        }

        [HttpGet("{key}/Switches")]
        public async Task<IDictionary<string, ModelCAN.CANSwitch>> GetSwitchesAsync(string key)
        {
            var signalBox = Get(key) as CANSignalBox;
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return null;
            }

            return signalBox.Switches.ToDictionary(k => k.Key, v => (ModelCAN.CANSwitch) v.Value);
        }

        [HttpGet("{key}/Signals")]
        public async Task<IDictionary<string, ModelCAN.CANSignal>> GetSignalsAsync(string key)
        {
            var signalBox = Get(key) as CANSignalBox;
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return null;
            }

            return signalBox.Signals.ToDictionary(k => k.Key, v => (ModelCAN.CANSignal)v.Value);
        }

        [HttpPost("{key}/CANControllers/{canId}/slaves/{i2cId}/CreateSignal")]
        public void CreateSignalFromI2C(string key, int canId, byte i2cId, [FromQuery] SignalFunction function, [FromQuery] string signalId, [FromQuery] string nextSignalId = null)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return;
            }

            var canController = signalBox.CANControllers.FirstOrDefault(cc => cc.Id == canId);
            if (canController == null)
            {
                NotFound("CanController");
                return;
            }

            var i2cController = canController.Slaves.FirstOrDefault(i2c => i2c.Id == i2cId);
            if (i2cController == null)
            {
                NotFound("Slave");
                return;
            }

            Signal nextSignal;
            if (nextSignalId == null || nextSignalId == string.Empty)
                nextSignal = null;
            else if (signalBox.Signals.ContainsKey(nextSignalId))
                nextSignal = signalBox.Signals[nextSignalId];
            else
            {
                NotFound("NextSignal");
                return;
            }

            signalBox.AddSignal(new CANSignal(signalBox, signalId, i2cController, function));
        }

        [HttpPost("{key}/CANControllers/{canId}/slaves/{i2cId}/CreateSwitch")]
        public void CreateSwithFromI2C(string key, int canId, byte i2cId, [FromQuery] string switchId, [FromQuery] byte straightPin, [FromQuery] byte divergingPin)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return;
            }

            var canController = signalBox.CANControllers.FirstOrDefault(cc => cc.Id == canId);
            if (canController == null)
            {
                NotFound("CanController");
                return;
            }

            var i2cController = canController.Slaves.FirstOrDefault(i2c => i2c.Id == i2cId);
            if (i2cController == null)
            {
                NotFound("Slave");
                return;
            }

            signalBox.AddSwitch(new CANSwitch(signalBox, switchId, i2cController, straightPin, divergingPin));
        }

        [HttpPost("{key}/CANControllers/{canId}/slaves/{i2cId}/IdentifySignal")]
        public async Task IdentifySignalAsync(string key, int canId, byte i2cId)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return;
            }

            var canController = signalBox.CANControllers.FirstOrDefault(cc => cc.Id == canId);
            if (canController == null)
            {
                NotFound("CanController");
                return;
            }

            var i2cController = canController.Slaves.FirstOrDefault(i2c => i2c.Id == i2cId);
            if (i2cController == null)
            {
                NotFound("Slave");
                return;
            }

            await signalBox.IdentifySignalAsync(i2cController);
        }
    }
}
