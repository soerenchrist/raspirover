using System.Reactive.Disposables;
using System.Reactive.Linq;
using PlayGround.Services;
using PlayGround.ViewModels;
using ReactiveUI;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PlayGround.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothSettingsView
    {
        public BluetoothSettingsView()
        {
            var blService = DependencyService.Get<IBluetoothService>();
            ViewModel = new BluetoothSettingsViewModel(blService);
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Devices, x => x.DevicesList.ItemsSource)
                    .DisposeWith(disposable);

                this.Bind(ViewModel, x => x.Message, x => x.MessageEntry.Text)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.SendMessageCommand, x => x.SendButton)
                    .DisposeWith(disposable);

                this.Bind(ViewModel, x => x.SelectedDevice, x => x.DevicesList.SelectedItem)
                    .DisposeWith(disposable);

                this.OneWayBind(ViewModel, x => x.Connected, x => x.ConnectionStateView.BackgroundColor,
                        x => x ? Color.Green : Color.Red)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.ConnectToDeviceCommand, x => x.ConnectButton)
                    .DisposeWith(disposable);
            });
        }
    }
}