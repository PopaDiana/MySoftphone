using MySoftphone.MVVM;
using MySoftphone.UI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.ViewModel
{
    class SipRegistrationViewModel : ObservableObject
    {
        #region Private Fields
        private TransportTypeEnum selectedTransportType;
        private string lineStatus;
        private ObservableCollection<string> registeredSIPAccounts;
        private string displayName;
        private string userName;
        private string registerName;
        private string password;
        private string domain;
        private TransportTypeEnum transportType;
        #endregion

        #region Properties
        public ObservableCollection<TransportTypeEnum> TransportTypes { get; set; }

        public RegisteredSIPAccounts SIPAccounts { get; set; }

        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                OnPropertyChanged("UserName");
            }
        }

        public string RegisterName
        {
            get
            {
                return this.registerName;
            }
            set
            {
                this.registerName = value;
                OnPropertyChanged("RegisterName");
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
                OnPropertyChanged("Password");
            }
        }

        public TransportTypeEnum TransportType
        {
            get
            {
                return this.transportType;
            }
            set
            {
                this.transportType = value;
                OnPropertyChanged("TransportType");
            }
        }

        public string Domain
        {
            get
            {
                return this.domain;
            }
            set
            {
                this.domain = value;
                OnPropertyChanged("Domain");
            }
        }

        public ObservableCollection<string> RegisteredSIPAccounts
        {
            get
            {
                return this.registeredSIPAccounts;
            }
            set
            {
                this.registeredSIPAccounts = value;
                OnPropertyChanged("RegisteredSIPAccounts");
            }
        }

        public TransportTypeEnum SelectedTransportType
        {
            get
            {
                return this.selectedTransportType;
            }
            set
            {
                this.selectedTransportType = value;
                OnPropertyChanged("SelectedTransportType");
            }
        }

        public string LineStatus
        {
            get { return this.lineStatus; }
            set 
            {
                this.lineStatus = value;
                OnPropertyChanged("LineStatus");
            }
        }


        #endregion

        #region Constructors
        public SipRegistrationViewModel()
        {
            this.TransportTypes = new ObservableCollection<TransportTypeEnum>() { TransportTypeEnum.TCP, TransportTypeEnum.UDP, TransportTypeEnum.TLS};
            this.SelectedTransportType = this.TransportTypes[0];
            this.LineStatus = "Unknown";
            this.SIPAccounts = new RegisteredSIPAccounts();
            this.RegisteredSIPAccounts = new ObservableCollection<string>( this.SIPAccounts.GetRegisteredAccountsAsString());

            RegisterButtonPressed = new RelayCommand(a => this.Register());
            UnregisterButtonPressed = new RelayCommand(a => this.Uregister());
            RemoveButtonPressed = new RelayCommand(a => this.Remove());
            this.SaveButtonPressed = new RelayCommand(a => this.Save());
        }
        #endregion

        #region Relay Commands
        public RelayCommand RegisterButtonPressed { get; set; }

        public RelayCommand UnregisterButtonPressed { get; set; }

        public RelayCommand RemoveButtonPressed { get; set; }

        public RelayCommand SaveButtonPressed { get; set; }
        #endregion

        #region Private Methods

        private void Save()
        {
            this.SIPAccounts.Add(
                new SIPAccount(this.DisplayName,this.UserName,
                this.RegisterName, this.Password, this.Domain, this.TransportType));
            this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());
        }

        private void Remove()
        {
            //throw new NotImplementedException();
            //set LineStatus
        }

        private void Uregister()
        {
            //throw new NotImplementedException();
            //set LineStatus
        }

        private void Register()
        {
            //throw new NotImplementedException();
            //set LineStatus
        }
        #endregion
    }
}
