using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;



namespace BluetoothWpf
{
    //В дальнейшем можно создать поток логера - выводить логи в любой вывод
    public class LbLoger
    {
        private ListBox logerListBox;

        public LbLoger(ref ListBox listBox)
        {
            logerListBox = listBox;
        }
        private string GetDateWithLogFormat()
        {
            string logString = String.Format("[{0:s}]: ", DateTime.Now.ToString());
            return logString;
        }

        public void Print(string message)
        {
            string logString = GetDateWithLogFormat() + message;
            logerListBox.Items.Add(logString);
            logerListBox.ItemsSource = logerListBox.ItemsSource;
        }

    }
}
//