using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaspiRover.Communication;
using System;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RaspiRover.Server
{
    public class RaspberryHubClient : IHostedService
    {
        private readonly ILogger<RaspberryHubClient> _logger;
        private readonly CompositionRoot _compositionRoot;
        private HubConnection? _connection;
        private IDisposable? _cameraDisposable;
        private IDisposable? _distanceDisposable;

        public RaspberryHubClient(IHostApplicationLifetime lifetime,
            ILogger<RaspberryHubClient> logger,
            CompositionRoot compositionRoot)
        {
            _logger = logger;
            _compositionRoot = compositionRoot;
            lifetime.ApplicationStarted.Register(StartConnection);
        }

        private async void StartConnection()
        {
            _logger.LogInformation("Start connecting Raspberry Pi client to SignalR");

            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5000/control", opts =>
                {

                    opts.HttpMessageHandlerFactory = (message) =>
                    {
                        if (message is HttpClientHandler clientHandler)
                            // bypass SSL certificate
                            clientHandler.ServerCertificateCustomValidationCallback +=
                            (_, _, _, _) => true;
                        return message;
                    };

                })
                .Build();

            _connection.On<int>(Methods.SetSpeed, i =>
            {
                _logger.LogDebug($"Setting speed to {i}");
                _compositionRoot.DriveMotor.Speed = i;
                _compositionRoot.BackLight.On = i < 0;
            });

            _connection.On<int>(Methods.SetSteerPosition, i =>
            {
                _logger.LogDebug($"Setting steer position to {i}");
                _compositionRoot.SteerMotor.Position = i;
            });

            _connection.On(Methods.TakeImage, async () =>
            {
                _logger.LogDebug("Requested to take a picture");
                var image = await _compositionRoot.Camera.TakeImage();
                await _connection.SendAsync("ImageTaken", image);
            });

            _connection.On<int>(Methods.StartVideo, (interval) =>
            {
                _logger.LogDebug("Requested to start video streaming");
                _cameraDisposable = _compositionRoot.Camera.StartVideoStream(TimeSpan.FromMilliseconds(interval))
                    .Do(x => _connection.SendAsync(Methods.ImageTaken, x))
                    .Subscribe();
            });

            _connection.On(Methods.StopVideo, () =>
            {
                _logger.LogDebug($"Requested to stop video streaming");
                _cameraDisposable?.Dispose();
            });

            _connection.On(Methods.ActivateDistanceMeasurement, () =>
            {
                _logger.LogDebug("Activating distance measurements");
                _distanceDisposable = _compositionRoot.NearbySensor.SubscribeToDistances()
                    .Subscribe(distance =>
                    {
                        _logger.LogDebug($"Measured distance of {Math.Round(distance)}cm");
                        _connection.SendAsync(Methods.DistanceMeasured, distance);
                    });
            });

            _connection.On(Methods.DeactivateDistanceMeasurement, () =>
            {
                _logger.LogDebug("Deactivating distance measurement");
                _distanceDisposable?.Dispose();
            });

            await _connection.StartAsync();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Raspberry pi client");
            _connection?.StopAsync(cancellationToken);
            _cameraDisposable?.Dispose();
            return Task.CompletedTask;
        }
    }
}
