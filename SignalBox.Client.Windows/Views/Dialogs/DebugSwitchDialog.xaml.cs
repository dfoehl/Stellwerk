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
    public sealed partial class DebugSwitchDialog : ContentDialog
    {
        public Switch Switch { get; private set; }
        public string SwitchState { get => $"Status: {Switch.State}"; }

        public DebugSwitchDialog(Switch @switch)
        {
            Switch = @switch;
            Title = $"Weiche {Switch.Id}";
            this.InitializeComponent();

            SetButtonVisibility();
        }

        private void SetButtonVisibility()
        {
            ToggleButton.Visibility = Visibility.Visible;
            FreeButton.Visibility = Visibility.Visible;
            AllocatingButton.Visibility = Visibility.Visible;
            AllocatedButton.Visibility = Visibility.Visible;
            BlockedButton.Visibility = Visibility.Visible;

            switch (Switch.State)
            {
                case TrackState.Free:
                    FreeButton.Visibility = Visibility.Collapsed;
                    break;
                case TrackState.Allocating:
                    AllocatingButton.Visibility = Visibility.Collapsed;
                    break;
                case TrackState.Allocated:
                    AllocatedButton.Visibility = Visibility.Collapsed;
                    break;
                case TrackState.Blocked:
                    BlockedButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private async void ToggleClickedAsync(object sender, RoutedEventArgs e)
        {
            await SignalBoxClient.PatchSwitchDirectionAsync(Switch, !Switch.IsStraight);
            Switch.IsStraight = !Switch.IsStraight;
            //Switch = await SignalBoxClient.GetSwitchAsync(Switch);
            SetButtonVisibility();
        }

        private async void FreeClickedAsync(object sender, RoutedEventArgs e)
        {
            Switch.State = TrackState.Free;
            SetButtonVisibility();
        }

        private async void AllocatingClickedAsync(object sender, RoutedEventArgs e)
        {
            Switch.State = TrackState.Allocating;
            SetButtonVisibility();
        }

        private async void AllocatedClickedAsync(object sender, RoutedEventArgs e)
        {
            Switch.State = TrackState.Allocated;
            SetButtonVisibility();
        }
        private async void BlockedClickedAsync(object sender, RoutedEventArgs e)
        {
            Switch.State = TrackState.Blocked;
            SetButtonVisibility();
        }
    }
}
