using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using BluetoothWpf.Models;
using BluetoothWpf.Models.ComPort;
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
        DispatcherTimer _comDataUpdateTimer;

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
            _logUpdatetimer.Interval = new TimeSpan(0, 0, 0, 0, 100);

            _btControlTimer = new DispatcherTimer();
            _btControlTimer.Tick += new EventHandler(CheckBtConnectionCallback);
            _btControlTimer.Interval = new TimeSpan(0, 0, 5);

            _csvWriterTimer = new DispatcherTimer();
            _csvWriterTimer.Tick += new EventHandler(CsvWriteTimerCallback);
            _csvWriterTimer.Interval = new TimeSpan(0, 0, 1);

            _nekomimiCsvWriter = new NekomimiCsvWriter();
            FillContexts();

            _btControlTimer.Start();

            _lbLoger.PropertyChanged += _lbLoger_PropertyChanged;

            _necomimiBluetooth = new NecomimiBluetooth(ref _lbLoger);

            _comDataUpdateTimer = new DispatcherTimer();
            _comDataUpdateTimer.Tick += new EventHandler(ComDataUpdateHandler);
            _comDataUpdateTimer.Interval = new TimeSpan(0, 0, 1);
            
            _comDataUpdateTimer.Start();

            FuzzyLogicExample fuzzyLogicExample = new FuzzyLogicExample();

            int velocity = fuzzyLogicExample.FuzzyLogic(0, 0, 10);
            // MessageBox.Show(velocity.ToString());

            presetationPath_TextBox.Drop += UIElement_OnDrop;

        }

        private int _currentSlideDelaySec = 60; 

        private void ComDataUpdateHandler(object? sender, EventArgs e)
        {
            var temperament = (int)Temperament_Slider.Value;
            var age = (int)Age_Slider.Value;
            
            if (_comPortProcessor != null && _comPortProcessor.LastAlphaWavePacket != null)
            {
                int attention = _comPortProcessor.LastAlphaWavePacket.Attention;
                int meditaion = _comPortProcessor.LastAlphaWavePacket.Meditation;

                progressBar_Attention.Value = attention;
                progressBar_Meditation.Value = meditaion;
                
                FuzzyLogicExample fuzzyLogic = new FuzzyLogicExample();

                var velocityTerm = fuzzyLogic.FuzzyLogic(age, temperament, attention / 10);

                int velocityInProcent = velocityTerm / fuzzyLogic.GetOutTermDiscretenes() * 100;
                progressBar_Velocity.Value = velocityInProcent;

                if (velocityInProcent <= 25)
                {
                    _currentSlideDelaySec = 150;
                }
                else if (velocityInProcent <= 50)
                {
                    _currentSlideDelaySec = 120;
                }
                else if (velocityInProcent <= 75)
                {
                    _currentSlideDelaySec = 90;
                }
                else if (velocityInProcent <= 100)
                {
                    _currentSlideDelaySec = 60;
                }
            }
        }

        private ComPortProcessor _comPortProcessor;
        private void button_startWorkWithCom_Click(object sender, RoutedEventArgs e)
        {
            var comNumberText = textBox_comPortNumber.Text;
            if (comNumberText != "")
            {
                
                int portNumber = 0;
                bool valueIsOk = int.TryParse(comNumberText, out portNumber);

                if (valueIsOk)
                {
                    _comPortProcessor = new ComPortProcessor(portNumber);
                }
            }
            else
            {
                MessageBox.Show("Введите имя!");
            }
        }

        private void FillContexts()
        {
            ExperimentContext.CurrentContext = "Начало теста";

            //AppDomain.CurrentDomain.BaseDirectory
            string[] contextFiles = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}\\ExperimentImages", "*.*", SearchOption.TopDirectoryOnly).Select(System.IO.Path.GetFileName).ToArray();

            foreach (var cotextFileName in contextFiles)
            {
                comboBox_experimentContext.Items.Add(cotextFileName);
            }
            comboBox_experimentContext.Items.Add("Переход в сенсорную комнату");
            comboBox_experimentContext.Items.Add("Сеанс релаксации");
        }

        private void CsvWriteTimerCallback(object sender, EventArgs e)
        {
            necomimimPacketsToCsv.Clear();
            // 1 pack per 7.5 ms = 133 packs per second
            int dequedPacks = _necomimiBluetooth.GetNLastParsedPacketsFromQueue(1000, ref necomimimPacketsToCsv);

            int writenPacks = _nekomimiCsvWriter.TryWritePacketsToCsv(ref necomimimPacketsToCsv);

            if(dequedPacks == 0)
            {
                Console.Beep();
            }
            if (writenPacks == dequedPacks)
            {
                _lbLoger.Print($"Записано {writenPacks}. {ExperimentContext.CurrentContext}. {ExperimentContext.TestSubjectName}");
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

            if(listBox1_btDevices.Items.Count >= 10)
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

        private void button_receive_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_testSubject.Text != "")
            {
                if (_nekomimiCsvWriter.FileName != textBox_testSubject.Text)
                {
                    _nekomimiCsvWriter.FileName = textBox_testSubject.Text;
                    _nekomimiCsvWriter.WriteHeader();
                    ExperimentContext.CurrentContext = comboBox_experimentContext.Text;
                    ExperimentContext.TestSubjectName = textBox_testSubject.Text;

                    HeaderNameOfTesterTextBlock.Text = ExperimentContext.TestSubjectName;
                }

                _necomimiBluetooth.Receive();                
                _csvWriterTimer.Start();
            }
            else
            {
                MessageBox.Show("Введите имя!");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_experimentContext.Text != null)
                ExperimentContext.CurrentContext = comboBox_experimentContext.Text;

            ExperimentContext.LastExperimentBeginTime = DateTime.Now;
        }

        private void button_SetSubjectName_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_testSubject.Text != "")
            {
                if (_nekomimiCsvWriter.FileName != textBox_testSubject.Text)
                {
                    _nekomimiCsvWriter.FileName = textBox_testSubject.Text;
                    _nekomimiCsvWriter.WriteHeader();
                    ExperimentContext.CurrentContext = comboBox_experimentContext.Text;
                    ExperimentContext.TestSubjectName = textBox_testSubject.Text ?? "Поле ФИО пустое";

                }
            }
            else
            {
                MessageBox.Show("Введите имя!");
            }
        }

        private void UIElement_OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files?.Count() != 0)
                {
                    string presentationPath = files[0];

                    presetationPath_TextBox.Text = files[0];

                }

            }
        }

        private void ButtonStartPresentation_OnClick(object sender, RoutedEventArgs e)
        {
            string presentationPath = presetationPath_TextBox.Text;

            if (presentationPath != null)
            {
                var application = new Microsoft.Office.Interop.PowerPoint.ApplicationClass();

                var ppApp = application.Presentations.Open(presentationPath);

                ppApp.SlideShowSettings.Run();

                var slidesCount = ppApp.Slides.Count;

                foreach (var slide in ppApp.Slides)
                {
                    Thread.Sleep(_currentSlideDelaySec * 1000);
                    ppApp.SlideShowWindow.View.Next();
                }
            }
        }

        private void comboBox_experimentContext_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string contextString = e.AddedItems[0] as string;

                if (contextString != null)
                    ExperimentContext.CurrentContext = contextString;

                ExperimentContext.LastExperimentBeginTime = DateTime.Now;

                HeaderContextTextBlock.Text = contextString; 

            }
            
        }
    }
}
