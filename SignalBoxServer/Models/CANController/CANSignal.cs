using SignalBox.Models;
using SignalBox.Models.CAN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModelCAN = SignalBox.Models.CAN;

namespace SignalBox.Server.Models.CANController
{
    public class CANSignal : ModelCAN.CANSignal
    {
        public CANSignalBox CANSignalBox => SignalBox as CANSignalBox;
        public CANSignal(SignalBox.Models.SignalBox signalBox, string signalId, I2CController i2cController, SignalFunction function) : base(signalBox)
        {
            Id = signalId;
            I2CController = i2cController;
            Function = function;
            _ = SetStateAsync(SignalState.Stop);
        }

        public override async Task SetStateAsync(SignalState state, SignalState nextSignalState = SignalState.Unknown)
        {
            var frame = new CANFrame();
            frame.Address = I2CController.Master.Id;
            frame.Data = new byte[] { 0xEE, (byte)(I2CController.Id << 1), (byte)GetStateByte(state, nextSignalState) };
            frame.DLC = 3;
            await CANSignalBox.SendFrameAsync(frame);
            signalState = state;
            nextSignalState = nextSignalState;
        }

        private CANSignalState GetStateByte(SignalState state, SignalState nextSignalState)
        {
            if (state == SignalState.Shunting)
            {
                switch (Function)
                {
                    case SignalFunction.Shunting:
                        return CANSignalState.Sh1;
                    case SignalFunction.Exit:
                    case SignalFunction.ExitWithPreliminary:
                        return CANSignalState.HP0Sh1;
                    default:
                        return CANSignalState.Off;
                }
            }
            else if (state == SignalState.Stop)
            {
                switch (Function)
                {
                    case SignalFunction.Entry:
                    case SignalFunction.EntryWithPreliminary:
                        return CANSignalState.HP0;
                    case SignalFunction.Exit:
                    case SignalFunction.ExitWithPreliminary:
                    case SignalFunction.Shunting:
                        return CANSignalState.HP00;
                    default:
                        return CANSignalState.Off;
                }
            }
            else
            {
                switch (state)
                {
                    case SignalState.Unknown:
                        return CANSignalState.Off;
                    case SignalState.Go:
                        switch (nextSignalState)
                        {
                            case SignalState.Go:
                                return CANSignalState.HP1Vr1;
                            case SignalState.Reduced:
                                return CANSignalState.HP1Vr2;
                            case SignalState.Shunting:
                            case SignalState.Stop:
                                return CANSignalState.HP1Vr0;
                        }
                        return CANSignalState.HP1;
                    case SignalState.Reduced:
                        switch (nextSignalState)
                        {
                            case SignalState.Go:
                                return CANSignalState.HP2Vr1;
                            case SignalState.Reduced:
                                return CANSignalState.HP2Vr2;
                            case SignalState.Shunting:
                            case SignalState.Stop:
                                return CANSignalState.HP2Vr0;
                        }
                        return CANSignalState.HP2;
                }
            }

            return CANSignalState.Off;
        }

        enum CANSignalState : byte
        {
            HP0 = 0x00,
            HP00 = 0x01,
            HP00X = 0x0E,
            HP0X = 0x0F,
            HP1 = 0x11,
            HP1Vr0 = 0x33,
            HP1Vr1 = 0x55,
            HP1Vr2 = 0x77,
            HP2 = 0x88,
            HP2Vr0 = 0x99,
            HP2Vr1 = 0xBB,
            HP2Vr2 = 0xDD,
            HP0Sh1 = 0xEE,
            Sh1 = 0xFF,
            Off = 0xF0
        }
    }
}
