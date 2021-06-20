using MySoftphone.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.ViewModel
{
    class MainViewModel : ObservableObject
    {
        #region Private fields
        private object currentView;

        private RelayCommand someCommand;
        #endregion

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

        public RelayCommand SipRegViewCommand { get; set; }

        public RelayCommand AudioCallViewCommand { get; set; }

        public RelayCommand SomeCommand 
        { 
            get
            {
                if(someCommand == null)
                {
                    someCommand = new RelayCommand(param => this.DragWindowOnMouseDrag(), null);
                }

                return someCommand;
            }
        }

        #endregion

        #region Constructor
        public MainViewModel()
        {
            SipRegVM = new SipRegistrationViewModel();
            AudioCallVM = new AudioCallViewModel();
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
        #endregion

        #region Private methods

        private void DragWindowOnMouseDrag()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
