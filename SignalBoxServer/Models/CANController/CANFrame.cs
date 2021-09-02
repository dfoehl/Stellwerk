using System.Linq;

namespace SignalBox.Server.Models.CANController
{
    public class CANFrame
    {
        public uint Address { get; set; }
        public bool RemoteRequest { get; set; }
        public bool ErrorFrame { get; set; }
        public FrameFormat FrameFormat { get; set; }
        public byte DLC { get; set; }
        public byte[] Data { get; set; }

        public CANFrame()
        {
            Data = new byte[8];
            FrameFormat = FrameFormat.Standard;
        }

        public override string ToString()
        {
            return $"Address: {Address:X3} ({FrameFormat}) | RTR: {RemoteRequest} | Error: {ErrorFrame} | DLC: {DLC} \r\n" +
                $"{string.Join('.', Data.Select(x => x.ToString("X2")))}\r\n\r\n";
        }
    }

    public enum FrameFormat
    {
        Extended,
        Standard
    }
}
