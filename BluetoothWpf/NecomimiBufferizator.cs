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
    class NecomimiBufferizator : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //Из рассчета, что в секунду приходит около 800 байт: 133 3 байтных RAW + MED + ATT + (баттарея, плохой уровень сигнала...)
        //Поэтому 2048 должно хватить

        private NetworkStream _btStream;
        private const int BUFFER_SIZE = 2048;
        private byte[] _buffer;

        private BinaryWriter binWriter;
        private BinaryReader binReader;

        private Task ReadBtBufferTask;

        public NecomimiBufferizator()
        {
            binWriter = new BinaryWriter(new MemoryStream());
            binReader = new BinaryReader(binWriter.BaseStream);
            binReader.BaseStream.Position = 0;

            _buffer = new byte[BUFFER_SIZE];
            ReadBtBufferTask = new Task(ReadBtDelegate);
            
        }

        void SetStream(ref NetworkStream btStream)
        {
            _btStream = btStream;
        }

        private Mutex mutex = new Mutex();

        void ReadBtDelegate()
        {
            if (_btStream == null)
                return;
            
            while(true)
            {
                while(_btStream.DataAvailable)
                {
                    _btStream
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
