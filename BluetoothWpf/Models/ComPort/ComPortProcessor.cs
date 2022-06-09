

using System;
using System.Text;
using RJCP.IO.Ports;

namespace BluetoothWpf.Models.ComPort
{
    public static class EncodingExtensions
    {
        public static string GetString(this Encoding encoding, Span<byte> source)
        {
            //naive way using ToArray, but possible to improve when needed
            return encoding.GetString(source.ToArray());
        }
    }
    
    public class ComPortProcessor
    {
        private SerialPortStream _serialPortStream;

        private byte[] _buffer = new byte[2048];
        public ComPortProcessor(int comPortNumber)
        {
            var comPortString = $"COM{comPortNumber}";
            _serialPortStream = new SerialPortStream(comPortString, 115200, 8, Parity.None, StopBits.One);
            
            _serialPortStream.DataReceived += SerialPortStreamOnDataReceived;
            _serialPortStream.Open();
        }

        // TODO: Этого в этом классе быть НЕ ДОЛЖНО!
        private AlphaWavePacket _lastAlphaWavePacket;

        public AlphaWavePacket LastAlphaWavePacket
        {
            get => _lastAlphaWavePacket;
        }

        private void SerialPortStreamOnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            Span<byte> bufferSpan = new Span<byte>(_buffer);
            _serialPortStream.Read(bufferSpan);

            var rxString =  Encoding.ASCII.GetString(bufferSpan);
            var parsedPacket = AlphaWaveProtocolParser.ParsePacketFromString(rxString);
            if (parsedPacket != null)
            {
                _lastAlphaWavePacket = parsedPacket;
            }

            // e.EventType == SerialData.Chars
        }
    }
}