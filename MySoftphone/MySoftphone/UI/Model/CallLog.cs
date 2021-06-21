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
        private string dir = "JsonFiles";

        private string jsonFileName = "CallLog.json";

        private string filePath;

        private List<CallLogItem> callLogList { get; set; }

        public CallLog()
        {
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

        public void AddToAgenda(CallLogItem item)
        {
            this.callLogList.Add(item);
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

            jsonList.OrderBy(c => c.Date.Year).ThenBy(c => c.Date.Month)
                .ThenBy(c => c.Date.Day).ThenBy(c => c.Date.Hour).ThenBy(c => c.Date.Minute);

            return jsonList;
        }

        private void OrderCallLogs()
        {
            this.callLogList.OrderBy(c => c.Date.Year).ThenBy(c => c.Date.Month)
                .ThenBy(c => c.Date.Day).ThenBy(c => c.Date.Hour).ThenBy(c => c.Date.Minute);
        }
    }
}
