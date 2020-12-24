using RaspiRover.GPIO.Contracts;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
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

        public IObservable<double> Distances()
        {
            if (_echoPin == null || _triggerPin == null)
                throw new InvalidOperationException("You have to call init before subscribing");
            return Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(_ => MeasureDistance(_triggerPin, _echoPin));
        }

        private double MeasureDistance(IGpioPin triggerPin, IGpioPin echoPin)
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(500);
            Stopwatch pulseLength = new Stopwatch();

            //Send pulse
            triggerPin.Write(GpioPinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            triggerPin.Write(GpioPinValue.Low);

            //Recieve pusle
            while (echoPin.Read() == false)
            {
            }
            pulseLength.Start();


            while (echoPin.Read())
            {
            }
            pulseLength.Stop();

            //Calculating distance
            TimeSpan timeBetween = pulseLength.Elapsed;
            Debug.WriteLine(timeBetween.ToString());
            double distance = timeBetween.TotalSeconds * 17000;

            return distance;
        }

        public void Dispose()
        {
            _triggerPin?.Write(false);
        }
    }
}
