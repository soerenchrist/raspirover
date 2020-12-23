using RaspiRover.GPIO.Contracts;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public class SteerMotor : ISteerMotor, IGpioPart
    {
        public int Pin { get; init; }

        private GpioPin? _gpioPin;


        public void Init()
        {
            _gpioPin = (GpioPin)Pi.Gpio[Pin];
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            _gpioPin.StartSoftPwm(0, 100);
        }

        public double Position {
            set {
                if (value > 100)
                    value = 100;
                if (value < -100)
                    value = -100;

                if (_gpioPin == null)
                    throw new InvalidOperationException("You have to call Init before settings the steer position");

                _gpioPin.SoftPwmValue = MapToRange(value);
            }
        }

        private static int MapToRange(double value)
        {
            const double left = 5;
            const double right = 15;

            var slope = (right - left) / (100 - -100);
            var output = left + slope * (value - -100);

            return (int)output;
        }

        public void Dispose()
        {
            if (_gpioPin != null) _gpioPin.SoftPwmValue = 0;
        }
    }
}
