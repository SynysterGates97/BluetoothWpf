using System;
using System.Collections;
using System.Collections.Concurrent;
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

        private Mutex packetsInQueueMutex;
        private ConcurrentQueue<NecomimimPacket> necomimimPackets;

        private NetworkStream _btStream;
        //private const int BUFFER_SIZE = 2048;
        //private byte[] _buffer;

        private BinaryWriter binWriter;
        private BinaryReader binReader;

        private Task ReadBtBufferTask;

        public ConcurrentQueue<NecomimimPacket> ParsedPacketsQueue { get; }

        public NecomimiReceiver()
        {
            binWriter = new BinaryWriter(new MemoryStream());
            binReader = new BinaryReader(binWriter.BaseStream);

            packetsInQueueMutex = new Mutex();
            binReader.BaseStream.Position = 0;

            necomimimPackets = new ConcurrentQueue<NecomimimPacket>();

            ReadBtBufferTask = new Task(ReadBtDelegate);
            
        }

        public void StartReceiving(ref NetworkStream btStream)
        {            
            _btStream = btStream;
            ReadBtBufferTask.Start();
        }


        private void ReadBtDelegate()
        {
            if (_btStream == null)
                return;

            byte[] readBuffer = new byte[256];
            //byte, чтобы точно не было превышения

            int byteInBufCounter = 0;
            while (true)
            {
                
                while (_btStream.DataAvailable)
                {
                    int readByte = _btStream.ReadByte();
                    if (readByte != -1)
                    {
                        readBuffer[byteInBufCounter] = (byte)readByte;
                        byteInBufCounter++;
                        if(byteInBufCounter > 255)
                        {
                            break;
                        }

                    }
                }

                //6 минимальный размер пакета, избавиться от магии?
                if(byteInBufCounter != 0 && byteInBufCounter >=6)
                {
                    NecomimiPacketParser.Parse(readBuffer, byteInBufCounter, ref necomimimPackets);
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
