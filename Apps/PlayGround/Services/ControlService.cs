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

        private readonly BehaviorSubject<byte[]?> _imageSubject = new(null);
        
        public IObservable<byte[]?> LastImage => _imageSubject.AsObservable();

        private ControlService(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
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
                var value = Map(speed, -100, 100, 0, 99);
                await _bluetoothService.SendData((byte)value);
            }
            catch (Exception) { }
        }

        public async Task SetSteerPosition(int steerPosition)
        {
            try
            {
                var value = Map(steerPosition, -10, 10, 101, 163);
                await _bluetoothService.SendData((byte)value);
            }
            catch (Exception)
            {
            }
        }
        private int Map(int x, int inMin, int inMax, int outMin, int outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
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
