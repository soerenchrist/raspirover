using System;
using System.Threading.Tasks;

namespace PlayGround.Services
{
    public interface IControlService
    {
        IObservable<bool> Connected { get; }
        IObservable<byte[]?> LastImage { get; }

        Task Disconnect();
        Task Connect();
        Task SetSpeed(int speed);
        Task SetSteerPosition(int steerPosition);
        Task TakeImage();
        Task StartVideo();
        Task StopVideo();
    }
}