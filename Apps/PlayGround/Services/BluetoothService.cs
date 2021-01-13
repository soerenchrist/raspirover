using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Base2Base.Abstractions.Connectivity.Bluetooth;
using Plugin.Bluetooth;
using Xamarin.Essentials;

namespace PlayGround.Services
{
    public class BluetoothService : IBluetoothService
    {
        private static IBluetoothService? _current;
        public static IBluetoothService Current => _current ??= new BluetoothService();

        private BluetoothService()
        {
            
        }
        public async Task<bool> CheckPermissionGranted()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
                return true;
            var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return result == PermissionStatus.Granted;
        }
        
        public IEnumerable<BluetoothDevice> GetBondedDevices()
        {
            return CrossBluetooth.Current.BondedDevices;
        }

        public async Task<bool> ConnectToDevice(string address)
        {
            try
            {
                await CrossBluetooth.Current.ConnectToDeviceAsync(address);
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }
        }

        public async Task SendData(byte value)
        {
            await CrossBluetooth.Current.WriteToDeviceAsync(new []{value});
        }
    }
}