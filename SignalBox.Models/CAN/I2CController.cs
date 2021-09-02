namespace SignalBox.Models.CAN
{
    public class I2CController
    {
        public byte Id { get; set; }
        public I2CType Type { get; set; }
        public CANController Master { get; set; }
    }

    public enum I2CType
    {
        HVMux,
        KSMux,
        Output,
        Input
    }
}