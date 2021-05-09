using CsvHelper;
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
            
            //Чет пахнет, нужно почитать, как поступать в таких случаях
            using (var rawWriter = new StreamWriter(rawFileName,true))
            using (var attentionWriter = new StreamWriter(attentionFileName, true))
            using (var meditationWriter = new StreamWriter(meditationFileName, true))
            using (var rawCsv = new CsvWriter(rawWriter, CultureInfo.InvariantCulture))
            using (var attentionCsv = new CsvWriter(attentionWriter, CultureInfo.InvariantCulture))
            using (var meditationCsv = new CsvWriter(meditationWriter, CultureInfo.InvariantCulture))
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
