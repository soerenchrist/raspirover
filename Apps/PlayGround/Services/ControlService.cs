using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PlayGround.Services
{
    public class ControlService : IControlService
    {
        private readonly IBluetoothService _bluetoothService;
        private static IControlService? _instance;
        public static IControlService Current => _instance ??= new ControlService(DependencyService.Get<IBluetoothService>());

        private readonly BehaviorSubject<bool> _connectedSubject = new(false);
        private readonly BehaviorSubject<byte[]?> _imageSubject = new(null);
        
        public IObservable<byte[]?> LastImage => _imageSubject.AsObservable();
        public IObservable<bool> Connected => _connectedSubject.AsObservable();

        private ControlService(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        public async Task Connect()
        {
            
        }

        public async Task Disconnect()
        {
            
        }
        
        public async Task TakeImage()
        {
           /*try
            {
                await _connection.SendAsync("TakeImage");
            }
            catch (Exception) { }*/
        }

        public async Task StartVideo()
        {
           /* if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                int interval = Preferences.Get(PreferenceKeys.VideoFrameRate, 500);
                await _connection.SendAsync("StartVideo", interval);
            }
            catch (Exception) { }*/
        }

        public async Task StopVideo()
        {
          /*  if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync("StopVideo");
            }
            catch (Exception) { }*/
        }

        public async Task SetSpeed(int speed)
        {
            try
            {
                await _bluetoothService.SendData((byte)speed);
            }
            catch (Exception) { }
        }

        public async Task SetSteerPosition(int steerPosition)
        {
            try
            {
                await _bluetoothService.SendData((byte)steerPosition);
            }
            catch (Exception)
            {
            }
        }

        public IObservable<double> MeasureDistance()
        {
            /*if (_connection == null || _connection.State != HubConnectionState.Connected)
                return Observable.Return(0.0);

            return Observable.Create<double>(observer =>
            {
                _connection.SendAsync("ActivateDistanceMeasurement");
                _connection.On<double>("DistanceMeasured", distance =>
                {
                    observer.OnNext(distance);
                });

                return Disposable.Create(() =>
                {
                    _connection.SendAsync("DeactivateDistanceMeasurement");
                });
            });*/
            return Observable.Return(0.0);
        }

        public async Task SetFrontLight(bool value)
        {
           /* if (_connection == null || _connection.State != HubConnectionState.Connected)
                return;
            try
            {
                await _connection.SendAsync("SetLight", "front", value);
            }
            catch (Exception) { }*/
           try
           {
               await _bluetoothService.SendData(value ? 255 : 254);
           }
           catch (Exception e)
           {
           }
        }
    }
}
