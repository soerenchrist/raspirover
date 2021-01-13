using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Base2Base.Abstractions.Connectivity.Bluetooth;
using PlayGround.Services;
using PlayGround.Util;
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
            ViewModel = new BluetoothSettingsViewModel(BluetoothService.Current);
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                this.OneWayBind(ViewModel, x => x.Devices, x => x.DevicesList.ItemsSource)
                    .DisposeWith(disposable);

                this.Bind(ViewModel, x => x.Message, x => x.MessageEntry.Text)
                    .DisposeWith(disposable);
                this.BindCommand(ViewModel, x => x.SendMessageCommand, x => x.SendButton)
                    .DisposeWith(disposable);
                
                Observable.FromEventPattern<ItemTappedEventArgs>(x => DevicesList.ItemTapped += x,
                    x => DevicesList.ItemTapped -= x)
                    .Select(args => (BluetoothDevice) args.EventArgs.Item)
                    .InvokeCommand(ViewModel, x => x.ConnectToDeviceCommand);
            });
        }
    }
}