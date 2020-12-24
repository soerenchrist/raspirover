using System;

namespace RaspiRover.GPIO.Contracts
{
    public interface INearbySensor
    {
        IObservable<double> Distances();
    }
}
