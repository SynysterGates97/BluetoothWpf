using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BluetoothWpf
{
    public class NecomimiPacketParser
    {
        private const int PARSE_BUFF_SIZE = 5120;
        static private byte[] _parseBuff = new byte[PARSE_BUFF_SIZE];
        static private byte[] _transferBuff = new byte[PARSE_BUFF_SIZE];

        static private int _parsingBytesCount = 0;
        enum ParsingResult
        {
            PARSED_OK,
            HEADER_NOT_FOUND,
            NOT_ENOUGH_DATA,
            WRONG_CRC,
            ERROR
        }

        static bool IsCrcOk(byte[] payload, int beginIndex, int crcIndex, int bufLen)
        {
            byte calcCrc8Nec = 0;
            //TODO: все же верхний код должен делать проверку
            if (crcIndex >= bufLen - 1)
                return false;

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

        static private int ParseHeader(byte[] rxBuf, int beginIndex, int len)
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

        static private void BufferizePacket(byte[] rxBuf, int bufLen)
        {
            if (bufLen + _parsingBytesCount <= PARSE_BUFF_SIZE)
            {
                System.Array.Copy(rxBuf, 0, _parseBuff, _parsingBytesCount, bufLen);
                _parsingBytesCount += bufLen;
            }

        }

        static public int Parse(byte[] rxBuf, int bufLen, ref ConcurrentQueue<NecomimimPacket> necomimimPacketsQueue)
        {
            BufferizePacket(rxBuf, bufLen);

            int parsingBytesCountBeforeParsing = _parsingBytesCount;

            var parseResult = _Parse(_parseBuff, bufLen, ref necomimimPacketsQueue);

            int parsedUpToIndex = parseResult.Item1;
            int parsedValuesCount = parseResult.Item2;

            System.Array.Copy(_parseBuff, _transferBuff, PARSE_BUFF_SIZE);
            System.Array.Copy(_transferBuff, parsedUpToIndex, _parseBuff, 0, parsingBytesCountBeforeParsing - parsedUpToIndex);

            _parsingBytesCount = parsingBytesCountBeforeParsing - parsedUpToIndex;

             return parsedValuesCount;
        }


        static int attentionCount = 0;
        //на вход подается Массив байт размером bufLen : 
        static private Tuple<int,int> _Parse(byte[] rxBuf, int bufLen, ref ConcurrentQueue<NecomimimPacket> necomimimPacketsQueue)
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

                        bool isCrcOk = IsCrcOk(rxBuf, payloadBeginIndex, crcIndex, bufLen);

                        if (isCrcOk)
                        {
                            NecomimimPacket newParsedNecomimiPacket = null;
                            while (parsingIndex < crcIndex)
                            {
                                newParsedNecomimiPacket = new NecomimimPacket();
                                newParsedNecomimiPacket.PacketContext = ExperimentContext.CurrentContext;

                                NecomimimPacket.CodeLevels codeLevel = (NecomimimPacket.CodeLevels)rxBuf[parsingIndex];

                                switch (codeLevel)
                                {
                                    case (NecomimimPacket.CodeLevels.ATTENTION):
                                        {
                                            if (ExperimentContext.isContextChangedAttention)
                                            {
                                                newParsedNecomimiPacket.ContextChangedIndicator = 100;
                                                ExperimentContext.isContextChangedAttention = false;
                                            }
                                            newParsedNecomimiPacket.AttentionCount = attentionCount;
                                            attentionCount++;
                                            newParsedNecomimiPacket.ESenseAttention = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.MEDITATION):
                                        {
                                            if (ExperimentContext.isContextChangedMeditation)
                                            {
                                                newParsedNecomimiPacket.ContextChangedIndicator = 100;
                                                ExperimentContext.isContextChangedMeditation = false;
                                            }
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
                                        //не готово
                                    case (NecomimimPacket.CodeLevels.ASIC_EEG_POWER):
                                        {
                                            //(AsicEegPower, 0, 24);
                                            //TODO: на всякий случай дописать
                                            //newParsedNecomimiPacket.AsicEegPower = rxBuf[parsingIndex + 1];
                                            parsingIndex += 25;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.EEG_POWER):
                                        {
                                            //(AsicEegPower, 0, 24);
                                            //TODO: на всякий случай дописать
                                            //newParsedNecomimiPacket.EegPower = rxBuf[parsingIndex + 1];
                                            parsingIndex += 33;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.HEART_RATE):
                                        {
                                            newParsedNecomimiPacket.HeartRate = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.NEVER_USED):
                                        {
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.RAW_8BIT):
                                        {
                                            newParsedNecomimiPacket.RawWaveValue8bit = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.RAW_MARKER):
                                        {
                                            newParsedNecomimiPacket.RawWaveMarker = rxBuf[parsingIndex + 1];
                                            parsingIndex += 2;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.RAW_WAVE_VALUE):
                                        {
                                            UInt16 firstByte = rxBuf[parsingIndex + 1];
                                            UInt16 secondByte = rxBuf[parsingIndex + 2];
                                            newParsedNecomimiPacket.RawWaveValue16bit = (UInt16)(firstByte << 8 | secondByte);
                                            parsingIndex += 3;
                                            break;
                                        }
                                    case (NecomimimPacket.CodeLevels.RRINTERVAL):
                                        {
                                            UInt16 firstByte = rxBuf[parsingIndex + 1];
                                            UInt16 secondByte = rxBuf[parsingIndex + 2];
                                            newParsedNecomimiPacket.PrintervalMs = (UInt16)(firstByte << 8 | secondByte);
                                            parsingIndex += 3;
                                            break;
                                        }
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

                            parsingIndex++;
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
                return new Tuple<int,int>(parsingIndex, newParsedValues);
            else
                return new Tuple<int, int>(parsingIndex, -1);
        }

    }
}
