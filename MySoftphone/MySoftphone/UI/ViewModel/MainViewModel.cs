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

        public SoftphoneManager SoftphoneManager { get; set; }

        public RelayCommand SipRegViewCommand { get; set; }

        public RelayCommand AudioCallViewCommand { get; set; }

        public RelayCommand SomeCommand
        {
            get
            {
                if (someCommand == null)
                {
                    someCommand = new RelayCommand(param => this.DragWindowOnMouseDrag(), null);
                }

                return someCommand;
            }
        }

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
        }

        #endregion Constructor

        #region Private methods

        private void DragWindowOnMouseDrag()
        {
            throw new NotImplementedException();
        }

        #endregion Private methods
    }
}