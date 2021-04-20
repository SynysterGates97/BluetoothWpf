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
            new necomimimPacket
        }

        //на вход подается пакет протокола necomimi: AA AA Size ........ CRC
        static public bool Parse()
        {

            throw new NotImplementedException();
        }
        public int MyProperty { get; set; }
    }

}
