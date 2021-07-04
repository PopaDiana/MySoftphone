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

        #endregion Properties

        #region Constructor

        public MainViewModel()
        {
            this.SoftphoneManager = new SoftphoneManager();
            SipRegVM = new SipRegistrationViewModel(this.SoftphoneManager);
            AudioCallVM = new AudioCallViewModel(this.SoftphoneManager);
            CurrentView = SipRegVM;

            SipRegViewCommand = new RelayCommand(o =>
            {
                CurrentView = SipRegVM;
            });

            AudioCallViewCommand = new RelayCommand(o =>
            {
                CurrentView = AudioCallVM;
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