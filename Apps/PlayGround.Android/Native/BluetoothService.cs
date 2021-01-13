using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Bluetooth;
using Java.Util;
using PlayGround.Android.Native;
using PlayGround.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Device = PlayGround.Models.Device;

[assembly: Dependency(typeof(BluetoothService))]
namespace PlayGround.Android.Native
{
    public class BluetoothService : IBluetoothService
    {
        private List<BluetoothDevice> _devices = new List<BluetoothDevice>();
        private readonly BluetoothAdapter _adapter;
        private BluetoothSocket? _socket;
        private static UUID? BtModuleUuid => UUID.FromString("00001101-0000-1000-8000-00805F9B34FB"); 

        public BluetoothService()
        {
            _adapter = BluetoothAdapter.DefaultAdapter ?? throw new InvalidOperationException();
        }
        
        public IEnumerable<Device> GetBondedDevices()
        {
            if (_adapter.BondedDevices == null)
                throw new InvalidOperationException();
            _devices = _adapter.BondedDevices.ToList();
            return _adapter.BondedDevices.Select(x => new Device(x.Address ?? string.Empty, x.Name ?? string.Empty));
        }

        public async Task<bool> CheckPermissionGranted()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
                return true;
            var result = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            return result == PermissionStatus.Granted;
        }

        public async Task<bool> ConnectToDevice(string address)
        {
            var device = _devices.FirstOrDefault(x => x.Address == address);
            if (device == null)
                throw new InvalidOperationException($"Device {address} is not bonded");
            
            try
            {
                _socket = CreateSocket(device);
                
            }
            catch (Exception e)
            {
                return false;
            }

            try
            {
                if (_socket != null)
                    await _socket.ConnectAsync();
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private BluetoothSocket? CreateSocket(BluetoothDevice device)
        {
            return device.CreateRfcommSocketToServiceRecord(BtModuleUuid);
        }

        public async Task SendData(byte value)
        {
            if (_socket == null || !_socket.IsConnected)
                throw new InvalidOperationException("Not connected");

            var stream = _socket.OutputStream;
            if (stream == null)
                throw new InvalidOperationException("No output stream");
            await stream.WriteAsync(new[] {value}, 0, 1);
        }

        public Task Disconnect()
        {
            if (_socket == null || !_socket.IsConnected)
                return Task.CompletedTask;
            
            _socket.Dispose();
            _socket = null;

            return Task.CompletedTask;
        }
    }
}