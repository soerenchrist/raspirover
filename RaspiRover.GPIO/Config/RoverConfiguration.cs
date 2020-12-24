using System.Collections.Generic;

namespace RaspiRover.GPIO.Config
{
    public class RoverConfiguration
    {
        public Dictionary<string, DriveMotor> Motors { get; init; } = new Dictionary<string, DriveMotor>();
        public Dictionary<string, SteerMotor> Servos { get; init; } = new Dictionary<string, SteerMotor>();

        public Dictionary<string, DistanceSensor> DistanceSensors { get; init; } =
            new Dictionary<string, DistanceSensor>();
        public Dictionary<string, Light> Lights { get; init; } = new Dictionary<string, Light>();
        public Camera? Camera { get; init; }
    }

}
