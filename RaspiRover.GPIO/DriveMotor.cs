using RaspiRover.GPIO.Contracts;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public sealed class DriveMotor : IDriveMotor, IGpioPart
    {
        public int PinForward { get; init; }
        public int PinBackward { get; init; }

        private GpioPin? _forwardPwm;
        private GpioPin? _backwardPwm;
        private int _speed;

        public void Init()
        {

            _forwardPwm = (GpioPin)Pi.Gpio[PinForward];
            _backwardPwm = (GpioPin)Pi.Gpio[PinBackward];
            _forwardPwm.PinMode = GpioPinDriveMode.Output;
            _backwardPwm.PinMode = GpioPinDriveMode.Output;
            _forwardPwm.StartSoftPwm(0, 100);
            _backwardPwm.StartSoftPwm(0, 100);
        }

        public int Speed {
            get => _speed;
            set {
                if (value > 100)
                    value = 100;
                if (value < -100)
                    value = -100;

                _speed = value;

                if (_forwardPwm == null || _backwardPwm == null)
                    throw new InvalidOperationException("Call init before setting the speed");

                if (_speed == value)
                    return;

                if (_speed == 0)
                {
                    _forwardPwm.SoftPwmValue = 0;
                    _backwardPwm.SoftPwmValue = 0;
                }
                else if (_speed > 0)
                {
                    _forwardPwm.SoftPwmValue = _speed;
                    _backwardPwm.SoftPwmValue = 0;
                }
                else
                {
                    _forwardPwm.SoftPwmValue = 0;
                    _backwardPwm.SoftPwmValue = _speed * -1;
                }
            }
        }

        public void Dispose()
        {
            if (_backwardPwm != null) _backwardPwm.SoftPwmValue = 0;
            if (_forwardPwm != null) _forwardPwm.SoftPwmValue = 0;
        }
    }
}
