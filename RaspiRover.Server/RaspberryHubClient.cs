using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaspiRover.GPIO;
using RaspiRover.GPIO.Config;
using RaspiRover.GPIO.Contracts;
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
        private readonly RoverConfiguration _configuration;
        private ICamera? _camera;
        private HubConnection? _connection;
        private IDisposable? _cameraDisposable;
        private IDisposable? _distanceDisposable;

        public RaspberryHubClient(IHostApplicationLifetime lifetime,
            ILogger<RaspberryHubClient> logger,
            RoverConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            InitializeGpios();
            lifetime.ApplicationStarted.Register(StartConnection);
        }

        private void InitializeGpios()
        {
            foreach (var motor in _configuration.Motors.Values)
            {
                motor.Init();
            }

            foreach (var light in _configuration.Lights.Values)
            {
                light.Init();
            }

            foreach (var servos in _configuration.Servos.Values)
            {
                servos.Init();
            }

            foreach (var distanceSensor in _configuration.DistanceSensors.Values)
            {
                distanceSensor.Init();
            }

            if (_configuration.Camera.Enabled)
            {
                _camera = new Camera();
            }
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

            _connection.On<int>("SetSpeed", speed =>
            {
                _logger.LogDebug($"Setting speed to {speed}");
                if (_configuration.Motors.ContainsKey("antrieb"))
                    _configuration.Motors["antrieb"].Speed = speed;

                if (_configuration.Lights.ContainsKey("ruecklicht"))
                {
                    _configuration.Lights["ruecklicht"].On = speed < 0;
                }
            });

            _connection.On<int>("SetSteerPosition", i =>
            {
                _logger.LogDebug($"Setting steer position to {i}");
                if (_configuration.Servos.ContainsKey("lenkung"))
                    _configuration.Servos["lenkung"].Position = i;
            });

            _connection.On("TakeImage", async () =>
            {
                _logger.LogDebug("Requested to take a picture");
                if (_camera != null)
                {
                    var image = await _camera.TakeImage();
                    await _connection.SendAsync("ImageTaken", image);
                }
            });

            _connection.On<int>("StartVideo", (interval) =>
            {
                _logger.LogDebug("Requested to start video streaming");
                if (_camera == null) return;
                _cameraDisposable = _camera.StartVideoStream(TimeSpan.FromMilliseconds(interval))
                    .Do(x => _connection.SendAsync("ImageTaken", x))
                    .Subscribe();
            });

            _connection.On("StopVideo", () =>
            {
                _logger.LogDebug($"Requested to stop video streaming");
                _cameraDisposable?.Dispose();
            });

            _connection.On("ActivateDistanceMeasurement", () =>
            {
                _logger.LogDebug("Activating distance measurements");
                if (_configuration.DistanceSensors.ContainsKey("front"))
                    _distanceDisposable = _configuration.DistanceSensors["front"]
                        .SubscribeToDistances()
                        .Subscribe(distance =>
                        {
                            _logger.LogDebug($"Measured distance of {Math.Round(distance)}cm");
                            _connection.SendAsync("DistanceMeasured", distance);
                        });
            });

            _connection.On("DeactivateDistanceMeasurement", () =>
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
