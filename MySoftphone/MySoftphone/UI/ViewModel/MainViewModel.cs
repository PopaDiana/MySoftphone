using MySoftphone.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySoftphone.UI.ViewModel
{
    class MainViewModel : ObservableObject
    {
        private object currentView;

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

        public RelayCommand DragWindow { get; set; }

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
    }
}
