using MySoftphone.MVVM;
using MySoftphone.UI.Model;
using Ozeki.Media;
using Ozeki.VoIP;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MySoftphone.UI.ViewModel
{
    internal class AudioCallViewModel : ObservableObject
    {
        #region Private Fields

        private string typedPhoneNumber;

        private Agenda agenda;

        private ObservableCollection<Caller> agendaItems;

        private Caller selectedAgendaEntry;

        private CallLogItem selectedLogEntry;

        #endregion Private Fields

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

        public SoftphoneManager SoftphoneManager { get; }

        #endregion Properties

        #region Constructors

        public AudioCallViewModel(SoftphoneManager softphoneManager)
        {
            this.SoftphoneManager = softphoneManager;
            agenda = new Agenda();
            AgendaItems = new ObservableCollection<Caller>(agenda.GetAgenda());

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
            OnDialPressedA = new RelayCommand(a => this.DialAudioPressed());
            OnDialPressedV = new RelayCommand(a => this.DialVideoPressed());
            OnDeleteContactPressed = new RelayCommand(a => this.DeleteContactPressed());
            OnClearLogsPressed = new RelayCommand(a => this.ClearLogsPressed());
        }

        #endregion Constructors

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

        public RelayCommand OnDialPressedA { get; set; }

        public RelayCommand OnDialPressedV { get; set; }

        public RelayCommand OnDeleteContactPressed { get; set; }

        public RelayCommand OnClearLogsPressed { get; set; }

        #endregion RelayCommands

        #region Private Methods

        private void AppendToPhoneNumber(object number)
        {
            if (number != null)
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
            try
            {
                if (!string.IsNullOrEmpty(this.TypedPhoneNumber))
                {
                    this.SoftphoneManager.Call(CallType.AudioVideo, this.TypedPhoneNumber);
                    this.TypedPhoneNumber = string.Empty;
                }
            }
            catch (Exception e)
            {
            }
        }

        private void AudioCallPressed()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.TypedPhoneNumber))
                {
                    this.SoftphoneManager.Call(CallType.Audio, this.TypedPhoneNumber);
                }
            }
            catch (Exception e)
            {
            }
        }

        private void TransferPressed()
        {
        }

        private void RejectPressed()
        {
            try
            {
                this.SoftphoneManager.RejectCall();
            }
            catch (Exception e)
            {
            }
        }

        private void HangUpPressed()
        {
            try
            {
                this.SoftphoneManager.HangUpCall();
            }
            catch (Exception e)
            {
            }
        }

        private void PickUpPressed()
        {
            try
            {
                this.SoftphoneManager.PickUpCall();
            }
            catch (Exception e)
            {
            }
        }

        private void DeleteContactPressed()
        {
            if (this.SelectedAgendaEntry != null && this.AgendaItems != null)
            {
                this.AgendaItems.Remove(this.SelectedAgendaEntry);
                this.SelectedAgendaEntry = null;
                this.agenda.UpdateAgenda(this.AgendaItems.ToList());
            }
        }

        private void DialAudioPressed()
        {
            try
            {
                if (this.SelectedAgendaEntry != null && !string.IsNullOrEmpty(this.SelectedAgendaEntry.PhoneNumber))
                {
                    this.SoftphoneManager.Call(CallType.Audio, this.SelectedAgendaEntry.PhoneNumber);
                }
            }
            catch (Exception e)
            {
            }
        }

        private void DialVideoPressed()
        {
            try
            {
                if (this.SelectedAgendaEntry != null && !string.IsNullOrEmpty(this.SelectedAgendaEntry.PhoneNumber))
                {
                    this.SoftphoneManager.Call(CallType.AudioVideo, this.SelectedAgendaEntry.PhoneNumber);
                }
            }
            catch (Exception e)
            {
            }
        }

        private void ClearLogsPressed()
        {
            this.SoftphoneManager.ClearCallLog();
        }

        #endregion Private Methods
    }
}