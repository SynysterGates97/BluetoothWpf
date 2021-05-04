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

        bool TryWritePacketsToCsv(ConcurrentQueue<NecomimimPacket> necomimimPacketsQueue)
        {
            if (necomimimPacketsQueue != null)
            {
                if (necomimimPacketsQueue.Count >= bufferizationCount)
                {
                    using (var writer = new StreamWriter(FileName))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        for(int i = 0; i < bufferizationCount; i++)
                        {
                            var nekomimiPacket = new NecomimimPacket();
                            bool isPacketDequed = necomimimPacketsQueue.TryDequeue(out nekomimiPacket);

                            if(isPacketDequed)
                                csv.WriteRecord(nekomimiPacket);
                        }
                        
                    }
                }
            }
            return true;
        }
    }
}
