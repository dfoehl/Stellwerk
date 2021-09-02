using SignalBox.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalBox.Server.Models
{
    public class ModelServer
    {
        private static ModelServer instance;
        public static ModelServer Instance
        {
            get
            {
                if (instance == null)
                    instance = new ModelServer();
                return instance;
            }
        }

        public ConcurrentDictionary<string, SignalBox.Models.SignalBox> SignalBoxes { get; private set; }

        public ModelServer()
        {
            SignalBoxes = new ConcurrentDictionary<string, SignalBox.Models.SignalBox>();
        }

        public void Save()
        {

        }
    }
}
