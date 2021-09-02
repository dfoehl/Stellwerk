using SignalBox.Client.Windows.ViewModels;
using SignalBox.Client.Windows.Views.Dialogs;
using SignalBox.Models;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SignalBox.Client.Windows.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class ConfigurationMainPage : Page
    {
        private ConfigurationViewModel viewModel;

        public ConfigurationMainPage()
        {
            this.InitializeComponent();
            viewModel = new ConfigurationViewModel();
            DataContext = viewModel;

            LoadSignalBoxesAsync();
        }

        private async void LoadSignalBoxesAsync()
        {
            viewModel.SignalBoxes.Clear();

            var signalBoxes = await SignalBoxClient.GetSignalBoxesAsync();

            foreach (var sb in signalBoxes)
            {
                viewModel.SignalBoxes.Add(sb);
            }
        }

        private void SignalBoxClicked(object sender, ItemClickEventArgs e)
        {
            var selectedSignalBox = e.ClickedItem as Models.SignalBox;
            NavigateToConfigurationPage(selectedSignalBox.Type, selectedSignalBox);
        }

        private void NavigateToConfigurationPage(string signalBoxType, Models.SignalBox signalBox)
        {
            Type pageType;
            switch (signalBoxType)
            {
                case "CAN":
                    pageType = typeof(CanConfigurationPage);
                    break;
                default:
                    pageType = null;
                    break;
            }

            Frame.Navigate(pageType, signalBox);
        }

        private async void AddSignalBoxClickedAsync(object sender, RoutedEventArgs e)
        {
            var dialog = new CreateSignalBoxDialog();
            var response = await dialog.ShowAsync();
            if (response == ContentDialogResult.Primary)
                LoadSignalBoxesAsync();
        }
    }
}
