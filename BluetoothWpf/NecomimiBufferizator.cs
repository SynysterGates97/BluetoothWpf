using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWpf
{
    class NecomimiBufferizator
    {
        //Из рассчета, что в секунду приходит около 800 байт: 133 3 байтных RAW + MED + ATT + (баттарея, плохой уровень сигнала...)
        //Поэтому 2048 должно хватить

        NecomimiPacketParser NecomimiPacketParser;
        private const int BUFFER_SIZE = 2048;
        private byte[] _buffer;

        private int _bytesInBuffer;

        private int MINIMUM_PACKET_SIZE = 6;

        public int BytesInBuffer
        {
            get { return _bytesInBuffer; }
        }


        public NecomimiBufferizator()
        {
            NecomimiPacketParser = new NecomimiPacketParser();
            _buffer = new byte[BUFFER_SIZE];
            _bytesInBuffer = 0;
        }

        public void GetAndParseNewBytes(byte[] rxBuf, int bufLen)
        {
            _bytesInBuffer += bufLen;
            //TODO: потенциально переполнение буфера)
            Array.Copy(_buffer, _bytesInBuffer, rxBuf, 0, bufLen);

            while(_bytesInBuffer >= MINIMUM_PACKET_SIZE)
            {
                //NecomimiPacketParser.Parse(_buffer, _bytesInBuffer);
            }






        }

    }
}
