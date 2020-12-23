using PlayGround.Util;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private ObservableAsPropertyHelper<bool> _isGyroSupported;
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

        public MainViewModel()
        {
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
                    Accelerometer.Stop();
                }

                return true;
            }).ToProperty(this, x => x.IsGyroSupported);


            IsGyroControl = Preferences.Get(PreferenceKeys.IsGyro, true);
            StartRoverCommand = ReactiveCommand.CreateFromTask(() => Shell.Current.GoToAsync("control"));
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
