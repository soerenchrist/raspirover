using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaspiRover.Server
{
    public class RaspberryHubClient : IHostedService
    {
        private HubConnection? _connection;
        private IDisposable? _cameraDisposable;

        public RaspberryHubClient(IHostApplicationLifetime lifetime)
        {
            lifetime.ApplicationStarted.Register(StartConnection);
        }

        private async void StartConnection()
        {
            Console.WriteLine("Connecting raspberry to signalr hub");

            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/control", opts =>
                {

                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                            (sender, certificate, chain, sslPolicyErrors) => true;
                        return message;
                    };

                })
                .Build();

            _connection.On<int>("ControlSpeed", i =>
            {
                Console.WriteLine($"Set speed to {i}");
                CompositionRoot.DriveMotor.Speed = i;
                CompositionRoot.BackLight.On = i < 0;
            });

            _connection.On<int>("ControlSteer", i =>
            {
                Console.WriteLine($"Set steer to {i}");
                CompositionRoot.SteerMotor.Position = i;
            });

            _connection.On("TakeImage", async () =>
            {
                Console.WriteLine("Requested to take picture");
                var image = await CompositionRoot.Camera.TakeImage();
                await _connection.SendAsync("ImageTaken", image);
            });

            _connection.On("StartVideo", () =>
            {
                Console.WriteLine("Start video");
                _cameraDisposable = CompositionRoot.Camera.StartVideoStream()
                    .Do(x => _connection.SendAsync("ImageTaken", x))
                    .Subscribe();
            });

            _connection.On("StopVideo", () =>
            {
                Console.WriteLine("Stop video");
                _cameraDisposable?.Dispose();
            });

            await _connection.StartAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection?.StopAsync(cancellationToken);
            _cameraDisposable?.Dispose();
            return Task.CompletedTask;
        }
    }
}
