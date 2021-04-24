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

            if (calcCrc8Nec == payload[crcIndex+1])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //на вход подается пакет протокола necomimi: 
        public int Parse(byte[] rxBuf, int bufLen, ref Queue<NecomimimPacket> necomimimPacketsQueue)
        {
            int parsingIndex = 0;
            while (parsingIndex < bufLen)
            {
                if (rxBuf[parsingIndex] == 0xAA)
                {
                    if (rxBuf[parsingIndex+1] == 0xAA)
                    {
                        //Разобран HEADER
                        int length = rxBuf[parsingIndex+2];
                        if (length + 1 <= bufLen)
                        {
                            //Данных хватает
                            int payloadBeginIndex = parsingIndex + 3;
                            //todo: при отладке проверить.
                            int crcIndex = payloadBeginIndex + length - 1;
                            bool isCrcOk = IsCrcOk(rxBuf, payloadBeginIndex, crcIndex);

                            if (isCrcOk)
                            {
                                NecomimimPacket newParsedNecomimiPacket = new NecomimimPacket();
                                NecomimimPacket.CodeLevels codeLevel = (NecomimimPacket.CodeLevels)rxBuf[parsingIndex + 3];

                                switch (codeLevel)
                                {
                                    case(NecomimimPacket.CodeLevels.ATTENTION):
                                    {
                                        newParsedNecomimiPacket.ESenseAttention = rxBuf[parsingIndex + 4];
                                        break;
                                    }
                                    case(NecomimimPacket.CodeLevels.MEDITATION):
                                    {
                                        newParsedNecomimiPacket.ESenseMeditation = rxBuf[parsingIndex + 4];
                                        break;
                                    }
                                    default:
                                        break;
                                }

                                necomimimPacketsQueue.Enqueue(newParsedNecomimiPacket);

                                return (int)ParsingResult.PARSED_OK;
                                //PARSE_VALUES
                            }
                        }


                    }
                }
            }
            return (int)ParsingResult.ERROR;
        }
    }

}
