using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;


namespace BluetoothWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //todo:
        //Вывнести в класс LbLoger, при вынесении класса Bluetooth
        private string GetDateWithLogFormat()
        {
            string logString = String.Format("[{0:s}]: ", DateTime.Now.ToString());
            return logString;
        }

        public void Print(string message)
        {
            string logString = GetDateWithLogFormat() + message;
            listBox1_btDevices.Items.Add(logString);
            listBox1_btDevices.ItemsSource = listBox1_btDevices.ItemsSource;
        }
        static BluetoothAddress bluetoothClientAddr = new BluetoothAddress(0);

        static BluetoothEndPoint btEndpoint = new BluetoothEndPoint(bluetoothClientAddr, BluetoothService.SerialPort);
        // client is used to manage connections
        static BluetoothClient localClient = new BluetoothClient(btEndpoint);
        // component is used to manage device discovery
        BluetoothComponent localComponent = new BluetoothComponent(localClient);
        // async methods, can be done synchronously too

        List<BluetoothDeviceInfo> bluetoothDeviceInfos = new List<BluetoothDeviceInfo>();

        BluetoothDeviceInfo necomimmiDevice;
        void  FindNecomimmiDevice()
        {
            
            throw new NotImplementedException();
        }
        public MainWindow()
        {
            localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);
            InitializeComponent();
        }
        const long necomimmiDeviceAddressLong = 0x98D332312290;
        //const BluetoothAddress necomimmiDeviceAddress = 0x98D332312290;

        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            try
            {
                // log and save all found devices
                for (int i = 0; i < e.Devices.Length; i++)
                {
                    var btDevice = e.Devices[i];
                    Print($"FOUND: {btDevice.DeviceName} {{{btDevice.DeviceAddress}}}");
                    bluetoothDeviceInfos.Add(e.Devices[i]);
                    if (e.Devices[i].DeviceAddress.ToInt64() == necomimmiDeviceAddressLong)
                    {
                        necomimmiDevice = e.Devices[i];
                    }

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Что-то не так: " + ex.ToString());
            }
        }

        private void component_DiscoverDevicesComplete(object sender, DiscoverDevicesEventArgs e)
        {
            if (necomimmiDevice == null)
            {
                localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);

                return;
            }
            else 
            {
                Print("Necomimmi device is found " + necomimmiDevice.DeviceName);
                
            }

            
            // log some stuff
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Print("Bt scan initiated");
            localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
           // listBox1_btDevices.Items.Clear(); //DeviceAddress = { 98D332312290}
            
        }

        const string pinCode = "666";
        private void button_pair_Click(object sender, RoutedEventArgs e)
        {
            if(necomimmiDevice == null)
            {
                Print("Сначала нужно найти энцефалограф!!!!");
                return;
            }
            BluetoothDeviceInfo[] pairedDevicesList = localClient.DiscoverDevices(255, false, true, false, false);


                bool isPaired = false;
                foreach (BluetoothDeviceInfo device in pairedDevicesList)
                {
                    
                    for (int i = 0; i < pairedDevicesList.Length; i++)
                    {
                        if (device.Equals(necomimmiDevice))
                        {
                            isPaired = true;
                            break;
                        }
                    }
                }

                // if the device is not paired, pair it!
                if (!isPaired)
                {
                    // replace DEVICE_PIN here, synchronous method, but fast
                    isPaired = BluetoothSecurity.PairRequest(necomimmiDevice.DeviceAddress, pinCode);
                    if (isPaired)
                    {
                        Print("Сопряжено");
                        // now it is paired
                    }
                    else
                    {
                        Print("Не сопряжено");
                        // pairing failed
                    }
                }
                else
                {
                    Print("Было сопряжено ранее");
                }
        }

        private void Connect(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                MessageBox.Show("Test");
                // client is connected now :)
            }
        }
        private void button_connect_Click(object sender, RoutedEventArgs e)
        {
            if (necomimmiDevice != null && necomimmiDevice.Authenticated)
            {
                // set pin of device to connect with
                localClient.SetPin(pinCode);
                // async connection method
                localClient.BeginConnect(necomimmiDevice.DeviceAddress, BluetoothService.SerialPort, new AsyncCallback(Connect), necomimmiDevice);
            }
        }
    }
}
