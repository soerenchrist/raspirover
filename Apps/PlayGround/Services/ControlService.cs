using Microsoft.AspNetCore.SignalR.Client;
using PlayGround.Util;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace PlayGround.Services
{
    public class ControlService : IControlService
    {
        private static IControlService? _instance;
        public static IControlService Current => _instance ??= new ControlService();

        private readonly BehaviorSubject<bool> _connectedSubject = new(false);
        private readonly BehaviorSubject<byte[]?> _imageSubject = new(null);
        private HubConnection? _connection;


        public IObservable<byte[]?> LastImage => _imageSubject.AsObservable();
        public IObservable<bool> Connected => _connectedSubject.AsObservable();

        private ControlService()
        {

        }

        public async Task Connect()
        {
            var url = Preferences.Get(PreferenceKeys.Server, "http://192.168.100.142:5000/control");
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();

            _connection.Closed += ConnectionOnClosed;
            _connection.Reconnected += ConnectionOnReconnected;

            _connection.On<byte[]>("ImageTaken", image =>
            {
                _imageSubject.OnNext(image);
            });

            try
            {
                await _connection.StartAsync();
                _connectedSubject.OnNext(true);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                _connectedSubject.OnNext(false);
            }
        }

        public async Task Disconnect()
        {
            if (_connection == null)
                return;
            _connection.Closed -= ConnectionOnClosed;
            _connection.Reconnected -= ConnectionOnReconnected;
            await _connection.StopAsync();
        }

        private Task ConnectionOnReconnected(string arg)
        {
            _connectedSubject.OnNext(true);
            return Task.CompletedTask;
        }

        private Task ConnectionOnClosed(Exception arg)
        {
            _connectedSubject.OnNext(false);
            return Task.CompletedTask;
        }

        public async Task TakeImage()
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync("TakeImage");
            }
            catch (Exception) { }
        }

        public async Task SetSpeed(int speed)
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync("SetSpeed", speed);
            }
            catch (Exception) { }
        }

        public async Task SetSteerPosition(int steerPosition)
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync("SetSteerPosition", steerPosition);
            }
            catch (Exception)
            {
            }
        }
    }
}
