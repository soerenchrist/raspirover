using System;
using System.Threading;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using RaspiRover.GPIO;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace RaspiRover
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.100.142:5000/control")
                .Build();

            connection.StartAsync();

            for (int i = -100; i < 100; i++)
            {
                connection.SendAsync("SetSteerPosition", i);
                Thread.Sleep(100);
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