using System.Device.Gpio;
using Microsoft.Extensions.Configuration;
using RaspiRover.GPIO;
using RaspiRover.GPIO.Contracts;

namespace RaspiRover.Server
{
    public static class CompositionRoot
    {
        public static readonly IDriveMotor DriveMotor = new DriveMotor(CreateConfiguration);
        public static readonly ISteerMotor SteerMotor = new SteerMotor(CreateConfiguration);

        public static readonly ILight BackLight = new Light(22);
        private static IConfiguration CreateConfiguration => new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }
}