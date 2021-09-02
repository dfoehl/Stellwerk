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
    public sealed partial class CreateCANSignalDialog : ContentDialog
    {
        private I2CController i2cController;
        private CANSignalBox signalBox;

        public CreateCANSignalDialog(CANSignalBox signalBox, I2CController i2cController)
        {
            this.InitializeComponent();
            this.i2cController = i2cController;
            this.signalBox = signalBox;
            i2cIdTextBox.Text = $"0x{this.i2cController.Id:X}";
            canIdTextBox.Text = $"0x{this.i2cController.Master.Id:X}";
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.Entry, "Einfahrsignal"));
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.EntryWithPreliminary, "Einfahrsignal mit Vorsignal"));
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.Exit, "Ausfahrsignal"));
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.ExitWithPreliminary, "Ausfahrsignal mit Vorsignal"));
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.Preliminary, "Vorsignal"));
            signalFunctionComboBox.Items.Add(new TypeComboBoxItem(SignalFunction.Shunting, "Rangiersignal"));
        }

        private async void PrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            args.Cancel = !await SignalBoxClient.PostNewCANSignalAsync(signalBox, i2cController, signalIdTextBox.Text, ((TypeComboBoxItem)signalFunctionComboBox.SelectedItem).Key);
        }

        private async void IdentifySignalAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchIdentifySignal(signalBox, i2cController);
        }
    }
}
