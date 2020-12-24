using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class SensorsViewModel : ViewModelBase
    {
        private bool _connected;
        public bool Connected {
            get => _connected;
            set => this.RaiseAndSetIfChanged(ref _connected, value);
        }

        private IDevice? _device;
        public IDevice? Device {
            get => _device;
            set => this.RaiseAndSetIfChanged(ref _device, value);
        }

        private IReadOnlyList<IService>? _services;
        public IReadOnlyList<IService>? Services {
            get => _services;
            set => this.RaiseAndSetIfChanged(ref _services, value);
        }

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
                Connected = true;
                Device = x;
            }).Subscribe();

            this.WhenAnyValue(x => x.Device)
                .Where(x => x != null)
                .Do(async x =>
                {
                    Services = await x!.GetServicesAsync();
                })
                .Subscribe();

            this.WhenAnyValue(x => x.Services)
                .Where(x => x != null)
                .Do(async x =>
                {
                    foreach (var service in x)
                    {
                        var characteristics = await service.GetCharacteristicsAsync();
                        foreach (var characteristic in characteristics)
                        {
                            Console.WriteLine(characteristic.Name);
                        }
                    }
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
