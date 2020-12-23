using Microsoft.Extensions.Configuration;
using RaspiRover.GPIO;
using RaspiRover.GPIO.Contracts;
using System;

namespace RaspiRover.Server
{
    public sealed class CompositionRoot
    {
        private readonly IConfiguration _configuration;
        private readonly Lazy<IDriveMotor> _driveMotor;
        private readonly Lazy<ISteerMotor> _steerMotor;
        private readonly Lazy<ICamera> _camera;
        private readonly Lazy<INearbySensor> _nearbySensor;
        private readonly Lazy<ILight> _backlight;

        public IDriveMotor DriveMotor => _driveMotor.Value;
        public ISteerMotor SteerMotor => _steerMotor.Value;
        public ICamera Camera => _camera.Value;
        public INearbySensor NearbySensor => _nearbySensor.Value;
        public ILight BackLight => _backlight.Value;

        public CompositionRoot(IConfiguration configuration)
        {
            _configuration = configuration;
            _driveMotor = new Lazy<IDriveMotor>(CreateDriveMotor);
            _steerMotor = new Lazy<ISteerMotor>(CreateSteerMotor);
            _camera = new Lazy<ICamera>(CreateCamera);
            _nearbySensor = new Lazy<INearbySensor>(CreateNearbySensor);
            _backlight = new Lazy<ILight>(CreateBackLight);
        }

        private IDriveMotor CreateDriveMotor => new DriveMotor(_configuration);
        private ISteerMotor CreateSteerMotor => new SteerMotor(_configuration);
        private static ICamera CreateCamera => new Camera();
        private static INearbySensor CreateNearbySensor => new NearbySensor(1, 2);
        private static ILight CreateBackLight => new Light(22);
    }
}
