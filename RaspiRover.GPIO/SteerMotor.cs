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
        public double UpperRange { get; init; }
        public double LowerRange { get; init; }

        private GpioPin? _gpioPin;


        public void Init()
        {
            _gpioPin = (GpioPin)Pi.Gpio[Pin];
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            _gpioPin.StartSoftPwm(0, 100);
        }

        public double Position {
            set {
                if (value > 10)
                    value = 10;
                if (value < -10)
                    value = -10;

                if (_gpioPin == null)
                    throw new InvalidOperationException("You have to call Init before settings the steer position");

                _gpioPin.SoftPwmValue = MapToRange(value);
            }
        }

        private int MapToRange(double value)
        {

            var slope = (UpperRange - LowerRange) / (10 - -10);
            var output = LowerRange + slope * (value - -10);

            Console.WriteLine($"Settings servo to {output}");

            return (int)output;
        }

        public void Dispose()
        {
            if (_gpioPin != null) _gpioPin.SoftPwmValue = 0;
        }
    }
}
