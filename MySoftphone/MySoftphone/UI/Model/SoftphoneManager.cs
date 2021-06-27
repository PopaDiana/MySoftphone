using MySoftphone.UI.ViewModel;
using Ozeki.Media;
using Ozeki.Network;
using Ozeki.VoIP;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MySoftphone.UI.Model
{
    internal class SoftphoneManager : ObservableObject
    {
        private const int MinPort = 20000;
        private const int MaxPort = 20500;

        private ISoftPhone softPhone;
        private ObservableCollection<string> registeredSIPAccounts;
        private string selectedSIPAccount = string.Empty;
        private object lockObj;
        private CallLog callLog;
        private ObservableCollection<CallLogItem> callLogItems;
        private ObservableCollection<IPhoneCall> activePhoneCalls;
        private IPhoneCall selectedPhoneCall;

        #region Public Properties
        public MediaHandlers MediaHandlers { get; private set; }

        public RegisteredSIPAccounts SIPAccounts { get; set; }

        public string SelectedSIPAccount
        {
            get
            {
                return this.selectedSIPAccount;
            }
            set
            {
                this.selectedSIPAccount = value;
                OnPropertyChanged("SelectedSIPAccount");
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

        public ObservableCollection<IPhoneCall> ActivePhoneCalls
        {
            get
            {
                return this.activePhoneCalls;
            }
            set
            {
                this.activePhoneCalls = value;
                OnPropertyChanged("ActivePhoneCalls");
            }
        }

        public IPhoneCall SelectedPhoneCall
        {
            get
            {
                return this.selectedPhoneCall;
            }
            set
            {
                this.selectedPhoneCall = value;
                OnPropertyChanged("SelectedPhoneCall");
            }
        }

        #endregion Public Properties

        #region Constructor

        public SoftphoneManager()
        {
            this.SIPAccounts = new RegisteredSIPAccounts();
            this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());

            if (this.RegisteredSIPAccounts.Count > 0)
            {
                this.SelectedSIPAccount = this.RegisteredSIPAccounts[0];
                //this.LineStatus = this.SIPAccounts.GetAccountStatus(this.SelectedSIPAccount);
            }

            this.callLog = new CallLog();
            CallLogItems = new ObservableCollection<CallLogItem>()
            {
                new CallLogItem(new Call("Dia", "074" ,CallDirectionEnum.IncomingAudio, CallState.Error)),
                new CallLogItem(new Call("Xyz", "989",CallDirectionEnum.OutgoingVideo, CallState.InCall)),
                new CallLogItem(new Call("Jack", "870",CallDirectionEnum.IncomingVideo, CallState.Rejected))
            };

            //new ObservableCollection<CallLogItem> ( callLog.GetCallLog());

            this.InitiateSoftphone();
        }

        #endregion Constructor

        #region Public Methods

        public void SaveSIPAccount(SIPAccountModel account, TransportTypeEnum selectedTransportType)
        {
            var lineConfig = new PhoneLineConfiguration(account.SIPAccount);
            lineConfig.TransportType = selectedTransportType == TransportTypeEnum.TCP ? Ozeki.Network.TransportType.Tcp
                : (selectedTransportType == TransportTypeEnum.TLS ? Ozeki.Network.TransportType.Tls : Ozeki.Network.TransportType.Udp);
            lineConfig.NatConfig = new NatConfiguration(NatTraversalMethod.STUN, "", true); // stun server address
            lineConfig.SRTPMode = Ozeki.Common.SRTPMode.None;

            account.PhoneLine = softPhone.CreatePhoneLine(lineConfig);
            this.SubscribeToLine(account.PhoneLine);

            this.SIPAccounts.Add(account);
            this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());
        }

        public void RemoveSipAccount()
        {
            if (!string.IsNullOrEmpty(this.SelectedSIPAccount))
            {
                this.SIPAccounts.Remove(this.SelectedSIPAccount);
                this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());

                if (this.RegisteredSIPAccounts.Count > 0)
                {
                    this.SelectedSIPAccount = this.RegisteredSIPAccounts[0];
                }
            }
        }

        public void ClearCallLog()
        {
            if (this.CallLogItems != null)
            {
                this.CallLogItems.Clear();
                this.callLog.UpdateCallLogs(this.CallLogItems.ToList());
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void InitiateSoftphone()
        {
            //softphone should not exist on initialization
            //if (this.softPhone != null)
            //{
            //    // unregister the phone lines
            //    foreach (var account in this.SIPAccounts.GetSipAccounts())
            //    {
            //        if (account.PhoneLine.RegState == RegState.RegistrationSucceeded)
            //            softPhone.UnregisterPhoneLine(account.PhoneLine);
            //    }

            //    softPhone.IncomingCall -= (SoftPhone_IncomingCall);
            //    softPhone.Close();
            //}

            // create new softphone

            this.softPhone = SoftPhoneFactory.CreateSoftPhone(MinPort, MaxPort);

            this.softPhone.IncomingCall += SoftPhone_IncomingCall;
        }

        private void SoftPhone_IncomingCall(object sender, VoIPEventArgs<IPhoneCall> e)
        {
            IPhoneCall phoneCall = e.Item;

            lock (lockObj)
            {
                SubscribeToCallEvents(phoneCall);

                if (phoneCall.CallState.IsCallEnded())
                {
                    this.callLog.AddToCallLog(phoneCall);
                    return;
                }

                this.ActivePhoneCalls.Add(phoneCall);

                if (this.SelectedPhoneCall == null)
                {
                    this.SelectedPhoneCall = phoneCall;
                    //MediaHandlers.AttachAudio(call);
                }
            }

            OnIncomingCall(phoneCall);
        }

        private void SubscribeToCallEvents(IPhoneCall phoneCall)
        {
            throw new NotImplementedException();
        }

        private void SubscribeToLine(IPhoneLine phoneLine)
        {
            if (phoneLine == null)
                return;

            //phoneLine.RegistrationStateChanged += Line_RegistrationStateChanged;
        }

        #endregion Private Methods

        #region Events

        public event EventHandler<GeneralEventArgs<IPhoneLine>> PhoneLineStateChanged;

        public event EventHandler<GeneralEventArgs<IPhoneCall>> IncomingCall;

        public event EventHandler<GeneralEventArgs<IPhoneCall>> PhoneCallStateChanged;

        private void OnIncomingCall(IPhoneCall call)
        {
            IncomingCall?.Invoke(this, new GeneralEventArgs<IPhoneCall>(call));
        }

        private void OnPhoneCallStateChanged(IPhoneCall call)
        {
            PhoneCallStateChanged?.Invoke(this, new GeneralEventArgs<IPhoneCall>(call));
        }

        private void OnPhoneLineStateChanged(IPhoneLine line)
        {
            PhoneLineStateChanged?.Invoke(this, new GeneralEventArgs<IPhoneLine>(line));
        }

        #endregion Events
    }
}