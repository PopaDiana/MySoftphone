using Ozeki.VoIP;

namespace MySoftphone.UI.Model
{
    internal class SIPAccountModel
    {
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }

        public string Password { get; set; }

        public TransportTypeEnum TransportType { get; set; }

        public string Domain { get; set; }

        public string SIPAccountAsString { get; set; }
    
        public SIPAccount SIPAccount { get; set; }

        public IPhoneLine PhoneLine { get; set; }

        public SIPAccountModel()
        {
            this.DisplayName = string.Empty;
            this.UserName = string.Empty;
            this.RegisterName = string.Empty;
            this.Password = string.Empty;
            this.Domain = string.Empty;
            this.TransportType = TransportTypeEnum.UDP;
            this.SIPAccountAsString = string.Empty;
        }

        public SIPAccountModel(string displayName,
            string userName,
            string registeredName,
            string password,
            string domain,
            TransportTypeEnum transport)
        {
            this.DisplayName = displayName;
            this.UserName = userName;
            this.RegisterName = registeredName;
            this.Password = password;
            this.Domain = domain;
            this.TransportType = transport;
            this.SIPAccountAsString = registeredName + "@" + domain;
            this.SIPAccount = new SIPAccount(true, this.DisplayName, this.UserName, this.RegisterName, this.Password, this.Domain);
        }

        internal bool IsValid()
        {
            bool valid = true;

            if (string.IsNullOrEmpty(this.DisplayName) ||
                string.IsNullOrEmpty(this.UserName) ||
                string.IsNullOrEmpty(this.RegisterName) ||
                string.IsNullOrEmpty(this.Password) ||
                string.IsNullOrEmpty(this.Domain))
                valid = false;
            //validate password and domain
            return valid;
        }
    }
}