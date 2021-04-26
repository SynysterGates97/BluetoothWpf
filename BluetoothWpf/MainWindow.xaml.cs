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
using System.Windows.Threading;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using MahApps.Metro.Controls;


namespace BluetoothWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        NecomimiBluetooth _necomimiBluetooth;
        LbLoger _lbLoger;
        private List<string> _listOfLogMessages;

        DispatcherTimer _logUpdatetimer;
        DispatcherTimer _btControlTimer;
       
        public MainWindow()
        {
            InitializeComponent();
            _listOfLogMessages = new List<string>();
            _lbLoger = new LbLoger();

            _logUpdatetimer = new DispatcherTimer();
            _logUpdatetimer.Tick += new EventHandler(LogUpdateTimerCallback);
            _logUpdatetimer.Interval = new TimeSpan(0, 0, 0, 0,100);

            _btControlTimer = new DispatcherTimer();
            _btControlTimer.Tick += new EventHandler(CheckBtConnectionCallback);
            _btControlTimer.Interval = new TimeSpan(0, 0, 5);

            _btControlTimer.Start();

            _lbLoger.PropertyChanged += _lbLoger_PropertyChanged;
           
            _necomimiBluetooth = new NecomimiBluetooth(ref _lbLoger);
        }

        private void CheckBtConnectionCallback(object sender, EventArgs e)
        {

            _necomimiBluetooth.ControlNecomomiDeviceConnection();

        }
            private void LogUpdateTimerCallback(object sender, EventArgs e)
        {
            _listOfLogMessages.Clear();

            _lbLoger.Flush(ref _listOfLogMessages);

            if(listBox1_btDevices.Items.Count >= 50)
            {
                listBox1_btDevices.Items.Clear();
            }

            if (_listOfLogMessages.Count > 0)
            {
                foreach (string logMessage in _listOfLogMessages)
                {
                    listBox1_btDevices.Items.Add(logMessage);
                }
            }
            _logUpdatetimer.Stop();
            // код здесь
        }

        private void _lbLoger_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            _logUpdatetimer.Start();
        }

        private void button_scan_Click(object sender, RoutedEventArgs e)
        {
            _necomimiBluetooth.FindNecomimiDevice();
            
        }

        private void button_pair_Click(object sender, RoutedEventArgs e)
        {
            _necomimiBluetooth.PairNecomimiDevice();
        }

        private void button_connect_Click(object sender, RoutedEventArgs e)
        {
            _necomimiBluetooth.ConnectToNecomimi();
        }

        private void button_receive_Click(object sender, RoutedEventArgs e)
        {
            _necomimiBluetooth.Receive();
        }
    }
}
