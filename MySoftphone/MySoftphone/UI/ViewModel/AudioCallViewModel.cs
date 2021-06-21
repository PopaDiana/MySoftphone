using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySoftphone.MVVM;
using MySoftphone.UI.Model;

namespace MySoftphone.UI.ViewModel
{
    class AudioCallViewModel : ObservableObject
    {
        #region Private Fields
        private string typedPhoneNumber;

        private Agenda agenda;

        private CallLog callLog;

        private ObservableCollection<Caller> agendaItems;

        private Caller selectedAgendaEntry;

        private ObservableCollection<CallLogItem> callLogItems;

        private CallLogItem selectedLogEntry;
        #endregion

        #region Properties
        public string TypedPhoneNumber
        {
            get
            {
                return this.typedPhoneNumber;
            }
            set
            {
                this.typedPhoneNumber = value;
                OnPropertyChanged("TypedPhoneNumber");
            }
        }

        public ObservableCollection<CallLogItem> CallLogItems
        {
            get
            {
                return this.callLogItems;
            }
            set
            {
                this.callLogItems = value;
                OnPropertyChanged("CallLogItems");
            }
        }

        public ObservableCollection<Caller> AgendaItems
        {
            get
            {
                return this.agendaItems;
            }
            set
            {
                this.agendaItems = value;
                OnPropertyChanged("AgendaItems");
            }
        }

        public Caller SelectedAgendaEntry
        {
            get
            {
                return this.selectedAgendaEntry;
            }
            set
            {
                this.selectedAgendaEntry = value;
                OnPropertyChanged("SelectedAgendaEntry");
            }
        }

        public CallLogItem SelectedLogEntry
        {
            get
            {
                return this.selectedLogEntry;
            }
            set
            {
                this.selectedLogEntry = value;
                OnPropertyChanged("SelectedLogEntry");
            }
        }
        #endregion

        #region Constructors
        public AudioCallViewModel()
        {
            agenda = new Agenda();
            AgendaItems =
            new ObservableCollection<Caller>(agenda.GetAgenda());
            //new ObservableCollection<Caller>()
            //{
            //    new Caller("Dia","098"),
            //    new Caller("Ana","990"),
            //    new Caller("John", "0744678"),
            //    new Caller("Mike", "009537")
            //};

            callLog = new CallLog();
            CallLogItems = new ObservableCollection<CallLogItem>()
            {
                new CallLogItem(new Call("Dia", "074" ,CallDirectionEnum.IncomingAudio, CallStateEnum.Lost)),
                new CallLogItem(new Call("Xyz", "989",CallDirectionEnum.OutgoingVideo, CallStateEnum.Ongoing)),
                new CallLogItem(new Call("Jack", "870",CallDirectionEnum.IncomingVideo, CallStateEnum.Rejected))
            };

            //new ObservableCollection<CallLogItem> ( callLog.GetCallLog());

            this.TypedPhoneNumber = string.Empty;

            DialpadButtonPressed = new RelayCommand(param => this.AppendToPhoneNumber(param));
            OnKeyDownDialpad = new RelayCommand(param => this.KeyDownDialpad(param));
            OnAudioCallPressed = new RelayCommand(param => this.AudioCallPressed());
            OnVidoCallPressed = new RelayCommand(param => this.VideoCallPressed());
            OnSaveToAgendaPressed = new RelayCommand(param => this.SaveToAgendaPressed());
            OnPickUpPressed = new RelayCommand(a => this.PickUpPressed());
            OnHangUpPressed = new RelayCommand(a => this.HangUpPressed());
            OnRejectPressed = new RelayCommand(a => this.RejectPressed());
            OnTransferPressed = new RelayCommand(a => this.TransferPressed());
            OnDialPressed = new RelayCommand(a => this.DialPressed());
            OnDeleteContactPressed = new RelayCommand(a => this.DeleteContactPressed());
            OnClearLogsPressed = new RelayCommand(a => this.ClearLogsPressed());
        }
        #endregion

        #region RelayCommands
        public RelayCommand DialpadButtonPressed { get; set; }

        public RelayCommand OnKeyDownDialpad { get; set; }

        public RelayCommand OnAudioCallPressed { get; set; }

        public RelayCommand OnVidoCallPressed { get; set; }

        public RelayCommand OnSaveToAgendaPressed { get; set; }

        public RelayCommand OnPickUpPressed { get; set; }

        public RelayCommand OnHangUpPressed { get; set; }

        public RelayCommand OnRejectPressed { get; set; }

        public RelayCommand OnTransferPressed { get; set; }

        public RelayCommand OnDialPressed { get; set; }

        public RelayCommand OnDeleteContactPressed { get; set; }

        public RelayCommand OnClearLogsPressed { get; set; }
        #endregion

        #region Private Methods
        private void AppendToPhoneNumber(object number)
        {
            if (number!=null)
            {
                this.TypedPhoneNumber += ((string)number);
            }
        }

        private void KeyDownDialpad(object param)
        {
            if (param is System.Windows.Controls.TextBox textbox)
            {
                this.TypedPhoneNumber += textbox.Text;
            }
        }

        private void SaveToAgendaPressed()
        {
            //check if number valid
            //save do agenda file
        }

        private void VideoCallPressed()
        {
            //throw new NotImplementedException();
        }

        private void AudioCallPressed()
        {
            //throw new NotImplementedException();
        }

        private void TransferPressed()
        {
            //throw new NotImplementedException();
        }

        private void RejectPressed()
        {
            //throw new NotImplementedException();
        }

        private void HangUpPressed()
        {
            //throw new NotImplementedException();
        }

        private void PickUpPressed()
        {
            //throw new NotImplementedException();
        }

        private void DeleteContactPressed()
        {
            if(this.SelectedAgendaEntry != null && this.AgendaItems != null)
            {
                this.AgendaItems.Remove(this.SelectedAgendaEntry);
                this.SelectedAgendaEntry = null;
                this.agenda.UpdateAgenda(this.AgendaItems.ToList());
            } 
        }

        private void DialPressed()
        {
            //throw new NotImplementedException();
        }

        private void ClearLogsPressed()
        {
            if(this.CallLogItems != null)
            {
                this.CallLogItems.Clear();
                this.callLog.UpdateCallLogs(this.CallLogItems.ToList());
            }
        }
        #endregion
    }
}
