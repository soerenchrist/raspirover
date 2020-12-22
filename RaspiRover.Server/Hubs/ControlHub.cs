using Microsoft.AspNetCore.SignalR;

namespace RaspiRover.Server.Hubs
{
    public class ControlHub : Hub
    { 
        public void SetSpeed(int speed)
        {
            CompositionRoot.DriveMotor.Speed = speed;
            CompositionRoot.BackLight.On = speed < 0;    
        }

        public void SetSteerPosition(double position)
        {
            CompositionRoot.SteerMotor.Position = position;
        }
    }
}