using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothWpf
{
    class NecomimiReceiver : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //Из рассчета, что в секунду приходит около 800 байт: 133 3 байтных RAW + MED + ATT + (баттарея, плохой уровень сигнала...)
        //Поэтому 2048 должно хватить

        private NetworkStream _btStream;
        //private const int BUFFER_SIZE = 2048;
        //private byte[] _buffer;

        private BinaryWriter binWriter;
        private BinaryReader binReader;

        private Task ReadBtBufferTask;

        public NecomimiReceiver()
        {
            binWriter = new BinaryWriter(new MemoryStream());
            binReader = new BinaryReader(binWriter.BaseStream);
            binReader.BaseStream.Position = 0;

            ReadBtBufferTask = new Task(ReadBtDelegate);
            
        }

        void StartReceiving(ref NetworkStream btStream)
        {            
            _btStream = btStream;
            ReadBtBufferTask.Start();
        }

        private Mutex mutex = new Mutex();

        void ReadBtDelegate()
        {
            if (_btStream == null)
                return;

            byte[] readBuffer = new byte[256];
            //byte, чтобы точно не было превышения
            int byteInBufCounter = 0;
            while(true)
            {
                while(_btStream.DataAvailable)
                {
                    int readByte = _btStream.ReadByte();
                    if (readByte != -1)
                    {
                        readBuffer[byteInBufCounter] = (byte)readByte;
                        byteInBufCounter++;
                        if(byteInBufCounter >= 256)
                        {
                            //PARSE_ARRAY(readBuffer, len);

                        }

                    }
                        binWriter.Write(readByte);
                }
            }

        }
        //

        ///////////////////////////////////////////////////////////////////////
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
