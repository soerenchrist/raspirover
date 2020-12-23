using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using ReactiveUI;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        private readonly IBluetoothLE _ble;
        private readonly IAdapter _adapter;


        public SensorsViewModel()
        {
            _ble = CrossBluetoothLE.Current;
            _adapter = _ble.Adapter;

            BackCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync(".."));

            Observable.StartAsync(() =>
                _adapter.ConnectToKnownDeviceAsync(new Guid("00000000-0000-0000-0000-a0e6f8ae0607"))
            ).Do(x =>
            {
                Console.WriteLine(x);
            }).Subscribe();


            /* try
             {
                 _adapter.ScanMode = ScanMode.Balanced;
                 _adapter.DeviceDiscovered += (sender, args) =>
                 {
                     Console.WriteLine(args.Device);
                 };
                 _adapter.StartScanningForDevicesAsync();
             }
             catch (DeviceConnectionException e)
             {
             } */
        }
    }
}
