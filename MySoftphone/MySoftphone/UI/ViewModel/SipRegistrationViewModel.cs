using MySoftphone.MVVM;
using MySoftphone.UI.Model;
using System;
using System.Collections.ObjectModel;

namespace MySoftphone.UI.ViewModel
{
    internal class SipRegistrationViewModel : ObservableObject
    {
        #region Private Fields

        private TransportTypeEnum selectedTransportType;
        private string displayName = string.Empty;
        private string userName = string.Empty;
        private string registerName = string.Empty;
        private string password = string.Empty;
        private string domain = string.Empty;
        private TransportTypeEnum transportType;

        #endregion Private Fields

        #region Properties

        public SoftphoneManager SoftphoneManager { get; }

        public ObservableCollection<TransportTypeEnum> TransportTypes { get; set; }

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

        #endregion Properties

        #region Constructors

        public SipRegistrationViewModel(SoftphoneManager softphoneManager)
        {
            this.SoftphoneManager = softphoneManager;
            this.TransportTypes = new ObservableCollection<TransportTypeEnum>() { TransportTypeEnum.TCP, TransportTypeEnum.UDP, TransportTypeEnum.TLS };
            this.SelectedTransportType = this.TransportTypes[0];

            RegisterButtonPressed = new RelayCommand(a => this.Register());
            UnregisterButtonPressed = new RelayCommand(a => this.Unregister());
            RemoveButtonPressed = new RelayCommand(a => this.Remove());
            this.SaveButtonPressed = new RelayCommand(a => this.Save());
        }

        #endregion Constructors

        #region Relay Commands

        public RelayCommand RegisterButtonPressed { get; set; }

        public RelayCommand UnregisterButtonPressed { get; set; }

        public RelayCommand RemoveButtonPressed { get; set; }

        public RelayCommand SaveButtonPressed { get; set; }

        #endregion Relay Commands

        #region Private Methods

        private void Save()
        {
            SIPAccountModel account = new SIPAccountModel(this.DisplayName, this.UserName,
                this.RegisterName, this.Password, this.Domain, this.TransportType);

            if (account.IsValid())
            {
                this.SoftphoneManager.SaveSIPAccount(account, this.SelectedTransportType);
            }

            this.UserName = string.Empty;
            this.DisplayName = string.Empty;
            this.Password = string.Empty;
            this.RegisterName = string.Empty;
            this.SelectedTransportType = this.TransportTypes[0];
            this.Domain = string.Empty;
        }

        private void Remove()
        {
            try
            {
                this.SoftphoneManager.RemoveSipAccount();
            }
            catch (Exception e)
            {
            }
        }

        private void Unregister()
        {
            try
            {
                this.SoftphoneManager.UnRegisterPhoneLine();
            }
            catch (Exception e)
            {
            }
        }

        private void Register()
        {
            try
            {
                this.SoftphoneManager.RegisterPhoneLine();
            }
            catch (Exception e)
            {
            }
        }

        #endregion Private Methods
    }
}