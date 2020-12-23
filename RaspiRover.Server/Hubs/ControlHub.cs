using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RaspiRover.Communication;
using System.Threading.Tasks;

namespace RaspiRover.Server.Hubs
{
    public class ControlHub : Hub
    {
        private readonly ILogger _logger;

        public ControlHub(ILogger logger)
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
            await Clients.Others.SendAsync(Methods.SetSpeed, speed);
        }

        public async Task SetSteerPosition(double position)
        {
            await Clients.Others.SendAsync(Methods.SetSteerPosition, position);
        }

        public async Task StartVideo(int milliseconds)
        {
            await Clients.Others.SendAsync(Methods.StartVideo, milliseconds);
        }

        public async Task StartVideo()
        {
            await Clients.Others.SendAsync(Methods.StartVideo, 500);
        }

        public async Task StopVideo()
        {
            await Clients.Others.SendAsync(Methods.StopVideo);
        }

        public async Task TakeImage()
        {
            await Clients.Others.SendAsync(Methods.TakeImage);
        }

        public async Task ImageTaken(byte[] image)
        {
            await Clients.Others.SendAsync(Methods.ImageTaken, image);
        }

        public async Task ActivateDistanceMeasurement()
        {
            await Clients.Others.SendAsync(Methods.ActivateDistanceMeasurement);
        }


        public async Task DeactivateDistanceMeasurement()
        {
            await Clients.Others.SendAsync(Methods.DeactivateDistanceMeasurement);
        }

        public async Task DistanceMeasured(double distance)
        {
            await Clients.Others.SendAsync(Methods.DistanceMeasured, distance);
        }
    }
}
