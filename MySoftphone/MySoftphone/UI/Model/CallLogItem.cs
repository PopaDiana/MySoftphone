using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class CallLogItem
    {
        public CallLogItem(string callerName, CallStateEnum type, DateTime date, TimeSpan duration)
        {
            this.CallerName = callerName;
            this.Type = type;
            this.Date = date;
            this.Duration = duration;
        }

        public CallLogItem(Call call)
        {
            this.CallerName = call.CallerName;
            this.Type = call.CallState;
            this.Date = call.StartTime;
            this.Duration = call.Duration;
        }

        public string CallerName { get; set; }

        public CallStateEnum Type { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Duration { get; set; }

    }
}
