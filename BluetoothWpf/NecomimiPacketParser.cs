using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BluetoothWpf
{
    public class NecomimiPacketParser
    {
        //Ряд атрибутов, типа амплитуды ритмов, уровни медитации/концентрации.
        //по умолчанию параметры отрицательные

        NecomimimPacket necomimimPacket;

        //TODO: Нужно перенести в другой класс, т.к. пока расположение тут нелогично
        public Queue<NecomimimPacket> ParsedPacketsQueue { get; set; }
        public NecomimiPacketParser()
        {
            ParsedPacketsQueue = new Queue<NecomimimPacket>();
            //new necomimimPacket necomimimPacket
        }

        enum ParsingResult
        {
            PARSED_OK,
            HEADER_NOT_FOUND,
            NOT_ENOUGH_DATA,
            WRONG_CRC,
            ERROR
        }

        static bool IsCrcOk(byte[] payload, int beginIndex, int crcIndex)
        {
            byte calcCrc8Nec = 0;
            for (int i = beginIndex; i <= crcIndex; i++)
            {
                calcCrc8Nec += payload[i];
            }
            calcCrc8Nec = (byte)(~calcCrc8Nec);

            if (calcCrc8Nec == payload[crcIndex + 1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private int ParseHeader(byte[] rxBuf, int beginIndex, int len)
        {
            int parsingIndex = beginIndex;
            while (len - parsingIndex >= 6)
            {
                if (rxBuf[parsingIndex] == 0xAA)
                {
                    parsingIndex++;
                    if (rxBuf[parsingIndex] == 0xAA)
                    {
                        parsingIndex++;
                        int sizeOfPayload = rxBuf[parsingIndex];
                        if (sizeOfPayload != 0xAA)
                        {
                            return parsingIndex;
                        }
                        else
                        {
                            parsingIndex++;
                        }
                        //Разобран HEADER
                        int length = rxBuf[parsingIndex + 2];
                    }
                }
                else
                {
                    parsingIndex++;
                }
            }
            return -1;
        }

        //на вход подается пакет протокола necomimi: 
        public int Parse(byte[] rxBuf, int bufLen, ref Queue<NecomimimPacket> necomimimPacketsQueue)
        {
            //минимальный размер пакета по факту -  6 байт
            int parsingIndex = 0;
            int newParsedValues = 0;
            while (bufLen - parsingIndex >= 6)
            {
                int headerOffset = ParseHeader(rxBuf, parsingIndex, bufLen);

                if (headerOffset != -1)
                {
                    parsingIndex = headerOffset;

                    int sizeOfPacket = rxBuf[parsingIndex];
                    if (sizeOfPacket + 1 <= bufLen)
                    {
                        parsingIndex++;
                        //Данных хватает
                        int payloadBeginIndex = parsingIndex;
                        //todo: при отладке проверить.
                        int crcIndex = payloadBeginIndex + sizeOfPacket - 1;
                        bool isCrcOk = IsCrcOk(rxBuf, payloadBeginIndex, crcIndex);

                        if (isCrcOk)
                        {
                            NecomimimPacket newParsedNecomimiPacket = null;
                            while (parsingIndex < crcIndex)
                            {
                                newParsedNecomimiPacket = new NecomimimPacket();

                                NecomimimPacket.CodeLevels codeLevel = (NecomimimPacket.CodeLevels)rxBuf[parsingIndex];

                                switch (codeLevel)
                                {
                                    case (NecomimimPacket.CodeLevels.ATTENTION):
                                        {
                                            newParsedNecomimiPacket.ESenseAttention = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.MEDITATION):
                                        {
                                            newParsedNecomimiPacket.ESenseMeditation = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.POOR_SIGNAL_QUALITY):
                                        {
                                            newParsedNecomimiPacket.PoorSignalQuality = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.BATTERY_LEVEL):
                                        {
                                            newParsedNecomimiPacket.BatteryLevel = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    // Нужно обязательно все другие прописать!!!
                                    default:
                                        parsingIndex++;
                                        break;
                                }
                                if (newParsedNecomimiPacket != null)
                                {
                                    necomimimPacketsQueue.Enqueue(newParsedNecomimiPacket);
                                    newParsedValues++;
                                }
                            }

                            
                        }
                    }
                    else
                    {
                        parsingIndex++;
                    }                   
                }
                else
                {
                    parsingIndex++;
                }
            }
            if (newParsedValues > 0)
                return newParsedValues;
            else
                return -1;
        }

    }
}
