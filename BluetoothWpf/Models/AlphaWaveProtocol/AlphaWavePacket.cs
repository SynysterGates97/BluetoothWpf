using System;

namespace BluetoothWpf.Models
{
    public class AlphaWavePacket
    {
        private byte _attention;
        private byte _meditation;
        private byte _crc8;
        private UInt32 _packetNumber;

        public byte Attention
        {
            get => _attention;
            set => _attention = value;
        }

        public byte Meditation
        {
            get => _meditation;
            set => _meditation = value;
        }

        public byte Crc8
        {
            get => _crc8;
            set => _crc8 = value;
        }

        public uint PacketNumber
        {
            get => _packetNumber;
            set => _packetNumber = value;
        }

        public AlphaWavePacket(byte attention, byte meditation, uint packetNumber = 0)
        {
            _attention = attention;
            _meditation = meditation;
            _packetNumber = packetNumber;
            _crc8 = (byte)(_attention ^ _meditation);
        }

        public bool IsCrcOk(byte receivedCrc)
        {
            return _crc8 == receivedCrc;
        }
        
    }
}