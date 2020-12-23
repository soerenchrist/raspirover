using System;
using System.Threading.Tasks;

namespace RaspiRover.GPIO.Contracts
{
    public interface ICamera
    {
        Task<byte[]> TakeImage();
        IObservable<byte[]> StartVideoStream(TimeSpan interval);
    }
}
