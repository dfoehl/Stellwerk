using SignalBox.Models;
using SignalBox.Models.CAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelCAN = SignalBox.Models.CAN;

namespace SignalBox.Server.Models.CANController
{
    public class CANSwitch : ModelCAN.CANSwitch
    {
        public CANSignalBox CANSignalBox => SignalBox as CANSignalBox;
        public CANSwitch(SignalBox.Models.SignalBox signalBox, string switchId, I2CController i2cController, byte straightPin, byte divergingPin) : base(signalBox)
        {
            Id = switchId;
            I2CController = i2cController;
            StraightPin = straightPin;
            DivergingPin = divergingPin;
        }

        public async override Task ToogleAsync(bool? straight = null)
        {
            var boolStraight = straight ?? !IsStraight;

            State = TrackState.Allocating;

            var frame = new CANFrame();
            frame.Address = I2CController.Master.Id;
            frame.Data = new byte[] { 0xEE, (byte)(I2CController.Id << 1), (byte)GetSwitchByte(boolStraight) };
            frame.DLC = 3;
            await CANSignalBox.SendFrameAsync(frame);

            await Task.Delay(1500);

            isStraight = boolStraight;
        }

        private byte GetSwitchByte(bool straight)
        {
            byte returnByte = 0b00010111; // Pull Output to high for 7 time fragments

            returnByte |= (byte) (straight ? 
                ((StraightPin << 5) & 0b11100000):
                ((DivergingPin << 5) & 0b11100000));

            return returnByte;
        }
    }
}
