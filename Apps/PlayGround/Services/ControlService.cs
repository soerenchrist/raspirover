using Microsoft.AspNetCore.SignalR.Client;
using PlayGround.Util;
using RaspiRover.Communication;
using System;
using System.Reactive.Disposables;
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

            _connection.On<byte[]>(Methods.ImageTaken, image =>
            {
                _imageSubject.OnNext(image);
            });

            try
            {
                await _connection.StartAsync();
                _connectedSubject.OnNext(true);
            }
            catch (Exception)
            {
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
                await _connection.SendAsync(Methods.TakeImage);
            }
            catch (Exception) { }
        }

        public async Task StartVideo()
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                int interval = Preferences.Get(PreferenceKeys.VideoFrameRate, 500);
                await _connection.SendAsync(Methods.StartVideo, interval);
            }
            catch (Exception) { }
        }

        public async Task StopVideo()
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync(Methods.StopVideo);
            }
            catch (Exception) { }
        }

        public async Task SetSpeed(int speed)
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync(Methods.SetSpeed, speed);
            }
            catch (Exception) { }
        }

        public async Task SetSteerPosition(int steerPosition)
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync(Methods.SetSteerPosition, steerPosition);
            }
            catch (Exception)
            {
            }
        }

        public IObservable<double> MeasureDistance()
        {
            if (_connection == null || _connection.State != HubConnectionState.Connected)
                return Observable.Return(0.0);

            return Observable.Create<double>(observer =>
            {
                _connection.SendAsync(Methods.ActivateDistanceMeasurement);
                _connection.On<double>(Methods.DistanceMeasured, distance =>
                {
                    observer.OnNext(distance);
                });

                return Disposable.Create(() => _connection.SendAsync(Methods.DeactivateDistanceMeasurement));
            });
        }
    }
}
