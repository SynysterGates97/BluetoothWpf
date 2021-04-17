using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BluetoothWpf
{
    class NecomimiBluetooth : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private const int _rxBufferSize = 1024;

        private byte[] _rxBuffer = new byte[_rxBufferSize];

        private BluetoothAddress _bluetoothClientAddr;

        private BluetoothEndPoint _btEndpoint;

        // client is used to manage connections
        private BluetoothClient _localClient;
        // component is used to manage device discovery
        private BluetoothComponent _localComponent;

        private BluetoothDeviceInfo _necomimmiDevice;
        const long _necomimmiDeviceAddressLong = 0x98D332312290;

        LbLoger _LbLoger;

        public NecomimiBluetooth(ref LbLoger lbLoger)
        {
            _bluetoothClientAddr = new BluetoothAddress(0);
            _btEndpoint = new BluetoothEndPoint(_bluetoothClientAddr, BluetoothService.SerialPort);
             _localClient = new BluetoothClient(_btEndpoint);
            _localComponent = new BluetoothComponent(_localClient);

            _localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesProgressCallback);
            _localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesCompleteCompleteCallback);

            _LbLoger = lbLoger;
        }

        private bool _isNecomimiConnected = false;
        public bool IsNecomimiConnected 
        { 
            get
            {
                return _isNecomimiConnected;
            }
            set
            {
                OnPropertyChanged("IsNecomimiConnected");
            }
        }

        private bool _isNecomimiPaired = false;
        public bool IsNecomimiPaired
        {
            get
            {
                return _isNecomimiPaired;
            }
            set
            {
                OnPropertyChanged("IsNecomimiPaired");
            }
        }

        private bool _isNecomimiFound = false;
        public bool IsNecomimiFound
        {
            get
            {
                return _isNecomimiFound;
            }
            set
            {
                OnPropertyChanged("IsNecomimiFound");
            }
        }

        private void DiscoverDevicesProgressCallback(object sender, DiscoverDevicesEventArgs e)
        {
            try
            {
                // log and save all found devices
                for (int i = 0; i < e.Devices.Length; i++)
                {
                    var btDevice = e.Devices[i];
                    _LbLoger.Print($"FOUND: {btDevice.DeviceName} {{{btDevice.DeviceAddress}}}");
                    if (e.Devices[i].DeviceAddress.ToInt64() == _necomimmiDeviceAddressLong)
                    {
                        _necomimmiDevice = e.Devices[i];
                        IsNecomimiFound = true;
                    }

                }
            }
            catch (Exception ex)
            {
                _LbLoger.Print($"Что-то не так: {ex.ToString()}");
            }
        }

        private void DiscoverDevicesCompleteCompleteCallback(object sender, DiscoverDevicesEventArgs e)
        {
            if (_necomimmiDevice == null)
            {
                _localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);

                return;
            }
            else
            {
                _LbLoger.Print($"Энецефалограф найден {_necomimmiDevice.DeviceName}, сканирование остановлено");

            }
        }
        
        public void FindNecomimiDevice()
        {
            _LbLoger.Print("Начат поиск энцефалографа");
            _localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
        }

        public void PairNecomimiDevice()
        {

            throw new NotImplementedException();
        }

        public void Connect()
        {

            throw new NotImplementedException();
        }

        public void Receive()
        {

            throw new NotImplementedException();
        }

        //Необходимо проверять с каким-то периодом не порвалось ли подключение
        public void ControlNecomomiDeviceConnection()
        {
            if()
            throw new NotImplementedException();
        }


        ///////////////////////////////////////////////////////////////////////
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
