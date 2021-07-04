using Newtonsoft.Json;
using Ozeki.VoIP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class CallLog
    {
        private string dir = "JsonFiles";

        private string jsonFileName = "CallLog.json";

        private string filePath;

        private List<CallLogItem> callLogList { get; set; }

        private Dictionary<IPhoneCall, Call> callsRecorder { get; set; }

        public CallLog()
        {
            callsRecorder = new Dictionary<IPhoneCall, Call>();

            Directory.CreateDirectory(dir);

            filePath = Path.Combine(dir, jsonFileName);
            
            if (!File.Exists(filePath))
                File.WriteAllText(filePath, string.Empty);

            this.callLogList = this.DeserializeCallLog();
        }

        public List<CallLogItem> GetCallLog()
        {
            return this.callLogList;
        }

        public void AddToCallLog(IPhoneCall phoneCall, bool ended = false)
        {
            if (phoneCall == null)
                return;

            Call call = new Call(phoneCall);
            CallLogItem item = new CallLogItem(call);
            
            if(!ended)
                this.callsRecorder.Add(phoneCall, call);

            this.callLogList.Add(item);
            this.OrderCallLogs();
            this.SerializeCallLog();
        }

        public void ClearLogs()
        {
            this.callLogList.Clear();
            File.WriteAllText(filePath, string.Empty);
        }

        public void UpdateCallLogs(List<CallLogItem> list)
        {
            this.callLogList.Clear();
            this.callLogList.AddRange(list);
            this.SerializeCallLog();
        }

        private void SerializeCallLog()
        {
            this.OrderCallLogs();
            string jsonString = JsonConvert.SerializeObject(this.callLogList);
            File.WriteAllText(filePath, jsonString);
        }

        private List<CallLogItem> DeserializeCallLog()
        {
            string fileContent = File.ReadAllText(filePath);
            List<CallLogItem> jsonList =
                JsonConvert.DeserializeObject<List<CallLogItem>>(fileContent);

            if (jsonList == null)
                return new List<CallLogItem>();

            jsonList.OrderBy(c => c.StartTime.Year).ThenBy(c => c.StartTime.Month)
                .ThenBy(c => c.StartTime.Day).ThenBy(c => c.StartTime.Hour).ThenBy(c => c.StartTime.Minute);

            return jsonList;
        }

        private void OrderCallLogs()
        {
            this.callLogList.OrderBy(c => c.StartTime.Year).ThenBy(c => c.StartTime.Month)
                .ThenBy(c => c.StartTime.Day).ThenBy(c => c.StartTime.Hour).ThenBy(c => c.StartTime.Minute);
        }

        internal void CallEnded(IPhoneCall call)
        {
            KeyValuePair<IPhoneCall, Call> listCall = this.callsRecorder.Where(c => c.Key.PhoneLine == call.PhoneLine
            && c.Key.DialInfo.CallerDisplay == call.DialInfo.CallerDisplay
            && c.Key.CallID == call.CallID).FirstOrDefault();

            if (listCall.Value != null)
            {
                foreach( var callItem in this.callLogList)
                {
                    if (callItem.CallerName == listCall.Value.CallerName
                        && callItem.StartTime == listCall.Value.StartTime)
                    {
                        callItem.Type = call.CallState;
                        callItem.Duration = DateTime.Now - callItem.StartTime;
                        this.SerializeCallLog();
                        this.callsRecorder.Remove(listCall.Key);
                    }
                }
            }
        }
    }
}
