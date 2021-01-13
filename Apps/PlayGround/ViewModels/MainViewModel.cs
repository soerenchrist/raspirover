using System.Collections.Generic;
using System.Linq;
using PlayGround.Util;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using PlayGround.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _isGyroSupported;
        public bool IsGyroSupported => _isGyroSupported.Value;

        private bool _isGyroControl;
        public bool IsGyroControl {
            get => _isGyroControl;
            private set => this.RaiseAndSetIfChanged(ref _isGyroControl, value);
        }
        public ReactiveCommand<Unit, Unit> StartRoverCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCalibrationCommand { get; }
        public ReactiveCommand<Unit, Unit> StartSettingsCommand { get; }
        public ReactiveCommand<bool, Unit> SetControlCommand { get; }
        public ReactiveCommand<Unit, Unit> DevicesCommand { get; }
        
        public Interaction<List<Models.Device>, Models.Device?> SelectDeviceInteraction { get; }
        public Interaction<string, Unit> ErrorInteraction { get; }

        public MainViewModel()
        {
            SelectDeviceInteraction = new Interaction<List<Models.Device>, Models.Device?>();
            ErrorInteraction = new Interaction<string, Unit>();
            _isGyroSupported = Observable.Start(() =>
            {
                try
                {
                    Accelerometer.Start(SensorSpeed.UI);
                }
                catch (FeatureNotSupportedException)
                {
                    Preferences.Set(PreferenceKeys.IsGyro, false);
                    return false;
                }
                finally
                {
                    if (Accelerometer.IsMonitoring)
                        Accelerometer.Stop();
                }

                return true;
            }).ToProperty(this, x => x.IsGyroSupported);

            IsGyroControl = Preferences.Get(PreferenceKeys.IsGyro, true);
            StartRoverCommand = ReactiveCommand.CreateFromTask(StartRover);
            DevicesCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("devices"));
            StartCalibrationCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("calibrate"));
            StartSettingsCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("settings"));
            SetControlCommand = ReactiveCommand.Create<bool>(x =>
            {
                IsGyroControl = x;
                Preferences.Set(PreferenceKeys.IsGyro, x);
            });
        }

        private async Task StartRover()
        {
            var bluetoothService = DependencyService.Get<IBluetoothService>();
            var devices = bluetoothService.GetBondedDevices();

            var chosenDevice = await SelectDeviceInteraction.Handle(devices.ToList());
            if (chosenDevice == null)
                return;

            var connected = await bluetoothService.ConnectToDevice(chosenDevice.Address);
            if (!connected)
            {
                await ErrorInteraction.Handle("Es konnte keine Verbindung hergestellt werden");
                return;
            }    
            
            await Shell.Current.GoToAsync("control");
        }
    }
}
