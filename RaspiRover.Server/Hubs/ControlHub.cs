using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace RaspiRover.Server.Hubs
{
    public class ControlHub : Hub
    {
        private readonly ILogger<ControlHub> _logger;

        public ControlHub(ILogger<ControlHub> logger)
        {
            _logger = logger;
        }

        public override Task OnConnectedAsync()
        {
            _logger.LogDebug($"Connection established with {Context.ConnectionId}");
            return Task.CompletedTask;
        }

        public async Task SetSpeed(int speed)
        {
            await Clients.Others.SendAsync("SetSpeed", "antrieb", speed);
        }

        public async Task SetSteerPosition(double position)
        {
            await Clients.Others.SendAsync("SetSteerPosition", "lenkung", position);
        }

        public async Task StartVideo(int milliseconds)
        {
            await Clients.Others.SendAsync("StartVideo", milliseconds);
        }
        public async Task StopVideo()
        {
            await Clients.Others.SendAsync("StopVideo");
        }

        public async Task TakeImage()
        {
            await Clients.Others.SendAsync("TakeImage");
        }

        public async Task ImageTaken(byte[] image)
        {
            await Clients.Others.SendAsync("ImageTaken", image);
        }

        public async Task ActivateDistanceMeasurement()
        {
            await Clients.Others.SendAsync("ActivateDistanceMeasurement", "front");
        }


        public async Task DeactivateDistanceMeasurement()
        {
            await Clients.Others.SendAsync("DeactivateDistanceMeasurement", "front");
        }

        public async Task DistanceMeasured(double distance, string sensorName)
        {
            await Clients.Others.SendAsync("DistanceMeasured", distance);
        }
    }
}
