using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using PomodoroTimer.Utils;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PomodoroTimer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
          
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            _settings = new Settings();
            _pomodoroManager = new PomodoroManager(_settings);
            _notificationManager = new NotificationManager();

            autoSwitch.IsOn = _settings.IsAutoSwich;

            timer.CompleteHandler = StepComplete;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see>
        ///                   <cref>SuspensionManager.SessionState</cref>
        ///                 </see> .
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }

        private void StepComplete(object sender, EventArgs e)
        {
            CompleteCurrentAndSetNext();

            if (!autoSwitch.IsOn)
            {
                state.Text = _pomodoroManager.CurrentStep.ToString();
                playButton.IsChecked = false;
                return;
            }

            StartNext(); 
        }

        private void CompleteCurrentAndSetNext()
        {
            var msg = string.Format("{0} is complete !", _pomodoroManager.CurrentStep);
            _notificationManager.ShowToast(msg);
            _pomodoroManager.Next();
        }

        private void StartNext() {
            state.Text = _pomodoroManager.CurrentStep.ToString();
            timer.Start(_pomodoroManager.CurrentLength * 60);

            var msg = string.Format("{0} is start !", _pomodoroManager.CurrentStep);
            _notificationManager.ShowToast(msg);
        }

        private void PlayButtonChecked(object sender, RoutedEventArgs e)
        {
            _pomodoroManager.Start(PomodoroManager.Step.Pomodoro); 
            state.Text = _pomodoroManager.CurrentStep.ToString();
            timer.Start(_pomodoroManager.CurrentLength * 60);
            playButton.Content = "Interruption";
        }

        private void PlayButtonUnchecked(object sender, RoutedEventArgs e)
        {
            timer.Reset(_pomodoroManager.CurrentLength * 60);
            playButton.Content = "Start";
        }

        private void AutoSwitchToggled(object sender, RoutedEventArgs e)
        {
            if (_settings == null)
                return;

            _settings.IsAutoSwich = ((ToggleSwitch)sender).IsOn;
            _settings.Save();
        }

        private PomodoroManager _pomodoroManager;
        private NotificationManager _notificationManager;
        private Settings _settings;

       

   
 
    }


}
