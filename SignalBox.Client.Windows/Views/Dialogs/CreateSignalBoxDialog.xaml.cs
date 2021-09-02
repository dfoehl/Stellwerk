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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SignalBox.Client.Windows.Views.Dialogs
{
    public sealed partial class CreateSignalBoxDialog : ContentDialog
    {
        public CreateSignalBoxDialog()
        {
            this.InitializeComponent();
        }

        private async void PrimaryButtonClickAsync(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            switch (((typeComboBox.SelectedItem as ComboBoxItem).Content as string).ToLower())
            {
                case "can":
                    var response = await SignalBoxClient.PostNewCANSignalBoxAsync(idTextBox.Text, nameTextBox.Text, urlTextBox.Text);
                    args.Cancel = !response.Item1;
                    errorTextBlock.Text = response.Item2.ToString();
                    errorTextBlock.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        private void TypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((typeComboBox.SelectedItem as ComboBoxItem).Content as string).ToLower())
            {
                case "can":
                    canContentGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
