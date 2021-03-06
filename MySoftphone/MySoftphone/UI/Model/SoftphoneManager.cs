using MySoftphone.UI.ViewModel;
using Ozeki.Media;
using Ozeki.Network;
using Ozeki.VoIP;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using utils;

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
        private CallLogItem selectedCallLogItem;
        private IPhoneCall selectedPhoneCall;
        private List<(IPhoneLine, ISIPSubscription)> linesSubscriptions;
        private string lineState;

        #region Public Properties

        public LogViewModel Log { get; }

        public MediaHandlers MediaHandlers { get; set; }

        public RegisteredSIPAccounts SIPAccounts { get; set; }

        public List<(IPhoneLine, SIPAccount)> PhoneLines { get; set; }

        public ObservableList<IPhoneCall> ActivePhoneCalls { get; private set; }

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

        public CallLogItem SelectedCallLogItem
        {
            get
            {
                return this.selectedCallLogItem;
            }
            set
            {
                this.selectedCallLogItem = value;
                OnPropertyChanged("SelectedCallLogItem");
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
                this.MediaHandlers.DetachAudio();
                this.MediaHandlers.DetachVideo();

                if (this.selectedPhoneCall != null && this.selectedPhoneCall.CallState.IsRemoteMediaCommunication())
                {
                    this.MediaHandlers.AttachAudio(this.selectedPhoneCall);
                    this.MediaHandlers.AttachVideo(this.selectedPhoneCall);
                }

                OnPropertyChanged("SelectedPhoneCall");
            }
        }

        public string LineState
        {
            get
            {
                return this.lineState;
            }
            set
            {
                this.lineState = value;
                OnPropertyChanged("LineState");
            }
        }

        #endregion Public Properties

        #region Constructor

        public SoftphoneManager(LogViewModel logVM)
        {
            this.Log = logVM;
            this.MediaHandlers = new MediaHandlers();
            this.ActivePhoneCalls = new ObservableList<IPhoneCall>();
            this.lockObj = new object();
            this.LineState = "";

            this.InitiateSoftphone();
            this.linesSubscriptions = new List<(IPhoneLine, ISIPSubscription)>();
            this.PhoneLines = new List<(IPhoneLine, SIPAccount)>();
            this.SIPAccounts = new RegisteredSIPAccounts();
            this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());

            if (this.RegisteredSIPAccounts.Count > 0)
            {
                this.SelectedSIPAccount = this.RegisteredSIPAccounts[0];
                this.PhoneLines = this.GetPhoneLines(this.SIPAccounts.GetSipAccounts());
                this.LineState = "Unknown";
            }

            this.callLog = new CallLog();
            this.CallLogItems = new ObservableCollection<CallLogItem>(callLog.GetCallLog());
            this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();

            this.EnableCodecs();
        }

        #endregion Constructor

        #region Public Methods

        public void UnRegisterPhoneLine()
        {
            IPhoneLine line = this.GetSelectedPhoneLine();
            if (line == null)
                return;

            var isips = this.linesSubscriptions.Where(x => x.Item1.Equals(line)).FirstOrDefault().Item2;

            if (isips == null)
                return;

            line.Subscription.Unsubscribe(isips);
            this.linesSubscriptions.Remove((line, isips));
            softPhone.UnregisterPhoneLine(line);

            this.Log.LogMessage("Unregistered from active line.");
        }

        public void Call(CallType callType, string phoneNr)
        {
            IPhoneLine line = this.GetSelectedPhoneLine();
            if (line != null)
            {
                if (!line.RegState.IsRegistered())
                {
                    this.Log.LogMessage("You are not registered to the selected phone line. Call was not initiated...");
                }
                else
                {
                    if (!string.IsNullOrEmpty(phoneNr))
                    {
                        var dialParams = new DialParameters(phoneNr);
                        dialParams.CallType = callType;

                        IPhoneCall call = softPhone.CreateCallObject(line, dialParams);
                        this.StartPhoneCall(call);

                        this.Log.LogMessage("Call was started");
                    }
                }
            }
            else
            {
                this.Log.LogMessage("No phone line was found...");
            }
        }

        public void TransferCall(string typedPhoneNumber)
        {
            if (this.SelectedPhoneCall == null)
                return;

            if (!this.SelectedPhoneCall.CallState.IsInCall())
                return;

            this.BlindTransferCall(typedPhoneNumber);

            this.Log.LogMessage("Call was blindly transfered to selected phone number");
        }

        public void RejectCall()
        {
            lock (lockObj)
            {
                if (this.SelectedPhoneCall == null)
                    return;

                this.SelectedPhoneCall.Reject();
                this.callLog.CallEnded(this.SelectedPhoneCall);
                this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
            }

            this.Log.LogMessage("Call was rejected");
        }

        public void HangUpCall()
        {
            lock (lockObj)
            {
                if (this.SelectedPhoneCall == null)
                    return;

                this.SelectedPhoneCall.HangUp();
                this.callLog.CallEnded(this.SelectedPhoneCall);
                this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
                this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();
            }

            this.Log.LogMessage("Call was hanged up");
        }

        public void PickUpCall()
        {
            lock (lockObj)
            {
                if (this.SelectedPhoneCall == null)
                    return;

                //this.SelectedPhoneCall.CallType == CallType.AudioVideo
                this.SelectedPhoneCall.Answer(CallType.AudioVideo);
                this.callLog.AddToCallLog(this.SelectedPhoneCall);
                this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
                this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();
            }
        }

        public void RegisterPhoneLine()
        {
            if (string.IsNullOrEmpty(this.SelectedSIPAccount))
                return;

            var line = this.PhoneLines.Where(x => this.SelectedSIPAccount.Contains(x.Item2.RegisterName)).FirstOrDefault().Item1;

            if (line != null)
            {
                softPhone.RegisterPhoneLine(line);
                this.Log.LogMessage("Registered to phone line");
            }
        }

        public void SaveSIPAccount(SIPAccountModel account, TransportTypeEnum selectedTransportType)
        {
            var lineConfig = this.CreateLineConfig(account, selectedTransportType);
            IPhoneLine phoneLine = softPhone.CreatePhoneLine(lineConfig);
            this.Log.LogMessage("Phone line created");
            this.SubscribeToLine(phoneLine);

            this.SIPAccounts.Add(account);
            this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());
            this.PhoneLines.Add((phoneLine, account.SIPAccount));
        }

        public void RemoveSipAccount()
        {
            this.UnRegisterPhoneLine();

            if (!string.IsNullOrEmpty(this.SelectedSIPAccount))
            {
                this.SIPAccounts.Remove(this.SelectedSIPAccount);
                this.RegisteredSIPAccounts = new ObservableCollection<string>(this.SIPAccounts.GetRegisteredAccountsAsString());
                this.LineState = string.Empty;
                this.Log.LogMessage("SIP account removed.");

                if (this.RegisteredSIPAccounts.Count > 0)
                {
                    this.SelectedSIPAccount = this.RegisteredSIPAccounts[0];
                    this.LineState = "Unknown";
                    this.Log.LogMessage("Selected phone line state is unknown");
                }
            }
        }

        public void ClearCallLog()
        {
            if (this.CallLogItems != null)
            {
                this.CallLogItems.Clear();
                this.callLog.UpdateCallLogs(this.CallLogItems.ToList());
                this.SelectedCallLogItem = null;

                this.Log.LogMessage("Call log cleared.");
            }
        }

        public void BlindTransferCall(string target)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(target))
                    return;

                if (this.SelectedPhoneCall == null)
                    return;

                this.SelectedPhoneCall.BlindTransfer(target);
            }
        }

        #endregion Public Methods

        #region Private Methods
        private void EnableCodecs()
        {
            IEnumerable<CodecInfo> videoCodecs = this.softPhone.Codecs.Where(c => c.MediaType == CodecMediaType.Video);
            foreach (var item in videoCodecs)
            {
                CodecInfo info = item as CodecInfo;
                if (info == null)
                    continue;

                this.softPhone.EnableCodec(info.PayloadType);
            }
        }

        private List<(IPhoneLine, SIPAccount)> GetPhoneLines(List<SIPAccountModel> accuntsList)
        {
            List<(IPhoneLine, SIPAccount)> list = new List<(IPhoneLine, SIPAccount)>();

            foreach (var acc in accuntsList)
            {
                var lineConfig = this.CreateLineConfig(acc, acc.TransportType);
                IPhoneLine phoneLine = softPhone.CreatePhoneLine(lineConfig);
                this.SubscribeToLine(phoneLine);

                if (!list.Contains((phoneLine, acc.SIPAccount)))
                    list.Add((phoneLine, acc.SIPAccount));
            }

            return list;
        }

        private PhoneLineConfiguration CreateLineConfig(SIPAccountModel account, TransportTypeEnum selectedTransportType)
        {
            var lineConfig = new PhoneLineConfiguration(account.SIPAccount);
            lineConfig.TransportType = selectedTransportType == TransportTypeEnum.TCP ? Ozeki.Network.TransportType.Tcp
                : (selectedTransportType == TransportTypeEnum.TLS ? Ozeki.Network.TransportType.Tls : Ozeki.Network.TransportType.Udp);
            lineConfig.NatConfig = new NatConfiguration(NatTraversalMethod.STUN, null, true);
            lineConfig.SRTPMode = Ozeki.Common.SRTPMode.None;

            return lineConfig;
        }

        private void InitiateSoftphone()
        {
            this.softPhone = SoftPhoneFactory.CreateSoftPhone(MinPort, MaxPort);
            this.softPhone.VideoEncoderQuality = VideoQuality.High;

            this.Log.LogMessage("Softphone was initiated");

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
                    this.callLog.AddToCallLog(phoneCall, true);
                    this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
                    this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();

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
            if (phoneCall == null)
                return;

            phoneCall.CallStateChanged += Call_CallStateChanged;
            phoneCall.DtmfReceived += Call_DtmfReceived;
            phoneCall.DtmfStarted += Call_DtmfStarted;
        }

        private void Call_DtmfStarted(object sender, VoIPEventArgs<DtmfInfo> e)
        {
            int signal = e.Item.Signal.Signal;
            MediaHandlers.StartDtmf(signal);
        }

        private void Call_DtmfReceived(object sender, VoIPEventArgs<DtmfInfo> e)
        {
            DtmfSignal signal = e.Item.Signal;
            MediaHandlers.StopDtmf(signal.Signal);
        }

        private void Call_CallStateChanged(object sender, CallStateChangedArgs e)
        {
            IPhoneCall call = sender as IPhoneCall;
            if (call == null)
                return;

            CallState state = e.State;

            OnPhoneCallStateChanged(call);
            CheckStopRingback();
            CheckStopRingtone();

            lock (lockObj)
            {
                // start ringtones
                if (state.IsRinging())
                {
                    if (call.IsIncoming)
                        MediaHandlers.StartRingtone();
                    else
                        MediaHandlers.StartRingback();

                    return;
                }

                // call has been answered
                if (state == CallState.Answered)
                {
                    return;
                }

                // attach media to the selected call when the remote party sends media data
                if (state.IsRemoteMediaCommunication())
                {
                    if (this.SelectedPhoneCall.Equals(call))
                    {
                        MediaHandlers.AttachAudio(call);
                        MediaHandlers.AttachVideo(call);
                    }
                    return;
                }

                // detach media from the selected call in hold state or when the call has ended
                if (state == CallState.LocalHeld || state == CallState.InactiveHeld || state.IsCallEnded())
                {
                    if (this.SelectedPhoneCall != null && this.SelectedPhoneCall.Equals(call))
                    {
                        MediaHandlers.DetachAudio();
                        MediaHandlers.DetachVideo();
                    }
                }

                // call has ended, clean up
                if (state.IsCallEnded())
                {
                    DisposeCall(call);

                    this.callLog.CallEnded(call);
                    this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
                    this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();
                    this.ActivePhoneCalls.Remove(call);
                }
            }
        }

        private void CheckStopRingtone()
        {
            lock (lockObj)
            {
                bool stopRinging = true;
                foreach (var phoneCall in this.ActivePhoneCalls)
                {
                    if (phoneCall.IsIncoming && phoneCall.CallState.IsRinging())
                    {
                        stopRinging = false;
                        break;
                    }
                }

                if (stopRinging)
                    MediaHandlers.StopRingtone();
            }
        }

        private void CheckStopRingback()
        {
            lock (lockObj)
            {
                bool stopRinging = true;
                foreach (var phoneCall in this.ActivePhoneCalls)
                {
                    if (!phoneCall.IsIncoming && phoneCall.CallState.IsRinging())
                    {
                        stopRinging = false;
                        break;
                    }
                }

                if (stopRinging)
                    MediaHandlers.StopRingback();
            }
        }

        private void DisposeCall(IPhoneCall call)
        {
            lock (lockObj)
            {
                UnsubscribeFromCallEvents(call);

                if (call.Equals(this.SelectedPhoneCall))
                    this.SelectedPhoneCall = null;
            }
        }

        private void UnsubscribeFromCallEvents(IPhoneCall call)
        {
            if (call == null)
                return;

            call.CallStateChanged -= (Call_CallStateChanged);
            call.DtmfReceived -= (Call_DtmfReceived);
            call.DtmfStarted -= (Call_DtmfStarted);
        }

        private void SubscribeToLine(IPhoneLine phoneLine)
        {
            if (phoneLine == null)
                return;
            this.Log.LogMessage("Subscribed to phone line");

            phoneLine.RegistrationStateChanged += Line_RegistrationStateChanged;
        }

        private void Line_RegistrationStateChanged(object sender, RegistrationStateChangedArgs e)
        {
            IPhoneLine line = sender as IPhoneLine;
            if (line == null)
                return;

            RegState state = e.State;

            if (state == RegState.RegistrationSucceeded)
            {
                var subscription = line.Subscription.Create(SIPEventType.MessageSummary);
                this.linesSubscriptions.Add((line, subscription));
                line.Subscription.Subscribe(subscription);
            }

            if (this.SelectedSIPAccount.Contains(line.SIPAccount.RegisterName))
            {
                this.LineState = state.ToString();
            }
        }

        private void StartPhoneCall(IPhoneCall call)
        {
            lock (lockObj)
            {
                if (call == null)
                    return;

                this.ActivePhoneCalls.Add(call);
                this.callLog.AddToCallLog(call);
                this.CallLogItems = new ObservableCollection<CallLogItem>(this.callLog.GetCallLog());
                this.SelectedCallLogItem = this.CallLogItems.FirstOrDefault();

                if (this.SelectedPhoneCall == null)
                    this.SelectedPhoneCall = call;

                SubscribeToCallEvents(call);
                call.Start();

                if (this.SelectedPhoneCall == null)
                    this.SelectedPhoneCall = call;
            }
        }

        private IPhoneLine GetSelectedPhoneLine()
        {
            IPhoneLine line = null;
            if (string.IsNullOrEmpty(this.SelectedSIPAccount))
                return line;

            line = this.PhoneLines.Where(x => this.SelectedSIPAccount.Contains(x.Item2.RegisterName)).FirstOrDefault().Item1;

            return line;
        }

        #endregion Private Methods

        #region Events

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

        #endregion Events
    }
}