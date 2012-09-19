using System;
using Windows.UI.Core;
using WinRTXamlToolkit.Tools;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace PomodoroTimer.Widget
{
    public sealed partial class CutdownTimer
    {
        public EventHandler CompleteHandler { get; set; }

        public CutdownTimer()
        {
            InitializeComponent();
            InitializeTimer();

            _recorder = new CutdownTimeRecoder ();
        }

        public void Start(int seconds)
        {
            SafeSetTime(r => 
            { 
                r.TotalTime = seconds;
                DataContext = new { Dashboard = _recorder.RemainingTime };
            });
        }

        public void Start()
        {
            SafeSetTime(r =>
            {
                r.TotalTime = _recorder.RemainingTime;
                DataContext = new { Dashboard = _recorder.RemainingTime };
            });
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void ReStart()
        {
            SafeSetTime(r =>
            {
                r.PassedTime = 0;
                DataContext = new { Dashboard = _recorder.RemainingTime };
            });
        }

        public void Reset(int seconds)
        {
            _timer.Stop();
            _recorder.TotalTime = seconds;
            DataContext = new { Dashboard = _recorder.RemainingTime };
        }

        public void Reset()
        {
            Reset(_recorder.TotalTime);
        }

        private void InitializeTimer()
        {
            //timer = new DispatcherTimer();
            _timer = new BackgroundTimer {Interval = TimeSpan.FromSeconds(1.00)};
            _timer.Tick += OneSecondPassed;
            DataContext = new { Dashboard = 0 };
        }

        private async void OneSecondPassed(object sender, object e)
        {
          if (Dispatcher.HasThreadAccess)
                DoOneSecondPassedWork();
          else 
              await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, DoOneSecondPassedWork);

        }

        private void DoOneSecondPassedWork()
        {
            _recorder.SecondPassed();
            DataContext = new { Dashboard = _recorder.RemainingTime };
            TryToFireComplete();
        }

        private void TryToFireComplete()
        {
            if (!_recorder.IsTimeOff) return;
            Stop();
            FireComplete();
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
            _timer.Stop();
            
            setTimeAction(_recorder);
            
            _timer.Start();
        }

        //private DispatcherTimer timer;
        private BackgroundTimer _timer;

        private readonly CutdownTimeRecoder _recorder;
    }
}
