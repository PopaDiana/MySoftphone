using Newtonsoft.Json;
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
        private string jsonFile = "CallLog.json";
        private List<CallLogItem> callLogList { get; set; }

        public CallLog()
        {
            File.WriteAllText(jsonFile, "");

            this.callLogList = this.DeserializeCallLog();
        }

        public List<CallLogItem> GetCallLog()
        {
            return this.callLogList;
        }

        public void AddToAgenda(CallLogItem item)
        {
            this.callLogList.Add(item);
            this.callLogList.Add(item);
            this.callLogList.OrderBy(c => c.Date.Year).ThenBy(c=> c.Date.Month)
                .ThenBy(c=> c.Date.Day).ThenBy(c => c.Date.Hour).ThenBy(c => c.Date.Minute);

            this.SerializeCallLog();
        }

        private void SerializeCallLog()
        {
            string jsonString = JsonConvert.SerializeObject(this.callLogList);
            File.WriteAllText(jsonFile, jsonString);
        }

        private List<CallLogItem> DeserializeCallLog()
        {
            string fileContent = File.ReadAllText(jsonFile);
            List<CallLogItem> jsonList =
                JsonConvert.DeserializeObject<List<CallLogItem>>(fileContent);

            if (jsonList == null)
                return new List<CallLogItem>();

            return jsonList;
        }
    }
}
