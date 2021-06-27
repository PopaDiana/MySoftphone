using Ozeki.VoIP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class CallLogItem
    {
        public CallLogItem(Call call)
        {
            this.CallerName = call.CallerName;
            this.Type = call.CallState;
            this.Date = call.StartTime;
            this.Duration = call.Duration;
        }

        public string CallerName { get; set; }

        public CallState Type { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Duration { get; set; }

    }
}
