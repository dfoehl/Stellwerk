using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Models.CAN
{
    public class CANSwitch : Switch
    {
        protected bool isStraight;
        public override bool IsStraight { get { return isStraight; } set { isStraight = value; } }
        public I2CController I2CController { get; set; }
        public byte StraightPin { get; set; }
        public byte DivergingPin { get; set; }

        public CANSwitch(SignalBox signalBox) : base(signalBox)
        {
        }

        public override Task ToogleAsync(bool? straight = null)
        {
            throw new InvalidOperationException();
        }
    }
}
