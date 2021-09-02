using SignalBox.Models;
using SignalBox.Models.CAN;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using TypeComboBoxItem = System.Collections.Generic.KeyValuePair<SignalBox.Models.SignalFunction, string>;

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SignalBox.Client.Windows.Views.Dialogs.CAN
{
    public sealed partial class CreateCANSwitchDialog : ContentDialog
    {
        private I2CController i2cController;
        private CANSignalBox signalBox;

        public CreateCANSwitchDialog(CANSignalBox signalBox, I2CController i2cController)
        {
            this.InitializeComponent();
            this.i2cController = i2cController;
            this.signalBox = signalBox;
            i2cIdTextBox.Text = $"0x{this.i2cController.Id:X}";
            canIdTextBox.Text = $"0x{this.i2cController.Master.Id:X}";
        }

        private async void PrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = !await SignalBoxClient.PostNewCANSwitchAsync(signalBox, i2cController, switchIdTextBox.Text, byte.Parse(straightPinTextBox.Text),byte.Parse(divergingPinTextBox.Text));
        }
    }
}
