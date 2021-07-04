using Ozeki.VoIP;
using System;

namespace MySoftphone.UI.Model
{
    class CallLogItem
    {
        public CallLogItem(Call call)
        {
            this.CallerName = call.CallerName;
            this.Type = call.CallState;
            this.StartTime = call.StartTime;
            this.Duration = call.Duration;
        }

        public string CallerName { get; set; }

        public CallState Type { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

    }
}
