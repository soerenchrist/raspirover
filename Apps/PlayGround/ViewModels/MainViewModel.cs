using PlayGround.Util;
using ReactiveUI;
using System.Reactive;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private bool _isGyroControl;
        public bool IsGyroControl {
            get => _isGyroControl;
            set => this.RaiseAndSetIfChanged(ref _isGyroControl, value);
        }
        public ReactiveCommand<Unit, Unit> StartRoverCommand { get; }
        public ReactiveCommand<Unit, Unit> StartCalibrationCommand { get; }
        public ReactiveCommand<Unit, Unit> StartSettingsCommand { get; }
        public ReactiveCommand<bool, Unit> SetControlCommand { get; }

        public MainViewModel()
        {
            IsGyroControl = Preferences.Get(PreferenceKeys.IsGyro, true);
            StartRoverCommand = ReactiveCommand.CreateFromTask(() =>
                IsGyroControl
                    ? Shell.Current.GoToAsync("gyro")
                    : Shell.Current.GoToAsync("manual"));
            StartCalibrationCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("calibrate"));
            StartSettingsCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("settings"));
            SetControlCommand = ReactiveCommand.Create<bool>(x =>
            {
                IsGyroControl = x;
                Preferences.Set(PreferenceKeys.IsGyro, x);
            });
        }

    }
}
