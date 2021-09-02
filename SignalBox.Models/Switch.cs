using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Models
{
    public abstract class Switch : TrackSegment
    {
        public virtual bool IsStraight { get; set; }
     
        protected Switch(SignalBox signalBox) : base(signalBox)
        {
        }

        public abstract Task ToogleAsync(bool? straight = null);
    }
}
