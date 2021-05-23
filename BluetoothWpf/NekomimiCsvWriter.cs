using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace BluetoothWpf
{
    class NekomimiCsvWriter
    {
        public string FileName { get; set; }
        private const int bufferizationCount = 20;

        public NekomimiCsvWriter()
        {
            FileName = "StandartName";
        }

        public int TryWritePacketsToCsv(ref List<NecomimimPacket> necomimimPacketsQueue)
        {
            int writenPacks = 0;
            string rawFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName + "_raw.csv";
            string attentionFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName + "_attention.csv";
            string meditationFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName + "_meditation.csv";

            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8, LeaveOpen = false};
            // Чет пахнет, нужно почитать, как поступать в таких случаях
            using (var rawWriter = new StreamWriter(rawFileName, true, Encoding.UTF8))
            using (var attentionWriter = new StreamWriter(attentionFileName, true, Encoding.UTF8))
            using (var meditationWriter = new StreamWriter(meditationFileName, true, Encoding.UTF8))
            using (var rawCsv = new CsvWriter(rawWriter, config))
            using (var attentionCsv = new CsvWriter(attentionWriter, config))
            using (var meditationCsv = new CsvWriter(meditationWriter, config))
            {
                foreach (var packet in necomimimPacketsQueue)
                {
                    if (packet.ESenseAttention != 0)
                    {
                        attentionCsv.WriteRecord(packet);
                        attentionCsv.NextRecord();
                    }
                    if (packet.ESenseMeditation != 0)
                    {
                        meditationCsv.WriteRecord(packet);
                        meditationCsv.NextRecord();
                    }
                    rawCsv.WriteRecord(packet);
                    rawCsv.NextRecord();

                    writenPacks++;
                }
            }
            return writenPacks;
        }
    }
}
