using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Models.CAN
{
    public class CANSignal : Signal
    {
        protected SignalState signalState, nextSignalState;

        public override SignalState State { get => signalState; set => signalState = value; }
        public override SignalState NextSignalState { get => nextSignalState; set => nextSignalState = value; }
        public I2CController I2CController { get; set; }

        public CANSignal(SignalBox signalBox) : base(signalBox)
        {
        }

        public override Task SetStateAsync(SignalState state, SignalState nextSignalState = SignalState.Unknown)
        {
            throw new InvalidOperationException();
        }
    }
}
