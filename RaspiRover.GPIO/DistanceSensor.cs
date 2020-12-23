using RaspiRover.GPIO.Contracts;
using System;
using System.Reactive.Linq;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace RaspiRover.GPIO
{
    public sealed class DistanceSensor : INearbySensor, IGpioPart
    {
        public int EchoPin { get; init; }
        public int TriggerPin { get; init; }

        private IGpioPin? _triggerPin;
        private IGpioPin? _echoPin;

        public void Init()
        {
            _triggerPin = Pi.Gpio[TriggerPin];
            _echoPin = Pi.Gpio[EchoPin];

            _triggerPin.PinMode = GpioPinDriveMode.Output;
            _echoPin.PinMode = GpioPinDriveMode.Input;
        }

        public IObservable<double> SubscribeToDistances()
        {
            if (_echoPin == null || _triggerPin == null)
                throw new InvalidOperationException("You have to call init before subscribing");
            return Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => MeasureDistance(_triggerPin, _echoPin));
        }

        private double MeasureDistance(IGpioPin triggerPin, IGpioPin echoPin)
        {
            triggerPin.Write(true);
            Pi.Timing.SleepMicroseconds(10);
            triggerPin.Write(false);

            var startTime = DateTime.Now;
            var stopTime = DateTime.Now;
            while (echoPin.Value == false)
            {
                startTime = DateTime.Now;
            }

            while (echoPin.Value)
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
            _triggerPin?.Write(false);
        }
    }
}
