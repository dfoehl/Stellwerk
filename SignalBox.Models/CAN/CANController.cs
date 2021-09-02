using System.Collections.Generic;
using System.Linq;

namespace SignalBox.Models.CAN
{
    public class CANController
    {
        public uint Id { get; set; }
        public List<I2CController> Slaves { get; private set; }

        public CANController()
        {
            Slaves = new List<I2CController>();
        }

        public void AddSlave(byte id)
        {
            if (!Slaves.Any(s => s.Id == id))
                Slaves.Add(new I2CController() { Id = id, Master = this });
        }
    }
}
