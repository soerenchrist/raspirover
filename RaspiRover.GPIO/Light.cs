using System;
using RaspiRover.GPIO.Contracts;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public class Light : ILight, IDisposable
    {
        private readonly GpioPin _gpioPin;
        private bool _on;

        public Light(int pin)
        {
            _gpioPin = (GpioPin)Pi.Gpio[pin];
            _gpioPin.PinMode = GpioPinDriveMode.Output;
            
        }

        public bool On
        {
            get => _on;
            set
            {
                _on = value;
                _gpioPin.Write(value);
            }
        }

        public void Dispose()
        {
            _gpioPin.Write(false);
        }
    }
}