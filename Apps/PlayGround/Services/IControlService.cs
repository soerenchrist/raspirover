using System;
using System.Threading.Tasks;

namespace PlayGround.Services
{
    public interface IControlService
    {
        IObservable<byte[]?> LastImage { get; }
        Task SetSpeed(int speed);
        Task SetSteerPosition(int steerPosition);
        Task TakeImage();
        Task StartVideo();
        Task StopVideo();
        IObservable<double> MeasureDistance();
        Task SetFrontLight(bool value);
    }
}