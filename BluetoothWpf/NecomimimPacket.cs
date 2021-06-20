using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BluetoothWpf
{
    public class NecomimimPacket
    {
       public enum CodeLevels
        {
            BATTERY_LEVEL = 0x01,
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

        private string _parsedTime;

        [Name("Время получения данных")]
        public string ParseTimeAbsolute
        {
            get
            {
                return _parsedTime;
            }
        }

        private string _parseTimeRelativeMinutes;

        [Name("Время с начала эксперимента")]
        public string ParseTimeRelative
        {
            get { return _parseTimeRelativeMinutes; }
        }
        public int AttentionCount { get; set; }

        [Name("Уровень заряда")]
        public byte BatteryLevel { get; set; }
        [Name("Индикатор плохого сигнала")]
        public byte PoorSignalQuality { get; set; }

        [Ignore]
        public byte HeartRate { get; set; }

        [Name("Уровень концентрации")]
        public byte ESenseAttention { get; set; }

        [Name("Уровень медитации")]
        public byte ESenseMeditation { get; set; }

        // TODO: Не есть правильно хранить это здесь, но это сделать будет быстрее.
        [Name("Контекст получения данных")]
        public string PacketContext { get; set; }

        [Name("Сырой сигнал, 8bit")]
        public byte RawWaveValue8bit { get; set; }

        [Name("Пометки для сырого сигнала")]
        public byte RawWaveMarker { get; set; }

        [Name("Сырой сигнал, 16bit")]
        public UInt16 RawWaveValue16bit { get; set; }

        // 32Bytes.
        [Ignore]
        public byte[] EegPower { get; set; }

        // 24bytes.
        [Ignore]
        public byte[] AsicEegPower { get; set; }

        UInt16 _printervalMs;
        [Ignore]
        public UInt16 PrintervalMs { get; set; }
        public NecomimimPacket()
        {
            PoorSignalQuality = 0;
            HeartRate = 0;
            ESenseAttention = 0;
            ESenseMeditation = 0;
            RawWaveMarker = 0;
            RawWaveValue16bit = 0;

            PacketContext = "Контекста нет";

            var currentTime = DateTime.Now;
            TimeSpan relativeTimeSpan = currentTime - ExperimentContext.LastExperimentBeginTime;
            _parsedTime = $"{currentTime}:{currentTime.Millisecond}";
            _parseTimeRelativeMinutes = $"{relativeTimeSpan.TotalHours}:{relativeTimeSpan.TotalMinutes};{relativeTimeSpan.TotalSeconds}";

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
