using System;
using System.Collections.Generic;
using System.Text;

namespace SignalBox.Models
{
    public abstract class SignalBoxElement
    {
        public virtual string Id { get; set; }
        public SignalBox SignalBox { get; protected set; }

        public SignalBoxElement(SignalBox signalBox)
        {
            SignalBox = signalBox;
        }
    }
}
