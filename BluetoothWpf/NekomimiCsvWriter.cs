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
            string woutRawFileName = AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName + ".csv";

            using (var rawWriter = new StreamWriter(rawFileName,true))
            using (var woutRawWriter = new StreamWriter(woutRawFileName, true))
            using (var rawCsv = new CsvWriter(rawWriter, CultureInfo.InvariantCulture))
            {
                using (var woutRawCsv = new CsvWriter(woutRawWriter, CultureInfo.InvariantCulture))
                {
                    foreach (var packet in necomimimPacketsQueue)
                    {
                        if(packet.ESenseAttention != 0 || packet.ESenseMeditation != 0)
                        {
                            woutRawCsv.WriteRecord(packet);
                            woutRawCsv.NextRecord();
                        }
                        rawCsv.WriteRecord(packet);
                        rawCsv.NextRecord();
                        writenPacks++;
                    }
                }
            }
            return writenPacks;
        }
    }
}
