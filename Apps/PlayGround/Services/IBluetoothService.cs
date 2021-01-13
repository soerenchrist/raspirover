using System.Collections.Generic;
using System.Threading.Tasks;
using Base2Base.Abstractions.Connectivity.Bluetooth;

namespace PlayGround.Services
{
    public interface IBluetoothService
    {
        IEnumerable<BluetoothDevice> GetBondedDevices();
        Task<bool> CheckPermissionGranted();
        Task<bool> ConnectToDevice(string address);
        Task SendData(byte value);
    }
}