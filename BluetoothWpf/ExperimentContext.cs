using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWpf
{
    static class ExperimentContext
    {
        public static string CurrentContext 
        { get; set; }

        public static string TestSubjectName
        { get; set; }

        public static DateTime LastExperimentBeginTime { get; set; }

    }
}
