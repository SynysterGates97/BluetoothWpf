using System.Text.RegularExpressions;

namespace BluetoothWpf.Models
{
    public static class AlphaWaveProtocolParser
    {
        // Regex protcolPattern = "$$CRC8=%d;A=%d;M=%d;#%d;$$"
        private const string _protcolPattern = 
            @"\$\$CRC8=(?<crc8>\d+?);A=(?<attention>\d+?);M=(?<meditation>\d+?);#(?<packNumber>\d+?);\$\$";

        public static AlphaWavePacket ParsePacketFromString(string receivedPacket)
        {
            var matches = Regex.Matches(receivedPacket, _protcolPattern);
            if (matches != null && matches.Count > 0)
            {
                var match = matches[0];
                var crc8String = match.Groups["crc8"].Value;
                var attentionString = match.Groups["attention"].Value;
                var meditationString = match.Groups["meditation"].Value;

                byte rxCrc8 = 0;
                byte rxAttention = 0;
                byte rxMeditation = 0;
                
                bool isValuesInPacketOk = byte.TryParse(crc8String, out rxCrc8) &&
                                          byte.TryParse(attentionString, out rxAttention) &&
                                          byte.TryParse(meditationString, out rxMeditation);

                if (isValuesInPacketOk)
                {
                    AlphaWavePacket parsedPacket = new AlphaWavePacket(rxAttention, rxMeditation);
                    if (parsedPacket.IsCrcOk(rxCrc8))
                    {
                        return parsedPacket;
                    }
                }
            }
            return null;
        }
        
    }
}