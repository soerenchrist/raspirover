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
        private ControlService(IBluetoothService bluetoothService)
        {
            _bluetoothService = bluetoothService;
        }

        public async Task Disconnect()
        {
            try
            {
                await _bluetoothService.Disconnect();
            }
            catch (Exception)
            {
                // ignored
            }
        }
        
        public async Task SetSpeed(int speed)
        {
            try
            {
                var value = Map(speed, -100, 100, 0, 99);
                await _bluetoothService.SendData((byte)value);
            }
            catch (Exception)
            {
                // ignored
            }
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
                // ignored
            }
        }
        private int Map(int x, int inMin, int inMax, int outMin, int outMax)
        {
            return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }

        public IObservable<double> MeasureDistance()
        {
            return Observable.Return(0.0);
        }

        public async Task SetFrontLight(bool value)
        {
          
           try
           {
               await _bluetoothService.SendData(value ? 255 : 254);
           }
           catch (Exception)
           {
               // ignored
           }
        }
    }
}
