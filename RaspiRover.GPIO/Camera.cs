using RaspiRover.GPIO.Contracts;
using System;
using System.Reactive.Linq;
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
                CaptureWidth = 320,
                CaptureHeight = 240,
                CaptureJpegQuality = 50,
                CaptureDisplayPreview = false,
                CaptureTimeoutMilliseconds = 300
            });

            return image;
        }

        public IObservable<byte[]> StartVideoStream()
        {
            return Observable.Interval(TimeSpan.FromSeconds(.5))
                .Select(x => Pi.Camera.CaptureImage(new CameraStillSettings
                {
                    CaptureWidth = 320,
                    CaptureHeight = 240,
                    CaptureJpegQuality = 50,
                    CaptureDisplayPreview = false,
                    CaptureTimeoutMilliseconds = 300
                }));
        }
    }
}
