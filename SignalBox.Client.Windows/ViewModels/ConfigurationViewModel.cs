using SignalBox.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalBox.Client.Windows.ViewModels
{
    internal class ConfigurationViewModel : ViewModel
    {
        public ObservableCollection<Models.SignalBox> SignalBoxes { get; private set; }
        private string selectedNewType;

        public string SelectedNewType
        {
            get { return selectedNewType; }
            set
            {
                if (value != selectedNewType)
                {
                    selectedNewType = value;
                    InvokePropertyChanged();
                }
            }
        }


        public ConfigurationViewModel()
        {
            SignalBoxes = new ObservableCollection<Models.SignalBox>();
        }
    }
}
