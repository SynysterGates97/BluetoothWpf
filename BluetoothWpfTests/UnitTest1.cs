using Microsoft.VisualStudio.TestTools.UnitTesting;
using BluetoothWpf;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace BluetoothWpfTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestParser()
        {

            //byte: value // Explanation
            //[0]: 0xAA  // [SYNC]
            //[1]: 0xAA  // [SYNC]
            //[2]: 0x08  // [PLENGTH] (payload length) of 8 bytes
            //[3]: 0x02  // [CODE] POOR_SIGNAL Quality
            //[4]: 0x20  // Some poor signal detected (32/255)
            //[5]: 0x01  // [CODE] BATTERY Level
            //[6]: 0x7E  // Almost full 3V of battery (126/127)
            //[7]: 0x04  // [CODE] ATTENTION eSense
            //[8]: 0x12  // eSense Attention level of 18%
            //[9]: 0x05  // [CODE] MEDITATION eSense
            //[10]: 0x60  // eSense Meditation level of 96%
            //[11]: 0xE3  // [CHKSUM] (1's comp inverse of 8-bit Payload sum of 0x1C)

            byte[] testBuffer = { 
                0xBB,0xCC,0xAA,0xAA,0x03,0xAA,
                0xAA, 0xAA, 
                0x08,
                0x02,
                    0x20,
                0x01,
                    0x7E,
                0x04,
                    0x12,
                0x05,
                    0x60,
                0xe3 
            };

            ConcurrentQueue<NecomimimPacket> queue = new ConcurrentQueue<NecomimimPacket>();
            NecomimiPacketParser.Parse(testBuffer, 12, ref queue);


            bool isParsingOk = false;
            if (queue.Count != 4)
                Assert.Fail();
            else 
            {
                //todo: Нужно будет сделать нормально
                NecomimimPacket parsedNecomimimPacketPoorSignal;
                    queue.TryDequeue(out parsedNecomimimPacketPoorSignal);
                NecomimimPacket parsedNecomimimPacketBattery;
                    queue.TryDequeue(out parsedNecomimimPacketBattery);
                NecomimimPacket parsedNecomimimPacketAttention;
                    queue.TryDequeue(out parsedNecomimimPacketAttention);
                NecomimimPacket parsedNecomimimPacketMeditaion;
                    queue.TryDequeue(out parsedNecomimimPacketMeditaion);

                bool isPoorSignalParsedOk = parsedNecomimimPacketPoorSignal.PoorSignalQuality == 0x20;
                bool isBatteryLevelParsedOk = parsedNecomimimPacketBattery.BatteryLevel == 0x7E;
                bool isAttentionParsedok = parsedNecomimimPacketAttention.ESenseAttention == 0x12;
                bool isMeditationParsedok = parsedNecomimimPacketMeditaion.ESenseMeditation == 0x60;

                isParsingOk = isPoorSignalParsedOk && isBatteryLevelParsedOk &&
                    isAttentionParsedok && isMeditationParsedok;
            }

            Assert.IsTrue(isParsingOk == true);
        }
    }
}
