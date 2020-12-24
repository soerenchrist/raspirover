using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RaspiRover.GPIO.Config;
using System;
using System.Collections.Generic;
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
        private HubConnection? _connection;
        private IDisposable? _cameraDisposable;
        private readonly Dictionary<string, IDisposable> _activeDistanceMeasurements = new();

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
            _logger.LogInformation($"Initializing {_configuration.Motors.Count} Motors");
            _logger.LogInformation($"Initializing {_configuration.Servos.Count} Servos");
            _logger.LogInformation($"Initializing {_configuration.Lights.Count} Lights");
            _logger.LogInformation($"Initializing {_configuration.DistanceSensors.Count} Distance Sensors");
            _logger.LogInformation($"Camera enabled: {_configuration.Camera != null}");
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

            _connection.On<string, int>("SetSpeed", (motorName, speed) =>
            {
                _logger.LogDebug($"Setting speed of {motorName} to {speed}");
                if (_configuration.Motors.TryGetValue(motorName, out var motor))
                    motor.Speed = speed;
            });

            _connection.On<string, int>("SetSteerPosition", (servoName, position) =>
            {
                _logger.LogDebug($"Setting steer position of {servoName} to {position}");
                if (_configuration.Servos.TryGetValue(servoName, out var servo))
                    servo.Position = position;
            });

            _connection.On("TakeImage", async () =>
            {
                _logger.LogDebug("Requested to take a picture");
                if (_configuration.Camera != null)
                {
                    var image = await _configuration.Camera.TakeImage();
                    await _connection.SendAsync("ImageTaken", image);
                }
            });

            _connection.On<int>("StartVideo", (interval) =>
            {
                _logger.LogDebug("Requested to start video streaming");
                if (_configuration.Camera == null) return;
                _cameraDisposable = _configuration.Camera.StartVideoStream(TimeSpan.FromMilliseconds(interval))
                    .Do(x => _connection.SendAsync("ImageTaken", x))
                    .Subscribe();
            });

            _connection.On("StopVideo", () =>
            {
                _logger.LogDebug("Requested to stop video streaming");
                _cameraDisposable?.Dispose();
            });

            _connection.On<string, bool>("SetLight", (lightName, state) =>
            {
                _logger.LogDebug($"Toggling light {lightName}");
                if (_configuration.Lights.TryGetValue(lightName, out var light))
                {
                    light.On = state;
                }
            });

            _connection.On<string>("ActivateDistanceMeasurement", sensorName =>
            {
                _logger.LogDebug($"Activating distance measurements on {sensorName}");

                if (_configuration.DistanceSensors.TryGetValue(sensorName, out var sensor))
                {
                    var disposable = sensor
                        .Distances()
                        .Subscribe(distance =>
                        {
                            _logger.LogDebug($"Measured distance of {Math.Round(distance)}cm");
                            _connection.SendAsync("DistanceMeasured", distance);
                        });
                    _activeDistanceMeasurements[sensorName] = disposable;
                }
            });

            _connection.On<string>("DeactivateDistanceMeasurement", sensorName =>
            {
                _logger.LogDebug($"Deactivating distance measurement of {sensorName}");
                if (_activeDistanceMeasurements.TryGetValue(sensorName, out var disposable))
                {
                    disposable.Dispose();
                }
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
            foreach (var motor in _configuration.Motors.Values)
            {
                motor.Dispose();
            }
            foreach (var servo in _configuration.Servos.Values)
            {
                servo.Dispose();
            }
            foreach (var light in _configuration.Lights.Values)
            {
                light.Dispose();
            }
            foreach (var distance in _configuration.DistanceSensors.Values)
            {
                distance.Dispose();
            }
            _cameraDisposable?.Dispose();
            return Task.CompletedTask;
        }
    }
}
