using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SignalBox.Models.CAN
{
    public class CANSignalBox : SignalBox
    {
        [JsonProperty("Signals")]
        private Dictionary<string, CANSignal> signals;
        private Dictionary<string, CANSwitch> switches;
        [JsonProperty("TrackSegments")]
        private Dictionary<string, TrackSegment> trackSegments;

        [JsonIgnore]
        public override IDictionary<string, Signal> Signals { get { return signals.ToDictionary(k => k.Key, v => (Signal)v.Value); } set { throw new InvalidOperationException(); } }
        [JsonIgnore]
        public override IDictionary<string, Switch> Switches { get { return switches.ToDictionary(k => k.Key, v => (Switch)v.Value); } set { throw new InvalidOperationException(); } }
        [JsonIgnore]
        public override IDictionary<string, TrackSegment> TrackSegments { get { return trackSegments; } set { throw new InvalidOperationException(); } }
        public ConcurrentBag<CANController> CANControllers { get; set; }
        public override string Name { get; set; }
        public override string Id { get; set; }

        public override string Type => "CAN";

        public CANSignalBox()
        {
            CANControllers = new ConcurrentBag<CANController>();
            signals = new Dictionary<string, CANSignal>();
            switches = new Dictionary<string, CANSwitch>();
            trackSegments = new Dictionary<string, TrackSegment>();
        }

        public async override Task UpdateSignalBoxAsync()
        {
            throw new NotImplementedException();
        }

        public void ReplaceSwitches(IDictionary<string, CANSwitch> source)
        {
            switches.Clear();
            foreach (var item in source)
            {
                switches.Add(item.Key, item.Value);
            }
        }

        public void ReplaceSignals(IDictionary<string, CANSignal> source)
        {
            signals.Clear();
            foreach (var item in source)
            {
                signals.Add(item.Key, item.Value);
            }
        }
    }
}
