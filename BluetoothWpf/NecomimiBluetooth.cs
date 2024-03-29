﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BluetoothWpf
{
    public class NecomimiBluetooth : INotifyPropertyChanged
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
        const string _pinCode = "666";

        private NecomimiReceiver necomimiReceiver;
       
        LbLoger _LbLoger;

        public NecomimiBluetooth(ref LbLoger lbLoger)
        {
            _bluetoothClientAddr = new BluetoothAddress(0);
            _btEndpoint = new BluetoothEndPoint(_bluetoothClientAddr, BluetoothService.SerialPort);
             _localClient = new BluetoothClient(_btEndpoint);
            _localComponent = new BluetoothComponent(_localClient);

            _localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesProgressCallback);
            _localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesCompleteCompleteCallback);

            //_connectTask = new Task()
            _LbLoger = lbLoger;
            necomimiReceiver = new NecomimiReceiver(ref lbLoger);
        }

        ~NecomimiBluetooth()
        {
            if(_localClient != null)
            {
                _localClient.Client.Disconnect(false);
            }
        }

        public int GetNLastParsedPacketsFromQueue(int requestedAmountOfPacks, ref List<NecomimimPacket> necomimimPacketsList)
        {
            int amountOfDequedPacks = 0;
            if (necomimiReceiver.ParsedPacketsQueue != null)
            {
                for (int i = 0; i < requestedAmountOfPacks; i++)
                {
                    if(necomimiReceiver.ParsedPacketsQueue.Count > 0)
                    {
                        // TODO: проверка на null
                        NecomimimPacket necomimimPacketToDeque;

                        if (necomimiReceiver.ParsedPacketsQueue.TryDequeue(out necomimimPacketToDeque))
                        {
                            amountOfDequedPacks++;
                            necomimimPacketsList.Add(necomimimPacketToDeque);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            return amountOfDequedPacks;
        }

        private bool _isNecomimiConnected = false;

        private bool _isConnecting = false;
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

        private void StartAutoConnect()
        {
            if(IsConnected())
            {
                //_localClient.Close();
            }
            FindNecomimiDevice();
        }
        private void DiscoverDevicesCompleteCompleteCallback(object sender, DiscoverDevicesEventArgs e)
        {
            if (_necomimmiDevice == null)
            {
                _localComponent.DiscoverDevicesAsync(10, true, true, true, true, null);
                _LbLoger.Print($"Энецефалограф не найден продолжаем сканирование");
                return;
            }
            else
            {
                _LbLoger.Print($"Энецефалограф найден {_necomimmiDevice.DeviceName}, сканирование остановлено");
                // todo: нужно сделать асинхронный запрос на коннект.
                if(PairNecomimiDevice())
                {
                    ConnectToNecomimi();
                }    


            }
        }
        
        private void FindNecomimiDevice()
        {
            _LbLoger.Print("Начат поиск энцефалографа");
            _localComponent.DiscoverDevicesAsync(10, true, true, true, true, null);
        }



        private bool PairNecomimiDevice()
        {
            
            if (_necomimmiDevice == null)
            {
                _LbLoger.Print("Сначала нужно найти энцефалограф!!!!");
                return false;
            }
            BluetoothDeviceInfo[] pairedDevicesList = _localClient.DiscoverDevices(255, false, true, false, false);

            bool isPaired = false;
            foreach (BluetoothDeviceInfo device in pairedDevicesList)
            {
                if (device.Equals(_necomimmiDevice))
                {
                    isPaired = true;
                    break;
                }
            }

            // if the device is not paired, pair it!
            if (!isPaired)
            {
                // replace DEVICE_PIN here, synchronous method, but fast
                isPaired = BluetoothSecurity.PairRequest(_necomimmiDevice.DeviceAddress, _pinCode);
                if (isPaired)
                {
                    isPaired = true;
                    _LbLoger.Print("Сопряжение с энцефалографом прошло успешно");
                    // now it is paired
                }
                else
                {
                    isPaired = false;
                    _LbLoger.Print("Сопряжение с энцефалографом не удалось");
                    // pairing failed
                }
            }
            else
            {
                _LbLoger.Print("Было сопряжено ранее");
            }
            return isPaired;
        }

        private void BtConnect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                _LbLoger.Print("Энцефалограф подключен");
                bool lcConnected = _localClient.Connected;
                bool dataAvailable = _localClient.GetStream().DataAvailable;

                if (_localClient.Connected)
                {
                    if(isConnectionWasEstablishedBefore)
                    {
                        var btStream = _localClient.GetStream();

                        btStream.Flush();
                        necomimiReceiver.StartReceiving(ref btStream);
                    }
                    _isNecomimiConnected = true;
                }
                else
                {
                    _LbLoger.Print($"То самое место {lcConnected}:{dataAvailable}");
                }
            }
            else
            {
                _LbLoger.Print("Подключение не удалось, пробуем ещё раз");
                _isNecomimiConnected = false;
            }
            _isConnecting = false;
        }

        private void ConnectToNecomimi()
        {
            try
            {
                necomimiReceiver.ReadingAllowed = true;
                if (_necomimmiDevice != null)
                {
                    // set pin of device to connect with
                    _localClient.SetPin(_pinCode);
                    // async connection method
                    _necomimmiDevice.Refresh();
                    if (!_necomimmiDevice.Connected)
                    {
                        _localClient.BeginConnect(_necomimmiDevice.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(BtConnect), _necomimmiDevice);
                    }
                    else
                    {
                        _LbLoger.Print("ЭЭГ уже подключен");
                    }
                }
            }
            catch (SocketException sEx)
            {
                if (sEx.SocketErrorCode == SocketError.IsConnected)
                {
                    necomimiReceiver.ReadingAllowed = true;
                }
            }
        }

        public void Receive()
        {
            if (_localClient.Connected)
            {
                var btStream = _localClient.GetStream();

                btStream.Flush();
                necomimiReceiver.StartReceiving(ref btStream);
            }
            else
            {
                _LbLoger.Print("Не получилось принять данные, т.к. подключение разорвалось");
            }
        }

        private bool IsConnected()
        {
            if(_necomimmiDevice != null)
            {
                _necomimmiDevice.Refresh();
                if (_necomimmiDevice.Connected)
                    return true;
                else 
                    return false;
            }
            return false;
        }


        bool isConnectionWasEstablishedBefore = false;
        //Необходимо проверять с каким-то периодом не порвалось ли подключение
        public bool ControlNecomomiDeviceConnection()
        {
            if(!IsConnected())
            {
                if (!_isConnecting)
                {
                    _necomimmiDevice = null;
                    IsNecomimiConnected = false;
                    IsNecomimiPaired = false;
                    IsNecomimiFound = false;
                    necomimiReceiver.StopReceiving();

                    if (isConnectionWasEstablishedBefore)
                    {
                        _localClient.Close();
                        _localComponent.Dispose();
                        // TODO: чет совсем уже 
                        _bluetoothClientAddr = new BluetoothAddress(0);
                        _btEndpoint = new BluetoothEndPoint(_bluetoothClientAddr, BluetoothService.SerialPort);
                        _localClient = new BluetoothClient(_btEndpoint);
                        _localComponent = new BluetoothComponent(_localClient);
                        
                        _localComponent.DiscoverDevicesProgress -= DiscoverDevicesProgressCallback;
                        _localComponent.DiscoverDevicesComplete -= DiscoverDevicesCompleteCompleteCallback;
                        _localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesProgressCallback);
                        _localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(DiscoverDevicesCompleteCompleteCallback);

                    }
                    isConnectionWasEstablishedBefore = true;
                    
                    _LbLoger.Print("Control->Энцефалограф не подключен, подключаем");
                    StartAutoConnect();
                    //TODO: Нужно сбросить все флаги ещё.
                    OnPropertyChanged("ControlNecomomiDeviceConnection");
                }
                _isConnecting = true;
            }
            return true;
        }


        ///////////////////////////////////////////////////////////////////////
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
