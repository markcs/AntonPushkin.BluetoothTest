using AntonPushkin.BlueToothTest.Services;
using GalaSoft.MvvmLight;

namespace AntonPushkin.BlueToothTest
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ShellViewModel : ViewModelBase
    {
        private IBluetoothDevicesManagerService bluetoothManagerService;
        
        /// <summary>
        /// Initializes a new instance of the ShellViewModel class.
        /// </summary>
        public ShellViewModel(IBluetoothDevicesManagerService bluetoothManagerService)
        {
            this.bluetoothManagerService = bluetoothManagerService;
        }
    }
}