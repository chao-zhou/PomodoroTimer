using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PomodoroTimer.Widget
{
    public sealed partial class CutdownTimer : UserControl
    {
        public EventHandler CompleteHandler { get; set; }

        public CutdownTimer()
        {
            this.InitializeComponent();
            this.InitializeTimer();

            this.recorder = new CutdownTimeRecoder ();
        }

        public void Start(int seconds)
        {
            SafeSetTime(r => 
            { 
                r.TotalTime = seconds;
                this.DataContext = new { Dashboard = recorder.RemainingTime };
            });
        }

        public void Start()
        {
            SafeSetTime(r =>
            {
                r.TotalTime = recorder.RemainingTime;
                this.DataContext = new { Dashboard = recorder.RemainingTime };
            });
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void ReStart()
        {
            SafeSetTime(r =>
            {
                r.PassedTime = 0;
                this.DataContext = new { Dashboard = recorder.RemainingTime };
            });
        }

        public void Reset()
        {
            timer.Stop();
            recorder.PassedTime = 0;
            this.DataContext = new { Dashboard = recorder.RemainingTime };
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1.00);
            timer.Tick += OneSecondPassed;
            this.DataContext = new { Dashboard = 0 };
        }

        private void OneSecondPassed(object sender, object e)
        {
            recorder.SecondPassed();
            this.DataContext = new { Dashboard = recorder.RemainingTime };

            if (recorder.IsTimeOff) {
                Stop();
                FireComplete();
            }
        }

        private void FireComplete()
        {
            if (CompleteHandler != null)
            {
                CompleteHandler(this, new EventArgs());
            }
        }


        private void SafeSetTime(Action<CutdownTimeRecoder> setTimeAction)
        {
            timer.Stop();
            
            setTimeAction(recorder);
            
            timer.Start();
        }

        private DispatcherTimer timer;
        private CutdownTimeRecoder recorder;
    }
}
