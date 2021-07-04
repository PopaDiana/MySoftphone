using MySoftphone.MVVM;
using MySoftphone.UI.Model;
using System;

namespace MySoftphone.UI.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        #region Private fields

        private object currentView;

        private RelayCommand someCommand;

        private string userName;

        #endregion Private fields

        #region Properties

        public SipRegistrationViewModel SipRegVM { get; set; }

        public AudioCallViewModel AudioCallVM { get; set; }

        public LogViewModel LogVM { get; set; }

        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChanged();
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

        public SoftphoneManager SoftphoneManager { get; set; }

        public RelayCommand SipRegViewCommand { get; set; }

        public RelayCommand AudioCallViewCommand { get; set; }

        public RelayCommand LogViewCommand { get; set; }

        #endregion Properties

        #region Constructor

        public MainViewModel()
        {
            this.LogVM = new LogViewModel();
            this.LogVM.LogMessage("Application started");
            this.SoftphoneManager = new SoftphoneManager(this.LogVM);
            this.SipRegVM = new SipRegistrationViewModel(this.SoftphoneManager, this.LogVM);
            this.AudioCallVM = new AudioCallViewModel(this.SoftphoneManager, this.LogVM);
            this.CurrentView = SipRegVM;

            SipRegViewCommand = new RelayCommand(o =>
            {
                CurrentView = SipRegVM;
            });

            AudioCallViewCommand = new RelayCommand(o =>
            {
                CurrentView = AudioCallVM;
            });

            LogViewCommand = new RelayCommand(o =>
            {
                CurrentView = LogVM;
            });

            if(this.SoftphoneManager.RegisteredSIPAccounts.Count > 0)
            {
                this.UserName = this.SoftphoneManager.SIPAccounts.GetSipAccounts()[0].DisplayName;
            }
            else
            {
                this.UserName = String.Empty;
            }
        }

        #endregion Constructor
    }
}