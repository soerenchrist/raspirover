using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Base2Base.Abstractions.Connectivity.Bluetooth;
using PlayGround.Models;
using PlayGround.Services;
using PlayGround.Util;
using ReactiveUI;

namespace PlayGround.ViewModels
{
    public class BluetoothSettingsViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _connected;
        public bool Connected => _connected.Value;
        private readonly ObservableAsPropertyHelper<bool> _permissionGranted;
        public bool PermissionGranted => _permissionGranted.Value;
        
        private readonly ObservableAsPropertyHelper<IEnumerable<Device>?> _devices;
        public IEnumerable<Device>? Devices => _devices.Value;
        private string _message = "";
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        private Device? _selectedDevice;
        public Device? SelectedDevice
        {
            get => _selectedDevice;
            set => this.RaiseAndSetIfChanged(ref _selectedDevice, value);
        }
        
        private ReactiveCommand<Unit, bool> CheckPermissionCommand { get; }
        private ReactiveCommand<Unit, IEnumerable<Device>> GetBondedDevicesCommand { get; }
        public ReactiveCommand<Unit, bool> ConnectToDeviceCommand { get; }
        public ReactiveCommand<Unit, Unit> DisconnectCommand { get; }
        public ReactiveCommand<Unit,Unit> SendMessageCommand { get; }
        
        public BluetoothSettingsViewModel(IBluetoothService bluetoothService)
        {
            CheckPermissionCommand = ReactiveCommand.CreateFromTask(bluetoothService.CheckPermissionGranted);
            GetBondedDevicesCommand = ReactiveCommand.Create(bluetoothService.GetBondedDevices);
            DisconnectCommand = ReactiveCommand.CreateFromTask(bluetoothService.Disconnect);
            ConnectToDeviceCommand = ReactiveCommand.CreateFromTask<Unit, bool>(
                x =>
                {
                    if (SelectedDevice == null)
                        return Task.FromResult(false);
                    return bluetoothService.ConnectToDevice(SelectedDevice.Address);
                });
            SendMessageCommand = ReactiveCommand.CreateFromTask(_ => bluetoothService.SendData(byte.Parse(Message)));
            
            _permissionGranted = CheckPermissionCommand
                .ToProperty(this, x => x.PermissionGranted);
            _connected = ConnectToDeviceCommand
                .StartWith(false)
                .ToProperty(this, x => x.Connected);
            _devices = GetBondedDevicesCommand
                .ToProperty(this, x => x.Devices);
            
            this.GetIsActivated()
                .Where(x => x)
                .Select(_ => Unit.Default)
                .InvokeCommand(this, x => x.CheckPermissionCommand);

            this.WhenAnyValue(x => x.PermissionGranted)
                .Where(x => x)
                .Select(_ => Unit.Default)
                .InvokeCommand(this, x => x.GetBondedDevicesCommand);

            this.GetIsActivated()
                .Where(x => !x)
                .Select(_ => Unit.Default)
                .InvokeCommand(this, x => x.DisconnectCommand);
        }
    }
}