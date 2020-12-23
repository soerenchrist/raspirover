using System;

namespace RaspiRover.GPIO.Contracts
{
    public interface IGpioPart : IDisposable
    {
        void Init();
    }
}
