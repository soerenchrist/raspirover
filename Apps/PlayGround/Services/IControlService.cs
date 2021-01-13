using System;
using System.Threading.Tasks;

namespace PlayGround.Services
{
    public interface IControlService
    {
        Task SetSpeed(int speed);
        Task SetSteerPosition(int steerPosition);
        Task SetFrontLight(bool value);
        Task Disconnect();
    }
}