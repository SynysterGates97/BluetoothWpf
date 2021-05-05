using CsvHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace BluetoothWpf
{
    class NekomimiCsvWriter
    {
        public string FileName { get; set; }
        private const int bufferizationCount = 20;

        public NekomimiCsvWriter()
        {
        }

        public int TryWritePacketsToCsv(ref List<NecomimimPacket> necomimimPacketsQueue)
        {
            int writenPacks = 0;
            using (var writer = new StreamWriter(FileName))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                foreach (var packet in necomimimPacketsQueue)
                {
                    csv.WriteRecord(packet);
                    writenPacks++;
                }
            }
            return writenPacks;
        }
    }
}
