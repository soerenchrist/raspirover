using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover
{
    class Program
    {
        static void Main(string[] args)
        {
            Pi.Init<BootstrapWiringPi>();

            var gpio = (GpioPin)Pi.Gpio[26];

            gpio.PinMode = GpioPinDriveMode.Output;
            gpio.StartSoftPwm(0, 100);

            while (true)
            {
                var input = Console.ReadLine();
                var steer = int.Parse(input);

                gpio.SoftPwmValue = steer;
            }
            /*
            var controller = new GpioController();
            var driveMotor = new DriveMotor(controller, configurationRoot);

            controller.Write(17, PinValue.Low);
            controller.Write(27, PinValue.Low);

            await Task.Delay(3000);
            
            */
        }
    }
}
