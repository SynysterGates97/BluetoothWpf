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
        NecomimiBluetooth _necomimiBluetooth;
        LbLoger _lbLoger;
        void  FindNecomimmiDevice()
        {
            
            throw new NotImplementedException();
        }
        public MainWindow()
        {
            InitializeComponent();
            _lbLoger = new LbLoger(ref listBox1_btDevices);
            _necomimiBluetooth = new NecomimiBluetooth(ref _lbLoger);
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
