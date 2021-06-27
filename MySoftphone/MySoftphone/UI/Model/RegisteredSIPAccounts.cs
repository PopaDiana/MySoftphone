using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MySoftphone.UI.Model
{
    internal class RegisteredSIPAccounts
    {
        private List<SIPAccountModel> sIPAccounts;

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

        public List<SIPAccountModel> GetSipAccounts()
        {
            return this.sIPAccounts;
        }

        public void Add(SIPAccountModel sIPAccount)
        {
            if (!this.sIPAccounts.Contains(sIPAccount))
            {
                this.sIPAccounts.Add(sIPAccount);
            }

            this.SerializeSIPAcounts();
        }

        public List<string> GetRegisteredAccountsAsString()
        {
            if (this.sIPAccounts == null)
                return new List<string>();

            List<string> stringAcc = this.sIPAccounts.Select(a => a.SIPAccountAsString).ToList();

            return stringAcc;
        }

        private void SerializeSIPAcounts()
        {
            string jsonString = JsonConvert.SerializeObject(this.sIPAccounts);
            File.WriteAllText(filePath, jsonString);
        }

        private List<SIPAccountModel> DeserializeAccountsFromFile()
        {
            string fileContent = File.ReadAllText(filePath);
            List<SIPAccountModel> jsonList =
                JsonConvert.DeserializeObject<List<SIPAccountModel>>(fileContent);

            if (jsonList == null)
                return new List<SIPAccountModel>();

            return jsonList;
        }

        internal void Remove(string selectedSIPAccount)
        {
            SIPAccountModel account = this.sIPAccounts.Where(a => a.SIPAccountAsString.Equals(selectedSIPAccount)).FirstOrDefault();
            if (account != null)
            {
                this.sIPAccounts.Remove(account);
                this.SerializeSIPAcounts();
            }
        }
    }
}