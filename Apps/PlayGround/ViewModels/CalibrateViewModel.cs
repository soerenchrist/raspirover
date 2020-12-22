using System;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using PlayGround.Util;
using ReactiveUI;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class CalibrateViewModel : ViewModelBase
    {
        private ObservableAsPropertyHelper<Vector3> _magneticField;
        public Vector3 MagneticField => _magneticField.Value;
        
        private string _position = "";
        public string CurrentPosition
        {
            get => _position;
            set => this.RaiseAndSetIfChanged(ref _position, value);
        }
        
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        
        private int _currentStep;
        private string[] _steps = {
            "Vollgas",
            "Rückwärts",
            "Links",
            "Rechts"
        };
        
        public CalibrateViewModel()
        {
            CurrentPosition = _steps[0];
            SaveCommand = ReactiveCommand.CreateFromTask(Save);

            _magneticField = Observable.FromEventPattern<AccelerometerChangedEventArgs>(
                    x => Accelerometer.ReadingChanged += x,
                    x => Accelerometer.ReadingChanged -= x)
                .Select(x => x.EventArgs.Reading.Acceleration)
                .ToProperty(this, x => x.MagneticField);

            this.GetIsActivated()
                .Do(x =>
                {
                    if (x)
                        Accelerometer.Start(SensorSpeed.Game);
                    else
                        Accelerometer.Stop();
                })
                .Subscribe();
        }

        private async Task Save()
        {
            if (_currentStep == 3)
            {
                Accelerometer.Stop();
                await Shell.Current.GoToAsync("..");
                return;
            }

            var key = _currentStep switch
            {
                0 => PreferenceKeys.FullSpeed,
                1 => PreferenceKeys.BackSpeed,
                2 => PreferenceKeys.Left,
                _ => PreferenceKeys.Right,
            };

            var value = _currentStep switch
            {
                < 2 => MagneticField.Z,
                >= 2 => MagneticField.Y
            };
            
            Preferences.Set(key, value);

            _currentStep++;
            CurrentPosition = _steps[_currentStep];
        }
    }
}