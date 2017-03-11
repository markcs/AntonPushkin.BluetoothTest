using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using AntonPushkin.BlueToothTest.Helpers;
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

        private readonly int timerInterval    = 1;       
        private DispatcherTimer timer;
        private object lockObject = new object();     


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

        private int? countdownTimeForLockWorkstation = null;

        public int? CountdownTimeForLockWorkstation
        {
            get { return countdownTimeForLockWorkstation; }
            set
            {
                if (value != countdownTimeForLockWorkstation)
                {
                    countdownTimeForLockWorkstation = value;
                    RaisePropertyChanged(() => CountdownTimeForLockWorkstation);
                }
            }
        }

        private string statusText = null;

        public string StatusText
        {
            get { return statusText; }
            set
            {
                if (value != statusText)
                {
                    statusText = value;
                    RaisePropertyChanged(() => StatusText);                    
                }
            }
        }


        private BluetoothDevice selectedDevice=null;

        public BluetoothDevice SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                if (value != selectedDevice)
                {
                    selectedDevice = value;
                    RaisePropertyChanged(() => SelectedDevice);
                    UpdateCommandState();
                }
            }
        }

        
        private BluetoothDevice selectedTriggerDevice = null;

        public BluetoothDevice SelectedTriggerDevice
        {
            get { return selectedTriggerDevice; }
            set
            {
                if (value != selectedTriggerDevice)
                {
                    selectedTriggerDevice = value;
                    RaisePropertyChanged(() => SelectedTriggerDevice);
                    UpdateCommandState();
                }
            }
        }
        /// <summary>
        /// Initializes a new instance of the ShellViewModel class.
        /// </summary>
        public ShellViewModel(IBluetoothDevicesManagerService bluetoothManagerService)
        {
            this.bluetoothManagerService = bluetoothManagerService;
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(timerInterval);
            this.timer.Tick += OnTimerTick;
            this.timer.Start();

            this.PairedDevices = new ObservableCollection<BluetoothDevice>();
            this.TriggerDevices = new ObservableCollection<BluetoothDevice>();

            RefreshCommand = new RelayCommand(LoadDevices, ()=>!IsDeviceListBusy);
            AddCommand = new RelayCommand(AddDeviceAsTriger, ()=>SelectedDevice!=null);
            RemoveCommand = new RelayCommand(RemoveDeviceFromTriggers, () => SelectedTriggerDevice != null);
            ResetCountdownCommand = new RelayCommand(ResetCountdown);
            // Load devices
            LoadDevices();
        }

        private void ResetCountdown()
        {
            lock (lockObject)
            {
                CountdownTimeForLockWorkstation = null;
                StatusText = "Lock workstation was canceled";
            }
        }

        private void RemoveDeviceFromTriggers()
        {
            if (SelectedTriggerDevice != null)
            {
                TriggerDevices.Remove(SelectedTriggerDevice);
                UpdateCommandState();
            }
        }

        private void AddDeviceAsTriger()
        {
            if (SelectedDevice != null)
            {
                if (!TriggerDevices.Any(td => td.DeviceName == SelectedDevice.DeviceName))
                    TriggerDevices.Add(SelectedDevice);                                
            }

            UpdateCommandState();
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            lock (lockObject)
            {
                if (CountdownTimeForLockWorkstation != null)
                {
                    if (CountdownTimeForLockWorkstation.Value > 0)
                    {
                        CountdownTimeForLockWorkstation--;
                        StatusText = String.Format("Your workstation will lock after {0} sec. For cancel it press Esc",
                            CountdownTimeForLockWorkstation);
                    }

                    if (CountdownTimeForLockWorkstation.Value == 0)
                    {
                        CountdownTimeForLockWorkstation = null;
                        RundllHelper.LockWorkstation();
                    }
                }
                else
                {
                    if (TriggerDevices.Count == 0)
                        return;

                    StatusText = "Check is all required devices connected...";
                    Task.Run(() => bluetoothManagerService.GetListOfNotConnectedDevices())
                        .ContinueWith((t) =>
                        {
                            System.Windows.Application.Current.Dispatcher.Invoke(() =>
                            {
                                var notConnectedDevices = t.Result;
                                if (
                                    TriggerDevices.Any(
                                        td => notConnectedDevices.Any(dd => dd.DeviceName == td.DeviceName)))
                                    CountdownTimeForLockWorkstation = 10;
                            });
                        });
                }
            }
        }

        public RelayCommand RefreshCommand { get; set; }

        public RelayCommand AddCommand { get; set; }

        public RelayCommand RemoveCommand { get; set; }

        public RelayCommand ResetCountdownCommand { get; set; }


        private void LoadDevices()
        {
            IsDeviceListBusy = true;
            PairedDevices.Clear();
            StatusText = "Discover bluetooth devices...";
            Task.Run(() => bluetoothManagerService.GetListOfPairedDevices())
            .ContinueWith((t) =>
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var device in t.Result)
                        PairedDevices.Add(device);

                    StatusText = String.Format("Loaded {0}", PairedDevices.Count);
                    IsDeviceListBusy = false;
                    UpdateCommandState();
                });
            });
        }

        private void UpdateCommandState()
        {
            RefreshCommand.RaiseCanExecuteChanged();
        }

    }
}