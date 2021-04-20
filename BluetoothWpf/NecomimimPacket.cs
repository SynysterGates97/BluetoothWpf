using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWpf
{
    class NecomimimPacket
    {

        byte _poorSignalQuality;
        public byte PoorSignalQuality
        {
            get { return _poorSignalQuality; }
        }

        byte _heartRate;
        public byte HeartRate
        {
            get { return _heartRate; }
        }

        byte _eSenseAttention;
        public byte ESenseAttention
        {
            get { return _eSenseAttention; }
        }

        byte _eSenseMeditation;
        public byte ESenseMeditation
        {
            get { return _eSenseMeditation; }
        }

        byte _rawWaveValue8bit;
        public byte RawWaveValue8bit
        {
            get { return _rawWaveValue8bit; }
        }

        byte _rawWaveMarker;
        public byte RawWaveMarker
        {
            get { return _rawWaveMarker; }
        }

        UInt16 _rawWaveValue16bit;
        public UInt16 RawWaveValue16bit
        {
            get { return _rawWaveValue16bit; }
        }

        // 32Bytes.
        byte[] _eegPower;
        public byte[] EegPower
        {
            get { return _eegPower; }
        }

        // 24bytes.
        byte[] _asicEegPower;
        public byte[] AsicEegPower
        {
            get { return _asicEegPower; }
        }

        UInt16 _printervalMs;
        public UInt16 PrintervalMs
        {
            get { return _printervalMs; }
        }

        public NecomimimPacket()
        {
            _poorSignalQuality = 0;
            _heartRate = 0;
            _eSenseAttention = 0;
            _eSenseMeditation = 0;
            _rawWaveMarker = 0;
            _rawWaveValue16bit = 0;

            _eegPower = new byte[32];
            _asicEegPower = new byte[24];
            _printervalMs = 0;
        }

        void Clear()
        {
            _poorSignalQuality = 0;
            _heartRate = 0;
            _eSenseAttention = 0;
            _eSenseMeditation = 0;
            _rawWaveMarker = 0;
            _rawWaveValue16bit = 0;

            Array.Clear(_eegPower, 0, 32);
            Array.Clear(_asicEegPower, 0, 24);
            _printervalMs = 0;
        }
    }
}
