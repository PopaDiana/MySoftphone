using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class Agenda
    {
        private string jsonFileName = "Agenda.json";

        private string dir = "JsonFiles";

        private string filePath;

        private List<Caller> callersList { get; set; }

        public Agenda()
        {
            Directory.CreateDirectory(dir);

            filePath = Path.Combine(dir, jsonFileName);

            if (!File.Exists(filePath))
                File.WriteAllText(filePath, string.Empty);

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

        public void DeleteFromAgenda(Caller caller)
        {
            if (this.callersList != null)
            {
                if (this.callersList.Contains(caller))
                {
                    this.callersList.Remove(caller);
                    this.SerializeAgenda();
                }
            }
        }

        internal void UpdateAgenda(List<Caller> agendaItems)
        {
            this.callersList.Clear();
            this.callersList.AddRange(agendaItems);
            this.SerializeAgenda();
        }

        private void SerializeAgenda()
        {
            this.callersList.OrderBy(c => c.Name);
            string jsonString = JsonConvert.SerializeObject(this.callersList);
            File.WriteAllText(filePath, jsonString);
        }

        private List<Caller> DeserializeAgenda()
        {
            string fileContent = File.ReadAllText(filePath);
            List<Caller> jsonList =
                JsonConvert.DeserializeObject<List<Caller>>(fileContent);

            if (jsonList == null)
                return new List<Caller>();

            jsonList.OrderBy(c => c.Name);
            return jsonList;
        }
    }
}
