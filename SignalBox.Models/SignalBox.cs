using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalBox.Models
{
    public class SignalBox
    {
        public virtual IDictionary<string, Signal> Signals { get; set; }
        public virtual IDictionary<string, Switch> Switches { get; set; }
        public virtual IDictionary<string, TrackSegment> TrackSegments { get; set; }
        public virtual string Name { get; set; }
        public virtual string Id { get; set; }
        public virtual string Type { get; set; }
        public virtual Task UpdateSignalBoxAsync()
        {
            throw new InvalidOperationException();
        }

        public override string ToString()
        {
            return $"{Name} ({Id} - {Type})";
        }
    }
}
