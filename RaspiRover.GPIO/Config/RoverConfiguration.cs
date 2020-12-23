using System.Collections.Generic;

namespace RaspiRover.GPIO.Config
{
    public class RoverConfiguration
    {
        public Dictionary<string, DriveMotor>? Motors { get; init; }
        public Dictionary<string, SteerMotor>? Servos { get; init; }
        public Dictionary<string, DistanceSensor>? DistanceSensors { get; init; }
        public Dictionary<string, Light>? Lights { get; init; }
        public CameraConfiguration? Camera { get; init; }
    }

    public class CameraConfiguration
    {
        public bool Enabled { get; init; }
    }
}
