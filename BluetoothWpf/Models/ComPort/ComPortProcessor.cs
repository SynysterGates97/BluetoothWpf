

using RJCP.IO.Ports;

namespace BluetoothWpf.Models.ComPort
{
    public class ComPortProcessor
    {
        private SerialPortStream _serialPortStream;

        public ComPortProcessor(int comPortNumber)
        {
            var comPortString = $"COM{comPortNumber}";
            _serialPortStream = new SerialPortStream(comPortString, 115200, 8, Parity.None, StopBits.One);
            
            _serialPortStream.DataReceived += SerialPortStreamOnDataReceived;
        }

        private void SerialPortStreamOnDataReceived(object? sender, SerialDataReceivedEventArgs e)
        {
            // e.EventType == SerialData.Chars
        }
    }
}