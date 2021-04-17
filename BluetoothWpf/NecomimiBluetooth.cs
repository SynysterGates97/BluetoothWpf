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

        BluetoothDeviceInfo necomimmiDevice;

        LbLoger _LbLoger;


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




        public NecomimiBluetooth(ref LbLoger lbLoger)
        {
            _bluetoothClientAddr = new BluetoothAddress(0);
            _btEndpoint = new BluetoothEndPoint(_bluetoothClientAddr, BluetoothService.SerialPort);s
            _localClient = new BluetoothClient(_btEndpoint);
            _localComponent = new BluetoothComponent(_localClient);

            _LbLoger = lbLoger;
        }

        
        public void FindNecomimiDevice()
        {

            throw new NotImplementedException();
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
