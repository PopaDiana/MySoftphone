using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class CallLogItem
    {
        public CallLogItem(string callerName, CallDirectionEnum direction, DateTime date, TimeSpan duration)
        {
            this.CallerName = callerName;
            this.Direction = direction;
            this.Date = date;
            this.Duration = duration;
        }

        public string CallerName { get; set; }

        public CallDirectionEnum Direction { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Duration { get; set; }

    }
}
