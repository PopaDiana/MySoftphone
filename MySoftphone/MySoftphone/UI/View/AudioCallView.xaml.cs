using MySoftphone.UI.Model;
using MySoftphone.UI.ViewModel;
using Ozeki.VoIP;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MySoftphone.UI.View
{
    /// <summary>
    /// Interaction logic for AudioCallView.xaml
    /// </summary>
    public partial class AudioCallView : UserControl
    {
        private AudioCallViewModel viewModel;

        public AudioCallView()
        {
            InitializeComponent();
            viewModel = (AudioCallViewModel)this.DataContext;
            if(viewModel!= null && viewModel.SoftphoneManager!= null)
            {
                viewModel.SoftphoneManager.PhoneCallStateChanged += Model_PhoneCallStateChanged;
            }
            
        }

        private void CallsWindowLoaded(object sender, RoutedEventArgs e)
        {
            var audioView = (AudioCallViewModel)this.DataContext;
            if (audioView != null && audioView.SoftphoneManager != null
                && audioView.SoftphoneManager.MediaHandlers != null)
            {
                this.localVideoViewer.SetImageProvider(audioView.SoftphoneManager.MediaHandlers.LocalImageProvider);
                this.remoteVideoViewer.SetImageProvider(audioView.SoftphoneManager.MediaHandlers.RemoteImageProvider);

                remoteVideoViewer.Start();
                localVideoViewer.Start();
            }
        }

        private void OnStartCameraPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                var audioView = (AudioCallViewModel)this.DataContext;
                if (audioView != null && audioView.SoftphoneManager != null
                    && audioView.SoftphoneManager.MediaHandlers != null)
                {
                    audioView.SoftphoneManager.MediaHandlers.StartVideo();
                    this.localVideoViewer.Start();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void OnEndCameraPressed(object sender, RoutedEventArgs e)
        {
            try
            {
                var audioView = (AudioCallViewModel)this.DataContext;
                if (audioView != null && audioView.SoftphoneManager != null
                    && audioView.SoftphoneManager.MediaHandlers != null)
                {
                    audioView.SoftphoneManager.MediaHandlers.StopVideo();
                    this.localVideoViewer.Stop();
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        private void Model_PhoneCallStateChanged(object sender, GeneralEventArgs<IPhoneCall> e)
        {
            this.UpdatePhoneCalls();
        }

        private void UpdatePhoneCalls()
        {
            this.activeCallsLV.Dispatcher.Invoke(new Action(() => activeCallsLV.Items.Refresh()));
        }
    }
}