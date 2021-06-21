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
    class RegisteredSIPAccounts
    {
        private List<SIPAccount> sIPAccounts;

        private string jsonFileName = "SIPAccounts.json";

        private string dir = "JsonFiles";

        private string filePath;
        public RegisteredSIPAccounts()
        {
            Directory.CreateDirectory(dir);

            filePath = Path.Combine(dir, jsonFileName);

            if (!File.Exists(filePath))
                File.WriteAllText(filePath, string.Empty);

            this.sIPAccounts = this.DeserializeAccountsFromFile();
        }

        public void Add(SIPAccount sIPAccount)
        {
            if(!this.sIPAccounts.Contains(sIPAccount))
            {
                this.sIPAccounts.Add(sIPAccount);
            }

            this.SerializeSIPAcounts();
        }

        public List<string> GetRegisteredAccountsAsString()
        {
            if (this.sIPAccounts == null)
                return new List<string>();

            List<string> stringAcc = new List<string>();

            foreach (var a in this.sIPAccounts)
            {
                stringAcc.Add(a.RegisterName + "@" + a.Domain);
            }

            return stringAcc;
        }

        private void SerializeSIPAcounts()
        {
            string jsonString = JsonConvert.SerializeObject(this.sIPAccounts);
            File.WriteAllText(filePath, jsonString);
        }

        private List<SIPAccount> DeserializeAccountsFromFile()
        {
            string fileContent = File.ReadAllText(filePath);
            List<SIPAccount> jsonList =
                JsonConvert.DeserializeObject<List<SIPAccount>>(fileContent);

            if (jsonList == null)
                return new List<SIPAccount>();

            return jsonList;
        }
    }
}
