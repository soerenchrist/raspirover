using System;
using System.Device.Gpio;
using Microsoft.Extensions.Configuration;
using RaspiRover.GPIO.Contracts;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public class SteerMotor : ISteerMotor, IDisposable
    {
        private double _position;
        private readonly GpioPin _gpioPin;

        public SteerMotor(IConfiguration configuration)
        {
            var pin = configuration.GetValue<int>("GPIO:Steer");

            if (pin == 0)
                throw new ArgumentException("Missing configuration for GPIO:Steer");

            _gpioPin = (GpioPin)Pi.Gpio[pin];
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            _gpioPin.StartSoftPwm(0, 100);
        }

        public double Position
        {
            get => _position;
            set
            {
                if (value > 100)
                    value = 100;
                if (value < -100)
                    value = -100;

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
            _gpioPin.SoftPwmValue = 0;
        }
    }
}