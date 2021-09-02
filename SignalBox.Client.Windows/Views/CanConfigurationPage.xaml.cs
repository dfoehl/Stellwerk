using SignalBox.Client.Windows.ViewModels;
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
using SignalBox.Client.Windows.Views.Dialogs;
using SignalBox.Client.Windows.Views.Dialogs.CAN;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SignalBox.Client.Windows.Views
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CanConfigurationPage : Page
    {
        private CanConfigurationViewModel viewModel;

        public CanConfigurationPage()
        {
            this.InitializeComponent();
            viewModel = new CanConfigurationViewModel();
            DataContext = viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var signalBox = e.Parameter as Models.SignalBox;

            viewModel.SignalBox = await SignalBoxClient.GetCANSignalBoxConfigAsync(signalBox.Id);
            await viewModel.LoadSignalBoxAsync(signalBox.Id);
        }

        private async void AddSignalClickedAsync(object sender, RoutedEventArgs e)
        {
            var result = await new CreateCANSignalDialog(viewModel.SignalBox, viewModel.SelectedI2CDevice).ShowAsync();
            if (result == ContentDialogResult.Primary)
                await viewModel.LoadSignalBoxAsync();
        }

        private async void RemoveSignalClickedAsync(object sender, RoutedEventArgs e)
        {
            
        }

        private async void AddSwitchClickedAsync(object sender, RoutedEventArgs e)
        {
            var result = await new CreateCANSwitchDialog(viewModel.SignalBox, viewModel.SelectedI2CDevice).ShowAsync();
            if (result == ContentDialogResult.Primary)
                await viewModel.LoadSignalBoxAsync();
        }

        private async void RemoveSwitchClickedAsync(object sender, RoutedEventArgs e)
        {

        }

        private async void RequestControllersClicked(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.RequestControllersAsync(viewModel.SignalBox);
        }

        private async void RefreshControllersClicked(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadSignalBoxAsync();
        }

        private async void SignalDoubleTappedAsync(object sender, DoubleTappedRoutedEventArgs e)
        {
            await new DebugSignalDialog(viewModel.SelectedSignal).ShowAsync();
        }

        private async void SwitchDoubleTappedAsync(object sender, DoubleTappedRoutedEventArgs e)
        {
            await new DebugSwitchDialog(viewModel.SelectedSwitch).ShowAsync();
        }
    }
}
