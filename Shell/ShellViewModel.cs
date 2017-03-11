using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

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

        public ObservableCollection<BluetoothDevice> PairedDevices { get; set; }
        public ObservableCollection<BluetoothDevice> TriggerDevices { get; set; }

        private bool isDeviceListBusy = true;
        public bool IsDeviceListBusy
        {
            get { return isDeviceListBusy; }
            set
            {
                if (value != isDeviceListBusy)
                {
                    isDeviceListBusy = value;
                    RaisePropertyChanged(()=>IsDeviceListBusy);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ShellViewModel class.
        /// </summary>
        public ShellViewModel(IBluetoothDevicesManagerService bluetoothManagerService)
        {
            this.bluetoothManagerService = bluetoothManagerService;

            this.PairedDevices = new ObservableCollection<BluetoothDevice>();
            this.TriggerDevices = new ObservableCollection<BluetoothDevice>();

            RefreshCommand = new RelayCommand(LoadDevices, ()=>!IsDeviceListBusy);

            // Load devices
            LoadDevices();
        }

        public ICommand RefreshCommand { get; set; }

        private void LoadDevices()
        {
            IsDeviceListBusy = true;
            PairedDevices.Clear();
            Task.Run(() => bluetoothManagerService.AsyncGetListOfPairedDevices())
            .ContinueWith((t) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    IsDeviceListBusy = false;
                    foreach (var device in t.Result)
                        PairedDevices.Add(device);
                });
            });
        }
    }
}