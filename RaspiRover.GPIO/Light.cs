using RaspiRover.GPIO.Contracts;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover.GPIO
{
    public class Light : ILight, IGpioPart
    {
        public int Pin { get; init; }

        private GpioPin? _gpioPin;
        private bool _on;

        public void Init()
        {
            _gpioPin = (GpioPin)Pi.Gpio[Pin];
            _gpioPin.PinMode = GpioPinDriveMode.Output;
        }

        public bool On {
            get => _on;
            set {
                if (_gpioPin == null)
                    throw new InvalidOperationException("You have to call init before setting lights on");

                _on = value;
                _gpioPin.Write(value);
            }
        }

        public void Dispose()
        {
            _gpioPin?.Write(false);
        }
    }
}
