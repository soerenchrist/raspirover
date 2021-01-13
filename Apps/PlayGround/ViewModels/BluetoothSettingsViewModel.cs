using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Base2Base.Abstractions.Connectivity.Bluetooth;
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
        
        private readonly ObservableAsPropertyHelper<IEnumerable<BluetoothDevice>?> _devices;
        public IEnumerable<BluetoothDevice>? Devices => _devices.Value;
        private string _message = "";
        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }
        
        
        private ReactiveCommand<Unit, bool> CheckPermissionCommand { get; }
        private ReactiveCommand<Unit, IEnumerable<BluetoothDevice>> GetBondedDevicesCommand { get; }
        public ReactiveCommand<BluetoothDevice, bool> ConnectToDeviceCommand { get; }
        public ReactiveCommand<Unit,Unit> SendMessageCommand { get; }
        
        public BluetoothSettingsViewModel(IBluetoothService bluetoothService)
        {
            CheckPermissionCommand = ReactiveCommand.CreateFromTask(bluetoothService.CheckPermissionGranted);
            GetBondedDevicesCommand = ReactiveCommand.Create(bluetoothService.GetBondedDevices);
            ConnectToDeviceCommand = ReactiveCommand.CreateFromTask<BluetoothDevice, bool>(
                x => bluetoothService.ConnectToDevice(x.Address));
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
        }
    }
}