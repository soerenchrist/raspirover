using System.Collections.Generic;
using System.Threading.Tasks;
using Base2Base.Abstractions.Connectivity.Bluetooth;
using PlayGround.Models;

namespace PlayGround.Services
{
    public interface IBluetoothService
    {
        IEnumerable<Device> GetBondedDevices();
        Task<bool> CheckPermissionGranted();
        Task<bool> ConnectToDevice(string address);
        Task SendData(byte value);
        Task Disconnect();
    }
}