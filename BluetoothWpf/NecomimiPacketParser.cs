using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BluetoothWpf
{
    class NecomimiPacketParser
    {
        //Ряд атрибутов, типа амплитуды ритмов, уровни медитации/концентрации.
        //по умолчанию параметры отрицательные

        NecomimimPacket necomimimPacket;
        public NecomimiPacketParser()
        {
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

            if (calcCrc8Nec == payload[crcIndex])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //на вход подается пакет протокола necomimi: 
        static public int Parse(byte[] rxBuf, int bufLen)
        {
            int parsingIndex = 0;
            while (parsingIndex >= 2)
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
                            int crcIndex = payloadBeginIndex + length;
                            bool isCrcOk = IsCrcOk(rxBuf, payloadBeginIndex, crcIndex);

                            if (isCrcOk)
                            {
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
