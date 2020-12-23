using PlayGround.Services;
using PlayGround.Util;
using ReactiveUI;
using System;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class ControlViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<bool> _connected;
        public bool Connected => _connected.Value;

        private readonly ObservableAsPropertyHelper<double> _distance;
        public double Distance => _distance.Value;

        private readonly ObservableAsPropertyHelper<byte[]?> _image;
        public byte[]? Image => _image.Value;

        private readonly ObservableAsPropertyHelper<Vector3> _orientation;
        private Vector3 Orientation => _orientation.Value;
        private bool _gyroMode;
        public bool GyroMode {
            get => _gyroMode;
            private set => this.RaiseAndSetIfChanged(ref _gyroMode, value);
        }

        private double _speed;
        public double Speed {
            get => _speed;
            private set => this.RaiseAndSetIfChanged(ref _speed, value);
        }

        private double _position;
        public double Position {
            get => _position;
            private set => this.RaiseAndSetIfChanged(ref _position, value);
        }

        private bool _videoRunning;
        public bool VideoRunning {
            get => _videoRunning;
            private set => this.RaiseAndSetIfChanged(ref _videoRunning, value);
        }

        public ReactiveCommand<Unit, Unit> TakeImageCommand { get; }
        public ReactiveCommand<Unit, Unit> VideoCommand { get; }
        public ReactiveCommand<Unit, Unit> BackCommand { get; }

        private readonly float _fullSpeedZ;
        private readonly float _backSpeedZ;
        private readonly float _rightY;
        private readonly float _leftY;

        public ControlViewModel()
        {
            GyroMode = Preferences.Get(PreferenceKeys.IsGyro, true);

            _fullSpeedZ = Preferences.Get(PreferenceKeys.FullSpeed, 1f);
            _backSpeedZ = Preferences.Get(PreferenceKeys.BackSpeed, 0f);
            _rightY = Preferences.Get(PreferenceKeys.Right, 0.6f);
            _leftY = Preferences.Get(PreferenceKeys.Left, -0.6f);

            TakeImageCommand = ReactiveCommand.CreateFromTask(_ => ControlService.Current.TakeImage());
            BackCommand = ReactiveCommand.CreateFromTask(_ => Shell.Current.GoToAsync(".."));
            VideoCommand = ReactiveCommand.Create(() =>
            {
                if (VideoRunning)
                    ControlService.Current.StopVideo();
                else
                    ControlService.Current.StartVideo();
                VideoRunning = !VideoRunning;
            });

            _orientation = Observable.FromEventPattern<AccelerometerChangedEventArgs>(
                    x => Accelerometer.ReadingChanged += x,
                    x => Accelerometer.ReadingChanged -= x)
                .Select(x => x.EventArgs.Reading.Acceleration)
                .Sample(TimeSpan.FromSeconds(.1))
                .ToProperty(this, x => x.Orientation, scheduler: RxApp.MainThreadScheduler);


            if (GyroMode)
            {
                this.WhenAnyValue(x => x.Orientation)
                    .Select(CalculateSpeed)
                    .Do(x => Speed = x)
                    .Subscribe();

                this.WhenAnyValue(x => x.Orientation)
                    .Select(CalculatePosition)
                    .Do(x => Position = x)
                    .Subscribe();
            }

            this.WhenAnyValue(x => x.Speed)
                .Do(async x => await ControlService.Current.SetSpeed((int)x))
                .Subscribe();

            this.WhenAnyValue(x => x.Position)
                .Do(async x => await ControlService.Current.SetSteerPosition((int)x))
                .Subscribe();

            this.GetIsActivated()
                .Do(active =>
                {
                    if (active)
                    {
                        ControlService.Current.Connect();
                        if (GyroMode) Accelerometer.Start(SensorSpeed.UI);
                    }
                    else
                    {
                        ControlService.Current.Disconnect();
                        if (GyroMode) Accelerometer.Stop();
                    }
                }).Subscribe();

            _distance = this.GetIsActivated()
                .Select(x => x ? ControlService.Current.MeasureDistance() : Observable.Return(0.0))
                .Switch()
                .Do(Console.WriteLine)
                .ToProperty(this, x => x.Distance, scheduler: RxApp.MainThreadScheduler);

            _connected = this.GetIsActivated()
                .Select(x => x ? ControlService.Current.Connected : Observable.Return(false))
                .Switch()
                .ToProperty(this, x => x.Connected, scheduler: RxApp.MainThreadScheduler);

            _image = this.GetIsActivated()
                .Select(active => active ? ControlService.Current.LastImage : Observable.Return<byte[]?>(null))
                .Switch()
                .ToProperty(this, x => x.Image);

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
