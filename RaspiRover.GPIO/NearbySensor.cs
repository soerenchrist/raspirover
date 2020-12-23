using RaspiRover.GPIO.Contracts;
using System;
using System.Reactive.Linq;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace RaspiRover.GPIO
{
    public sealed class NearbySensor : INearbySensor, IDisposable
    {
        private readonly IGpioPin _triggerPin;
        private readonly IGpioPin _echoPin;

        public NearbySensor(int triggerPin, int echoPin)
        {
            _triggerPin = Pi.Gpio[triggerPin];
            _echoPin = Pi.Gpio[echoPin];

            _triggerPin.PinMode = GpioPinDriveMode.Output;
            _echoPin.PinMode = GpioPinDriveMode.Input;
        }

        public IObservable<double> SubscribeToDistances()
        {
            return Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => MeasureDistance());
        }

        private double MeasureDistance()
        {
            _triggerPin.Write(true);
            Pi.Timing.SleepMicroseconds(10);
            _triggerPin.Write(false);

            var startTime = DateTime.Now;
            var stopTime = DateTime.Now;
            while (_echoPin.Value == false)
            {
                startTime = DateTime.Now;
            }

            while (_echoPin.Value == true)
            {
                stopTime = DateTime.Now;
            }

            var timeElapsed = stopTime - startTime;
            const int speedOfSoundInCmProS = 34300;
            var distance = (timeElapsed.TotalSeconds * speedOfSoundInCmProS) / 2;

            return distance;
        }

        public void Dispose()
        {
            _triggerPin.Write(false);
        }
    }
}
