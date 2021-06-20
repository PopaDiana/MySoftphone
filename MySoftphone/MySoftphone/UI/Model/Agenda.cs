using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class Agenda
    {
        private string jsonFile = "Agenda.json";
        private List<Caller> callersList {get;set;}

        public Agenda()
        {
            this.callersList = this.DeserializeAgenda();
        }

        public List<Caller> GetAgenda()
        {
            return this.callersList;
        }

        public void AddToAgenda(Caller caller)
        {
            this.callersList.Add(caller);
            this.callersList.Add(caller);
            this.callersList.OrderBy(c => c.Name);

            this.SerializeAgenda();
        }

        private void SerializeAgenda()
        {
            string jsonString = JsonConvert.SerializeObject(this.callersList);
            File.WriteAllText(jsonFile, jsonString);
        }

        private List<Caller> DeserializeAgenda()
        {
            string fileContent = File.ReadAllText(jsonFile);
            List<Caller> jsonList = 
                JsonConvert.DeserializeObject<List<Caller>>(fileContent);

            if (jsonList == null)
                return new List<Caller>();

            return jsonList;
        }
    }
}
