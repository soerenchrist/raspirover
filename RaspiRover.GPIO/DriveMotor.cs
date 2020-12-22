using Microsoft.Extensions.Configuration;
using RaspiRover.GPIO.Contracts;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public sealed class DriveMotor : IDriveMotor, IDisposable
    {
        private int _speed;
        private readonly int _gpioBackward;
        private readonly int _gpioForward;
        private readonly GpioPin _forwardPwm;
        private readonly GpioPin _backwardPwm;

        public DriveMotor(IConfiguration configuration)
        {
            _gpioForward = configuration.GetValue<int>("GPIO:DriveForward");
            _gpioBackward = configuration.GetValue<int>("GPIO:DriveBackward");

            if (_gpioBackward == 0 || _gpioForward == 0)
                throw new ArgumentException("Missing configuration for GPIO:DriveForward or GPIO:DriveBackward");


            _forwardPwm = (GpioPin)Pi.Gpio[_gpioForward];
            _backwardPwm = (GpioPin)Pi.Gpio[_gpioBackward];
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
            _backwardPwm.SoftPwmValue = 0;
            _forwardPwm.SoftPwmValue = 0;
        }
    }
}
