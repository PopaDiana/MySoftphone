using Ozeki.Camera;
using Ozeki.Media;
using Ozeki.VoIP;
using System;
using System.IO;
using System.Drawing;

namespace MySoftphone.UI.Model
{
    public class MediaHandlers
    {
        private bool initialized;

        private PhoneCallAudioSender phoneCallAudioSender;
        private PhoneCallAudioReceiver phoneCallAudioReceiver;

        private PhoneCallVideoSender phoneCallVideoSender;
        private PhoneCallVideoReceiver phoneCallVideoReceiver;

        private WaveStreamPlayback ringbackPlayer;
        private WaveStreamPlayback ringtonePlayer;
        private DtmfEventWavePlayer dtmfPlayer;
        private MediaConnector audioConnector;
        private MediaConnector videoConnector;
        private AudioMixerMediaHandler recordDataMixer;

        public AudioQualityEnhancer AudioEnhancer { get; private set; }
        public Microphone Microphone { get; private set; }
        public Speaker Speaker { get; private set; }
        public IWebCamera WebCamera { get; private set; }
        public ImageProvider<Image> LocalImageProvider { get; private set; }
        public ImageProvider<Image> RemoteImageProvider { get; private set; }


        public MediaHandlers()
        {
            this.audioConnector = new MediaConnector();
            this.videoConnector = new MediaConnector();
            this.Initialize();
        }

        private void Initialize()
        {
            if (this.initialized)
                return;

            InitAudio();
            InitVideo();

            if (Microphone != null)
            {
                Microphone.Start();
            }

            if (Speaker != null)
            {
                Speaker.Start();
            }

            this.initialized = true;
        }

        private void InitVideo()
        {
            this.WebCamera = WebCameraFactory.GetDefaultDevice();
            this.WebCamera.DesiredFrameRate = 30;
            this.WebCamera.Resolution = new Ozeki.OzResolution(640,480);

            this.LocalImageProvider = new DrawingImageProvider();
            this.RemoteImageProvider = new DrawingImageProvider();

            this.phoneCallVideoReceiver = new PhoneCallVideoReceiver();
            this.phoneCallVideoSender = new PhoneCallVideoSender();

            this.videoConnector.Connect(this.phoneCallVideoReceiver, this.RemoteImageProvider);
            if (this.WebCamera != null)
            {
                this.videoConnector.Connect(this.WebCamera.VideoChannel, this.LocalImageProvider);
                this.videoConnector.Connect(this.WebCamera.VideoChannel, this.phoneCallVideoSender);
            }
        }

        private void InitAudio()
        {
            // create devices
            Microphone = Microphone.GetDefaultDevice();
            Speaker = Speaker.GetDefaultDevice();

            // audio processors
            if (Microphone == null)
                AudioEnhancer = new AudioQualityEnhancer(new AudioFormat());
            else
                AudioEnhancer = new AudioQualityEnhancer(Microphone.MediaFormat);
            AudioEnhancer.SetEchoSource(Speaker);
            this.dtmfPlayer = new DtmfEventWavePlayer();

            // ringtones
            var ringbackStream = LoadRingbackStream();
            var ringtoneStream = LoadRingtoneStream();
            this.ringtonePlayer = new WaveStreamPlayback(ringtoneStream, true, false);
            this.ringbackPlayer = new WaveStreamPlayback(ringbackStream, true, false);

            // mixers
            this.recordDataMixer = new AudioMixerMediaHandler();

            // phone handlers
            this.phoneCallAudioSender = new PhoneCallAudioSender();
            this.phoneCallAudioReceiver = new PhoneCallAudioReceiver();

            this.audioConnector.Connect(this.AudioEnhancer, this.phoneCallAudioSender);
            this.audioConnector.Connect(this.AudioEnhancer, this.recordDataMixer);
            this.audioConnector.Connect(this.phoneCallAudioReceiver, this.recordDataMixer);
            if (this.Microphone != null)
            {
                this.audioConnector.Connect(this.Microphone, this.AudioEnhancer);
            }

            // connect incoming
            if (this.Speaker != null)
            {
                this.audioConnector.Connect(this.phoneCallAudioReceiver, Speaker);
                this.audioConnector.Connect(this.ringtonePlayer, Speaker);
                this.audioConnector.Connect(this.ringbackPlayer, Speaker);
                this.audioConnector.Connect(this.dtmfPlayer, Speaker);
            }
        }

        private Stream LoadRingtoneStream()
        {
            Stream basicRing = Properties.Resources.basic_ring;

            if (basicRing == null)
                throw new Exception("Cannot load default ringtone.");

            return basicRing;
        }

        private Stream LoadRingbackStream()
        {
            Stream ringback = Properties.Resources.ringback;

            if (ringback == null)
                throw new Exception("Cannot load default ringback.");

            return ringback;
        }

        internal void AttachAudio(IPhoneCall selectedCall)
        {
            AudioEnhancer.Refresh();
            AudioEnhancer.Start();
            this.phoneCallAudioSender.AttachToCall(selectedCall);
            this.phoneCallAudioReceiver.AttachToCall(selectedCall);
        }

        public void DetachAudio()
        {
            this.phoneCallAudioSender.Detach();
            this.phoneCallAudioReceiver.Detach();
            this.AudioEnhancer.Stop();
        }

        internal void AttachVideo(IPhoneCall selectedCall)
        {
            this.phoneCallVideoReceiver.AttachToCall(selectedCall);
            this.phoneCallVideoSender.AttachToCall(selectedCall);
        }

        public void DetachVideo()
        {
            this.phoneCallVideoReceiver.Detach();
            this.phoneCallVideoSender.Detach();
        }

        internal void StartRingtone()
        {
            if (this.ringtonePlayer != null)
                this.ringtonePlayer.Start();
        }

        internal void StartRingback()
        {
            if (this.ringbackPlayer != null)
                this.ringbackPlayer.Start();
        }

        internal void StopRingback()
        {
            if (this.ringbackPlayer != null)
                this.ringbackPlayer.Stop();
        }

        internal void StopRingtone()
        {
            if (this.ringtonePlayer != null)
                this.ringtonePlayer.Stop();
        }

        internal void StopDtmf(int signal)
        {
            if (!this.initialized)
                return;

            this.dtmfPlayer.Stop();
        }

        internal void StartDtmf(int signal)
        {
            if (!this.initialized)
                return;

            this.dtmfPlayer.Start(signal);
        }

        public void StartVideo()
        {
            if (this.WebCamera != null)
                this.WebCamera.Start();
        }

        public void StopVideo()
        {
            if (this.WebCamera != null)
                this.WebCamera.Stop();
        }
    }
}