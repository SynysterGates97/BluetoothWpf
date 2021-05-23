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
        DispatcherTimer _csvWriterTimer;

        private List<NecomimimPacket> necomimimPacketsToCsv;
        private NekomimiCsvWriter _nekomimiCsvWriter;

        public MainWindow()
        {
            InitializeComponent();
            _listOfLogMessages = new List<string>();
            _lbLoger = new LbLoger();

            necomimimPacketsToCsv = new List<NecomimimPacket>();

            _logUpdatetimer = new DispatcherTimer();
            _logUpdatetimer.Tick += new EventHandler(LogUpdateTimerCallback);
            _logUpdatetimer.Interval = new TimeSpan(0, 0, 0, 0,100);

            _btControlTimer = new DispatcherTimer();
            _btControlTimer.Tick += new EventHandler(CheckBtConnectionCallback);
            _btControlTimer.Interval = new TimeSpan(0, 0, 5);

            _csvWriterTimer = new DispatcherTimer();
            _csvWriterTimer.Tick += new EventHandler(CsvWriteTimerCallback);
            _csvWriterTimer.Interval = new TimeSpan(0, 0, 1);

            _nekomimiCsvWriter = new NekomimiCsvWriter();

            ExperimentContext.CurrentContext = "Контекста нет";

            _btControlTimer.Start();

            _lbLoger.PropertyChanged += _lbLoger_PropertyChanged;
           
            _necomimiBluetooth = new NecomimiBluetooth(ref _lbLoger);
        }

        private void CsvWriteTimerCallback(object sender, EventArgs e)
        {
            _nekomimiCsvWriter.FileName = textBox_testSubject.Text;

            necomimimPacketsToCsv.Clear();
            // 1 pack per 7.5 ms = 133 packs per second
            int dequedPacks = _necomimiBluetooth.GetNLastParsedPacketsFromQueue(600, ref necomimimPacketsToCsv);

            int writenPacks = _nekomimiCsvWriter.TryWritePacketsToCsv(ref necomimimPacketsToCsv);

            if(dequedPacks == 0)
            {
                Console.Beep();
            }
            if (writenPacks == dequedPacks)
            {
                _lbLoger.Print($"Записано {writenPacks}");
            }
            else
            {
                _lbLoger.Print($"Запись не удалась {dequedPacks}->{writenPacks}");
            }
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
            _csvWriterTimer.Start();
        }
    }
}
