using MySoftphone.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.Model
{
    class SIPAccount
    {
        public string DisplayName { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; }

        public string Password { get; set; }

        public TransportTypeEnum TransportType { get; set; }

        public string Domain { get; set; }

        public SIPAccount()
        {
            this.DisplayName = string.Empty;
            this.UserName = string.Empty;
            this.RegisterName = string.Empty;
            this.Password = string.Empty;
            this.Domain = string.Empty;
            this.TransportType = TransportTypeEnum.UDP;
        }

        public SIPAccount(string displayName,
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
        }
    }
}
