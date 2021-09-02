using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SignalBox.Models;
using SignalBox.Server.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SignalBox.Server.Controllers
{
    [Route("api/SignalBoxes")]
    [ApiController]
    public class SignalBoxController : ControllerBase
    {
        // GET: api/<SignalBoxController>
        [HttpGet]
        public IEnumerable<SignalBox.Models.SignalBox> Get()
        {
            return ModelServer.Instance.SignalBoxes.Values;
        }

        // GET api/<SignalBoxController>/5
        [HttpGet("{id}")]
        public SignalBox.Models.SignalBox Get(string id)
        {
            return ModelServer.Instance.SignalBoxes[id];
        }

        // DELETE api/<SignalBoxController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPatch("{key}/Signals/{signalId}/State")]
        public async void SetSignalState(string key, string signalId, [FromQuery] SignalState newState, [FromQuery] SignalState newNextState = SignalState.Unknown)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return;
            }

            if (signalBox.Signals.ContainsKey(signalId))
            {
                await signalBox.Signals[signalId].SetStateAsync(newState, newNextState);
            }
            else
                NotFound("Signal");
        }

        [HttpPatch("{key}/Switches/{switchId}/Direction")]
        public async void SetSwitchDirection(string key, string switchId, [FromQuery] bool isNewDirectionStraight)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return;
            }

            if (signalBox.Switches.ContainsKey(switchId))
            {
                await signalBox.Switches[switchId].ToogleAsync(isNewDirectionStraight);
            }
            else
                NotFound("Switch");
        }

        [HttpGet("{key}/Switches/{switchId}")]
        public async Task<Switch> GetSwitch(string key, string switchId)
        {
            var signalBox = Get(key);
            if (signalBox == null)
            {
                NotFound("SignalBox");
                return null;
            }

            if (signalBox.Switches.ContainsKey(switchId))
            {
                return signalBox.Switches[switchId];
            }
            else
                NotFound("Switch");

            return null;
        }
    }
}
