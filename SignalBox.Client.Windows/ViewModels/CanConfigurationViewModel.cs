using SignalBox.Models.CAN;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Client.Windows.ViewModels
{
    public class CanConfigurationViewModel : ViewModel
    {
        private CANSignalBox signalBox;

        public CANSignal SelectedSignal { get; set; }
        public ObservableCollection<CANSignal> Signals { get; private set; }
        public CANSwitch SelectedSwitch { get; set; }
        public ObservableCollection<CANSwitch> Switches { get; private set; }
        public I2CController SelectedI2CDevice { get; set; }
        public ObservableCollection<I2CController> I2CDevices { get; private set; }
        public bool IsNew { get; internal set; }
        public CANSignalBox SignalBox { get { return signalBox; }
            set
            {
                if(value != signalBox)
                {
                    signalBox = value;
                    HasChanged = true;
                    InvokePropertyChanged();
                }
            }
        }

        public CanConfigurationViewModel()
        {
            Signals = new ObservableCollection<CANSignal>();
            Switches = new ObservableCollection<CANSwitch>();
            I2CDevices = new ObservableCollection<I2CController>();
        }

        internal async Task LoadSignalBoxAsync(string id = null)
        {
            id = id ?? signalBox.Id;

            SignalBox =  await SignalBoxClient.GetCANSignalBoxConfigAsync(SignalBox.Id);

            I2CDevices.Clear();
            foreach (var canController in signalBox.CANControllers)
            {
                foreach (var i2cDevices in canController.Slaves)
                {
                    I2CDevices.Add(i2cDevices);
                }
            }

            Signals.Clear();
            await SignalBoxClient.GetCANSignalsAsync(SignalBox);
            foreach (var signal in signalBox.Signals.Values)
            {
                Signals.Add(signal as CANSignal);
            }

            Switches.Clear();
            await SignalBoxClient.GetCANSwitchesAsync(SignalBox);
            foreach (var @switch in signalBox.Switches.Values)
            {
                Switches.Add(@switch as CANSwitch);
            }
        }
    }
}
