using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntonPushkin.BlueToothTest.Services;

namespace AntonPushkin.BlueToothTest
{
    public class BluetoothDevicesManagerService : IBluetoothDevicesManagerService
    {
        public async Task<bool> AsyncCheckThatDevicesIsAvailable(IEnumerable<string> devices)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> AsyncGetListOfPairedDevices()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> AsyncGetListOfUnPairedDevices()
        {
            throw new NotImplementedException();
        }
    }
}
