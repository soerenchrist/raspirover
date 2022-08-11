using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using PlayGround.Services;
using PlayGround.UWP.Native;
using Xamarin.Forms;
using Device = PlayGround.Models.Device;

[assembly: Dependency(typeof(BluetoothService))]
namespace PlayGround.UWP.Native
{
    public class BluetoothService : IBluetoothService
    {
        public IEnumerable<Device> GetBondedDevices()
        {
            var services = Windows.Devices.Enumeration.DeviceInformation
                .FindAllAsync(
                    RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort))
                .GetAwaiter().GetResult();

            foreach (var service in services)
            {
                
            }

            return new List<Device>();
        }

        public Task<bool> CheckPermissionGranted()
        {
            return Task.FromResult(true);
        }

        public Task<bool> ConnectToDevice(string address)
        {
            throw new System.NotImplementedException();
        }

        public Task SendData(byte value)
        {
            throw new System.NotImplementedException();
        }

        public Task Disconnect()
        {
            throw new System.NotImplementedException();
        }
    }
}