using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntonPushkin.BlueToothTest;
using InTheHand.Net.Bluetooth;

namespace AntonPushkin.BlueToothTest
{
    public class BluetoothDevicesManagerService : IBluetoothDevicesManagerService
    {
        public IList<BluetoothDevice> AsyncGetListOfPairedDevices()
        {
            var btClient = new InTheHand.Net.Sockets.BluetoothClient();
            InTheHand.Net.Sockets.BluetoothDeviceInfo[] array = btClient.DiscoverDevices();
            var devices = (IList<BluetoothDevice>) array.Select(a => new BluetoothDevice(a)).ToList();
            return devices;
        }
    }
}
