using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace PomodoroTimer
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : PomodoroTimer.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
          
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
            pMgr = new PomodoroManager(new Settings());
            nMgr = new NotificationManager();

            timer.CompleteHandler = new EventHandler(StepComplete);
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {

        }

        private void StepComplete(object sender, EventArgs e)
        {
            var msg = string.Format("{0} is complete !", pMgr.CurrentStep);
            nMgr.ShowToast(msg);

            pMgr.Next();

            state.Text = pMgr.CurrentStep.ToString();
            timer.Start(pMgr.CurrentLength * 60);
            
            msg = string.Format("{0} is start !", pMgr.CurrentStep);
            nMgr.ShowToast(msg);
        }


        private void playButton_Checked(object sender, RoutedEventArgs e)
        {
            pMgr.Start(PomodoroManager.Step.Pomodoro); 
            state.Text = pMgr.CurrentStep.ToString();
            timer.Start(pMgr.CurrentLength * 60);
            playButton.Content = "Interruption";
        }

        private void playButton_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Reset();
            playButton.Content = "Start";
        }

        private PomodoroManager pMgr;
        private NotificationManager nMgr;
    }


}
