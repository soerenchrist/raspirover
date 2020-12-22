using System;
using System.Numerics;
using System.Reactive.Linq;
using PlayGround.Services;
using PlayGround.Util;
using ReactiveUI;
using Xamarin.Essentials;

namespace PlayGround.ViewModels
{
    public class RotationViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _connected;
        private readonly ObservableAsPropertyHelper<Vector3> _orientation;
        private readonly ObservableAsPropertyHelper<int> _speed;
        private readonly ObservableAsPropertyHelper<int> _steerPosition;
        private readonly float _fullSpeedZ;
        private readonly float _backSpeedZ;
        private readonly float _rightY;
        private readonly float _leftY;
     
        
        public Vector3 Orientation => _orientation.Value;
        public int Speed => _speed.Value;
        public int SteerPosition => _steerPosition.Value;
        public bool Connected => _connected.Value;

        public RotationViewModel()
        {
            _orientation = Observable.FromEventPattern<AccelerometerChangedEventArgs>(
                    x => Accelerometer.ReadingChanged += x,
                    x => Accelerometer.ReadingChanged -= x)
                .Select(x => x.EventArgs.Reading.Acceleration)
                .Sample(TimeSpan.FromSeconds(.1))
                .ToProperty(this, x => x.Orientation, scheduler: RxApp.MainThreadScheduler);

            _speed = this.WhenAnyValue(x => x.Orientation)
                .Select(CalculateSpeed)
                .ToProperty(this, x => x.Speed);
            
            _steerPosition = this.WhenAnyValue(x => x.Orientation)
                .Select(CalculatePosition)
                .ToProperty(this, x => x.SteerPosition);

            _fullSpeedZ = Preferences.Get(PreferenceKeys.FullSpeed, 1f);
            _backSpeedZ = Preferences.Get(PreferenceKeys.BackSpeed, 0f);
            _rightY = Preferences.Get(PreferenceKeys.Right, 0.6f);
            _leftY = Preferences.Get(PreferenceKeys.Left, -0.6f);

            this.WhenAnyValue(x => x.Speed)
                .Sample(TimeSpan.FromSeconds(1))
                .Do(async x => await ControlService.Current.SetSpeed(x))
                .Subscribe();
            this.WhenAnyValue(x => x.SteerPosition)
                .Sample(TimeSpan.FromSeconds(1))
                .Do(async x => await ControlService.Current.SetSteerPosition(x))
                .Subscribe();
            
            this.GetIsActivated()
                .Do(x =>
                {
                    if (x)
                    {
                        ControlService.Current.Connect();
                        Accelerometer.Start(SensorSpeed.UI);
                    }
                    else
                    {
                        Accelerometer.Stop();
                        ControlService.Current.Disconnect();
                    }
                })
                .Subscribe();
            
            _connected = this.GetIsActivated()
                .Select(x => x ? ControlService.Current.Connected : Observable.Return(false))
                .Switch()
                .ToProperty(this, x => x.Connected, scheduler: RxApp.MainThreadScheduler);
            
        }

        private int CalculateSpeed(Vector3 orientation)
        {
            if (orientation.Z > _fullSpeedZ)
                return 100;
            if (orientation.Z < _backSpeedZ)
                return -100;

            const int lowerBound = -100;
            const int upperBound = 100;

            return (int)MapToRange(orientation.Z, (_backSpeedZ, _fullSpeedZ), (lowerBound, upperBound));
        }
        
        
        private int CalculatePosition(Vector3 orientation)
        {
            if (orientation.Y < _leftY)
                return -100;
            if (orientation.Y > _rightY)
                return 100;

            const int lowerBound = -100;
            const int upperBound = 100;

            return (int)MapToRange(orientation.Y, (_leftY, _rightY), (lowerBound, upperBound));
        }

        private float MapToRange(float input, (float, float) inputRange, (float, float) outputRange)
        {
            var output = outputRange.Item1 + ((outputRange.Item2 - outputRange.Item1) / (inputRange.Item2 - inputRange.Item1)) *
                (input - inputRange.Item1);
            return output;
        }
    }
}