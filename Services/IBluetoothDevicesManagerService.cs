using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntonPushkin.BlueToothTest.Services
{
    public interface IBluetoothDevicesManagerService
    {
        Task<IEnumerable<string>> AsyncGetListOfPairedDevices();

        Task<IEnumerable<string>> AsyncGetListOfUnPairedDevices();

        Task<bool> AsyncCheckThatDevicesIsAvailable(IEnumerable<string> devices);
    }
}