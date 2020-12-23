using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace RaspiRover.Server.Hubs
{
    public class ControlHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine("Connected");
            return Task.CompletedTask;
        }

        public async Task SetSpeed(int speed)
        {
            await Clients.Others.SendAsync("ControlSpeed", speed);
        }

        public async Task SetSteerPosition(double position)
        {
            await Clients.Others.SendAsync("ControlSteer", position);
        }

        public async Task StartVideo(int milliseconds)
        {
            await Clients.Others.SendAsync("StartVideo", milliseconds);
        }

        public async Task StartVideo()
        {
            await Clients.Others.SendAsync("StartVideo", 500);
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
            Console.WriteLine($"Sending image with {image.Length} to clients");
            await Clients.Others.SendAsync("ImageTaken", image);
        }
    }
}
