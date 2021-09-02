using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Models
{
    public abstract class Signal : SignalBoxElement
    {
        public abstract SignalState State { get; set; }
        public abstract SignalState NextSignalState { get; set; }
        public SignalType Type { get; set; }
        public SignalFunction Function {get;set;}

        protected Signal(SignalBox signalBox) : base(signalBox)
        {
        }

        public abstract Task SetStateAsync(SignalState state, SignalState nextSignalState = SignalState.Unknown);
    }

    public enum SignalState
    {
        Unknown,
        Stop,
        Go,
        Reduced,
        Shunting
    }

    public enum SignalFunction
    {
        Entry,
        EntryWithPreliminary,
        Exit,
        ExitWithPreliminary,
        Shunting,
        Preliminary
    }

    public enum SignalType
    {
        HVLight,
        HVSemaphore,
        KS
    }
}