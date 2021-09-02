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

// Die Elementvorlage "Inhaltsdialogfeld" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace SignalBox.Client.Windows.Views.Dialogs
{
    public sealed partial class DebugSignalDialog : ContentDialog
    {
        public Signal Signal { get; private set; }
        public string SignalState { get => $"Status: {Signal.State}"; }

        public DebugSignalDialog(Signal signal)
        {
            Signal = signal;
            Title = $"Signal {signal.Id}";
            this.InitializeComponent();

            SetButtonVisibility();
        }

        private void SetButtonVisibility()
        {
            Hp0Button.Visibility = Visibility.Visible;
            Hp1Button.Visibility = Visibility.Visible;
            Hp2Button.Visibility = Visibility.Visible;
            Sh1Button.Visibility = Visibility.Visible;
            CodeLightButton.Visibility = Visibility.Visible;
            DSOffButton.Visibility = Visibility.Visible;
            Vr0Button.Visibility = Visibility.Visible;
            Vr1Button.Visibility = Visibility.Visible;
            Vr2Button.Visibility = Visibility.Visible;
            DSCodeLightButton.Visibility = Visibility.Visible;

            switch (Signal.State)
            {
                case Models.SignalState.Stop:
                    Hp0Button.Visibility = Visibility.Collapsed;
                    Vr0Button.Visibility = Visibility.Collapsed;
                    Vr1Button.Visibility = Visibility.Collapsed;
                    Vr2Button.Visibility = Visibility.Collapsed;
                    DSCodeLightButton.Visibility = Visibility.Collapsed;
                    DSOffButton.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Go:
                    Hp1Button.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Reduced:
                    Hp2Button.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Shunting:
                    Sh1Button.Visibility = Visibility.Collapsed;
                    DSOffButton.Visibility = Visibility.Collapsed;
                    Vr0Button.Visibility = Visibility.Collapsed;
                    Vr1Button.Visibility = Visibility.Collapsed;
                    Vr2Button.Visibility = Visibility.Collapsed;
                    DSCodeLightButton.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            switch (Signal.NextSignalState)
            {
                case Models.SignalState.Unknown:
                    DSOffButton.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Stop:
                    Vr0Button.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Go:
                    Vr1Button.Visibility = Visibility.Collapsed;
                    break;
                case Models.SignalState.Reduced:
                    Vr2Button.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private async void Hp0ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Models.SignalState.Stop);
            Signal.State = Models.SignalState.Stop;
            SetButtonVisibility();
        }

        private async void Hp1ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Models.SignalState.Go);
            Signal.State = Models.SignalState.Go;
            SetButtonVisibility();
        }

        private async void Hp2ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Models.SignalState.Reduced);
            Signal.State = Models.SignalState.Reduced;
            SetButtonVisibility();
        }

        private async void Sh1ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Models.SignalState.Shunting);
            Signal.State = Models.SignalState.Shunting;
            SetButtonVisibility();
        }

        private void CodeLightClicked(object sender, RoutedEventArgs e)
        {

        }

        private async void DistantOffClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Signal.State);
            Signal.NextSignalState = Models.SignalState.Unknown;
            SetButtonVisibility();
        }

        private async void Vr0ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Signal.State, Models.SignalState.Stop);
            Signal.NextSignalState = Models.SignalState.Stop;
            SetButtonVisibility();
        }

        private async void Vr1ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Signal.State, Models.SignalState.Go);
            Signal.NextSignalState = Models.SignalState.Go;
            SetButtonVisibility();
        }

        private async void Vr2ClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSignalStateAsync(Signal, Signal.State, Models.SignalState.Reduced);
            Signal.NextSignalState = Models.SignalState.Reduced;
            SetButtonVisibility();
        }

        private async void DSCodeLightClickedAsync(object sender, RoutedEventArgs e)
        {

        }
    }
}
