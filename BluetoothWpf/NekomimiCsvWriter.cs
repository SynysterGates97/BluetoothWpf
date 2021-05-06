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
            string exeFolder = AppDomain.CurrentDomain.BaseDirectory + "\\" + FileName;
            using (var writer = new StreamWriter(exeFolder,true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var packet in necomimimPacketsQueue)
                {
                    csv.WriteRecord(packet);
                    csv.NextRecord();
                    writenPacks++;
                }
            }
            return writenPacks;
        }
    }
}
