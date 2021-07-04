using System;
using System.Text;

namespace MySoftphone.UI.ViewModel
{
    internal class LogViewModel : ObservableObject
    {
        private StringBuilder logText;

        public StringBuilder LogText
        {
            get
            {
                return this.logText;
            }
            set
            {
                this.logText = value;
                OnPropertyChanged("LogText");
            }
        }

        public LogViewModel()
        {
            this.LogText = new StringBuilder();
        }

        public void LogMessage(string message)
        {
            this.LogText.AppendLine(DateTime.Now.ToString() + " : " + message);
        }
    }
}