using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

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

        public async Task TakeImage()
        {
            var image = await CompositionRoot.Camera.TakeImage();
            await Clients.Caller.SendAsync("ImageTaken", image);
        }
    }
}
