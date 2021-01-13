﻿using PlayGround.Services;
using PlayGround.Util;
using ReactiveUI;
using System;
using System.Numerics;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PlayGround.ViewModels
{
    public class ControlViewModel : ViewModelBase
    {
        private readonly ObservableAsPropertyHelper<Vector3> _orientation;
        private Vector3 Orientation => _orientation.Value;
        private bool _gyroMode;
        public bool GyroMode {
            get => _gyroMode;
            private set => this.RaiseAndSetIfChanged(ref _gyroMode, value);
        }

        private bool _frontLightOn;
        public bool FrontLightOn {
            get => _frontLightOn;
            set => this.RaiseAndSetIfChanged(ref _frontLightOn, value);
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

        public ReactiveCommand<Unit, Unit> BackCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleLightCommand { get; }

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

            ToggleLightCommand = ReactiveCommand.CreateFromTask(() =>
            {
                FrontLightOn = !FrontLightOn;
                return ControlService.Current.SetFrontLight(FrontLightOn);
            });
            BackCommand = ReactiveCommand.CreateFromTask(_ => Shell.Current.GoToAsync(".."));

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
                        if (GyroMode) Accelerometer.Start(SensorSpeed.UI);
                    }
                    else
                    {
                        if (GyroMode) Accelerometer.Stop();
                        ControlService.Current.Disconnect();
                    }
                }).Subscribe();

            this.WhenActivated(disposable =>
            {
                KeyManager.Current.ObserveKeyPress("Up")
                    .Select(keydown => keydown
                        ? Observable.Interval(TimeSpan.FromMilliseconds(20)).Select(_ => true)
                        : Observable.Return(false))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(down =>
                        {
                            if (down)
                            {
                                var newSpeed = Speed + 3;
                                Speed = Math.Min(newSpeed, 100);
                            }
                            else
                                Speed = 0;
                        })
                    .Subscribe()
                    .DisposeWith(disposable);
                KeyManager.Current.ObserveKeyPress("Down")
                    .Select(keydown => keydown
                        ? Observable.Interval(TimeSpan.FromMilliseconds(20)).Select(_ => true)
                        : Observable.Return(false))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(down =>
                    {
                        if (down)
                        {
                            var newSpeed = Speed - 3;
                            Speed = Math.Max(newSpeed, -100);
                        }
                        else
                            Speed = 0;
                    })
                    .Subscribe()
                    .DisposeWith(disposable);
                KeyManager.Current.ObserveKeyPress("Left")
                    .Select(keydown => keydown
                        ? Observable.Interval(TimeSpan.FromMilliseconds(20)).Select(_ => true)
                        : Observable.Return(false))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(down =>
                    {
                        if (down)
                        {
                            var newPosition = Position - 1;
                            Position = Math.Max(newPosition, -10);
                        }
                        else
                            Position = 0;
                    })
                    .Subscribe()
                    .DisposeWith(disposable);
                KeyManager.Current.ObserveKeyPress("Right")
                    .Select(keydown => keydown
                        ? Observable.Interval(TimeSpan.FromMilliseconds(20)).Select(_ => true)
                        : Observable.Return(false))
                    .Switch()
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .Do(down =>
                    {
                        if (down)
                        {
                            var newPosition = Position + 1;
                            Position = Math.Min(newPosition, 10);
                        }
                        else
                            Position = 0;
                    })
                    .Subscribe()
                    .DisposeWith(disposable);
            });

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
                return -10;
            if (orientation.Y > _rightY)
                return 10;

            const int lowerBound = -10;
            const int upperBound = 10;

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
