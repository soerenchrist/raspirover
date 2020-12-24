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
        public int CaptureWidth { get; init; }
        public int CaptureHeight { get; init; }
        public int JpgQuality { get; init; }

        public Task<byte[]> TakeImage()
        {
            var image = Pi.Camera.CaptureImageAsync(new CameraStillSettings
            {
                CaptureWidth = CaptureWidth,
                CaptureHeight = CaptureHeight,
                CaptureJpegQuality = JpgQuality,
                CaptureDisplayPreview = false,
                CaptureTimeoutMilliseconds = 300,
            });

            return image;
        }

        public IObservable<byte[]> StartVideoStream(TimeSpan interval)
        {
            return Observable.Interval(interval)
                .Select(x => Pi.Camera.CaptureImage(new CameraStillSettings
                {
                    CaptureWidth = CaptureWidth,
                    CaptureHeight = CaptureHeight,
                    CaptureJpegQuality = JpgQuality,
                    CaptureDisplayPreview = false,
                    CaptureTimeoutMilliseconds = 300
                }));
        }
    }
}
