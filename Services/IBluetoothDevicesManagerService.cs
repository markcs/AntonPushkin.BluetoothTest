using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AntonPushkin.BlueToothTest
{
    public interface IBluetoothDevicesManagerService
    {
        IList<BluetoothDevice> GetListOfPairedDevices();

        IList<BluetoothDevice> GetListOfNotConnectedDevices();
    }
}