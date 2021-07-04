using Ozeki.VoIP;
using System;

namespace MySoftphone.UI.Model
{
    class CallLogItem
    {
        public CallLogItem(Call call)
        {
            if(call == null)
            {
                this.CallerName = string.Empty;
                this.PhoneNumber = string.Empty;
                this.Type = CallState.Completed;
                this.StartTime = DateTime.Now;
                this.Duration = new TimeSpan(0);
            }
            else
            {
                this.CallerName = call.CallerName;
                this.PhoneNumber = call.PhoneNumber;
                this.Type = call.CallState;
                this.StartTime = call.StartTime;
                this.Duration = call.Duration;
            }
        }

        public string CallerName { get; set; }

        public CallState Type { get; set; }

        public DateTime StartTime { get; set; }

        public TimeSpan Duration { get; set; }

        public string PhoneNumber { get; set; }

    }
}
