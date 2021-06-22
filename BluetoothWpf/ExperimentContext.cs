using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWpf
{
    static class ExperimentContext
    {
        static string _currentContext = "Контекст не задан";
        public static string CurrentContext 
        {
            get
            {
                return _currentContext;
            }
            set
            {
                if(_currentContext != value)
                {
                    isContextChangedAttention = true;
                    isContextChangedMeditation = true;
                }
                _currentContext = value;
            }
        }

        public static string TestSubjectName
        { get; set; }

        public static DateTime LastExperimentBeginTime { get; set; }

        //ЛЮТЕЙШИЙ КОСТЫЛЬ ДЛЯ ОТОБРАЖЕНИЯ НА ГРАФИКАХ СМЕНЫ КОНТЕКСТА

        public static bool isContextChangedAttention { get; set; }
        public static bool isContextChangedMeditation{ get; set; }

    }
}
