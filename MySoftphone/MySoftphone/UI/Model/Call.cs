using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class Call
    {
        public string CallerName;

        public string PhoneNumber;

        public CallDirectionEnum Direction;

        public CallStateEnum CallState;

        public DateTime StartTime;

        public DateTime EndTime;

        public TimeSpan Duration;

        public Call()
        {
            this.CallerName = string.Empty;
            this.PhoneNumber = string.Empty;
            this.Direction = CallDirectionEnum.IncomingAudio;
            this.CallState = CallStateEnum.Ended;
            this.StartTime = new DateTime();
            this.EndTime = new DateTime();
            this.Duration = new TimeSpan(0);
        }

        public Call(string callerName,
            string phoneNumber, 
            CallDirectionEnum direction,
            CallStateEnum state)
        {
            this.CallerName = callerName;
            this.PhoneNumber = phoneNumber;
            this.Direction = direction;
            this.CallState = state;
            this.StartTime = DateTime.Now;
            this.EndTime = new DateTime();
            this.Duration = new TimeSpan(0);
        }

        public void CallEnded(CallStateEnum state)
        {
            this.EndTime = DateTime.Now;
            this.Duration = this.EndTime - this.StartTime;
            this.CallState = state;
        }
    }
}
