using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;



namespace BluetoothWpf
{
    //В дальнейшем можно создать поток логера - выводить логи в любой вывод
    public class LbLoger : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Mutex _queueMutex;

        private Queue<string> _queueOfMessages;
        public LbLoger()
        {
            _queueMutex = new Mutex();
            _queueOfMessages = new Queue<string>();
        }
        private string GetDateWithLogFormat()
        {
            string logString = String.Format("[{0:s}]: ", DateTime.Now.ToString());
            return logString;
        }

        public void Print(string message)
        {
            _queueMutex.WaitOne();
                _queueOfMessages.Enqueue(GetDateWithLogFormat() + message);
                OnPropertyChanged("Print");
            _queueMutex.ReleaseMutex();
        }

        //На вход подавать пустой список
        public bool Flush(ref List<string> outputLogMessages)
        {
            if (_queueOfMessages.Count == 0)
                return false;

            while (_queueOfMessages.Count > 0)
            {
                _queueMutex.WaitOne(100);
                    string outputLogString = _queueOfMessages.Dequeue();
                _queueMutex.ReleaseMutex();

                outputLogMessages.Add(outputLogString);
            }

            return true;

        }

        ///////////////////////////////////////////////////////////////////////
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
//