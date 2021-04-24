using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWpf
{
    public class NecomimimPacket
    {
        public enum CodeLevels
        {
            POOR_SIGNAL_QUALITY = 0x02,
            HEART_RATE = 0x03,
            ATTENTION = 0x04,
            MEDITATION = 0x05,
            RAW_8BIT = 0x06,
            RAW_MARKER = 0x07,
            RAW_WAVE_VALUE = 0x80,
            EEG_POWER = 0x81,
            ASIC_EEG_POWER = 0x83,
            RRINTERVAL = 0x86,
            NEVER_USED = 0x55
        }
        public byte PoorSignalQuality { get; set; }

        public byte HeartRate { get; set; }

        public byte ESenseAttention { get; set; }

        public byte ESenseMeditation { get; set; }

        public byte RawWaveValue8bit { get; set; }

        public byte RawWaveMarker { get; set; }

        public UInt16 RawWaveValue16bit { get; set; }

        // 32Bytes.
        public byte[] EegPower { get; set; }

        // 24bytes.
        public byte[] AsicEegPower { get; set; }

        UInt16 _printervalMs;
        public UInt16 PrintervalMs { get; set; }
        public NecomimimPacket()
        {
            PoorSignalQuality = 0;
            HeartRate = 0;
            ESenseAttention = 0;
            ESenseMeditation = 0;
            RawWaveMarker = 0;
            RawWaveValue16bit = 0;

            EegPower = new byte[32];
            AsicEegPower = new byte[24];
            PrintervalMs = 0;
        }

        void Clear()
        {
            PoorSignalQuality = 0;
            HeartRate = 0;
            ESenseAttention = 0;
            ESenseMeditation = 0;
            RawWaveMarker = 0;
            RawWaveValue16bit = 0;

            Array.Clear(EegPower, 0, 32);
            Array.Clear(AsicEegPower, 0, 24);
            _printervalMs = 0;
        }
    }
}
