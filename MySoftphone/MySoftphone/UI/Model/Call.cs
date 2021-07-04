using Ozeki.Network;
using Ozeki.VoIP;
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

        public CallState CallState;

        public DateTime StartTime;

        public DateTime EndTime;

        public TimeSpan Duration;

        public IPhoneCall PhoneCall;

        public Call()
        {
            this.CallerName = string.Empty;
            this.PhoneNumber = string.Empty;
            this.Direction = CallDirectionEnum.IncomingAudio;
            this.CallState = CallState.Completed;
            this.StartTime = new DateTime();
            this.EndTime = new DateTime();
            this.Duration = new TimeSpan(0);
        }

        public Call(IPhoneCall phoneCall)
        {
            this.PhoneCall = phoneCall;

            var account = phoneCall.PhoneLine.SIPAccount.AsSIPAddress(phoneCall.PhoneLine.Config.TransportType);
            DialInfo caller = new DialInfo(account);
            if (!this.PhoneCall.IsIncoming)
            {
                this.CallerName = phoneCall.OtherParty.ToString();
                this.PhoneNumber = phoneCall.DialInfo.Dialed;
            }
            else
            {
                this.CallerName = phoneCall.OtherParty.DisplayName;
                this.PhoneNumber = phoneCall.OtherParty.UserName;
            }
            
            this.Direction = this.GetCallDirection(phoneCall.CallType, phoneCall.IsIncoming);
            this.CallState = phoneCall.CallState;
            this.StartTime = DateTime.Now;
            this.EndTime = new DateTime();
            this.Duration = new TimeSpan(0);
        }

        public void CallEnded(CallState state)
        {
            this.EndTime = DateTime.Now;
            this.Duration = this.EndTime - this.StartTime;
            this.CallState = state;
        }

        private CallDirectionEnum GetCallDirection(CallType callType, bool isIncoming)
        {
            switch (callType)
            {
                case CallType.Audio:
                    if (isIncoming)
                        return CallDirectionEnum.IncomingAudio;
                    else
                        return CallDirectionEnum.OutgoingAudio;
                case CallType.AudioVideo:
                case CallType.Video:
                    if (isIncoming)
                        return CallDirectionEnum.IncomingVideo;
                    else return CallDirectionEnum.OutgoingVideo;
                default:
                    return CallDirectionEnum.IncomingAudio;
            }
        }
    }
}
