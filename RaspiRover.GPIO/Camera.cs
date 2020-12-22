using RaspiRover.GPIO.Contracts;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Camera;

namespace RaspiRover.GPIO
{
    public class Camera : ICamera
    {
        public Task<byte[]> TakeImage()
        {
            var image = Pi.Camera.CaptureImageAsync(new CameraStillSettings
            {
                CaptureWidth = 640,
                CaptureHeight = 480,
                CaptureJpegQuality = 92,
                CaptureTimeoutMilliseconds = 300
            });

            return image;
        }
    }
}
