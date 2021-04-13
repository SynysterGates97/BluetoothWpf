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
        static BluetoothAddress bluetoothClientAddr = new BluetoothAddress(0);

        static BluetoothEndPoint btEndpoint = new BluetoothEndPoint(bluetoothClientAddr, BluetoothService.SerialPort);
        // client is used to manage connections
        static BluetoothClient localClient = new BluetoothClient(btEndpoint);
        // component is used to manage device discovery
        BluetoothComponent localComponent = new BluetoothComponent(localClient);
        // async methods, can be done synchronously too

        List<BluetoothDeviceInfo> bluetoothDeviceInfos = new List<BluetoothDeviceInfo>();
             
        public MainWindow()
        {
            localComponent.DiscoverDevicesProgress += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesProgress);
            localComponent.DiscoverDevicesComplete += new EventHandler<DiscoverDevicesEventArgs>(component_DiscoverDevicesComplete);
            InitializeComponent();
        }
       

        private void component_DiscoverDevicesProgress(object sender, DiscoverDevicesEventArgs e)
        {
            try
            {
                // log and save all found devices
                for (int i = 0; i < e.Devices.Length; i++)
                {
                    if (e.Devices[i].Remembered)
                    {
                        listBox1_btDevices.Items.Add(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress + "): Device is known");
                    }
                    else
                    {
                        listBox1_btDevices.Items.Add(e.Devices[i].DeviceName + " (" + e.Devices[i].DeviceAddress + "): Device is unknown");
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
            listBox1_btDevices.Items.Add("SCAN_COMPLETED");
            // log some stuff
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            listBox1_btDevices.Items.Clear();
            localComponent.DiscoverDevicesAsync(255, true, true, true, true, null);
        }
    }
}
