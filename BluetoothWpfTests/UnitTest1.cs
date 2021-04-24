using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluetoothWpf;
using System.Collections;
using System.Collections.Generic;

namespace BluetoothWpfTests
{
    [TestClass]
    public class UnitTest1
    {
        NecomimiPacketParser necomimiPacketParser = new NecomimiPacketParser();
        [TestMethod]
        public void TestParser()
        {
            byte[] testBuffer = { 0xAA, 0xAA, 0x08,0x02,
                0x20,0x01,0x7E,0x04,
                0x12,0x05,0x60,0xe3 };

            Queue<NecomimimPacket> queue = new Queue<NecomimimPacket>();
            necomimiPacketParser.Parse(testBuffer, 12, ref queue);

            NecomimimPacket parsedNecomimimPacket = queue.Dequeue();
            Assert.AreEqual(1,1);
       
        }
    }
}
