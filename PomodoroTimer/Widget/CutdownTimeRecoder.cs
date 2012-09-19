namespace PomodoroTimer.Widget
{
    class CutdownTimeRecoder
    {
        public int TotalTime
        {
            get { return _totalTime; }
            set
            {
                PassedTime = 0;
                _totalTime = value;
            }
        }
        public int PassedTime { get; set; }
        public int RemainingTime { get { return TotalTime - PassedTime; } }
        public bool IsTimeOff { get { return RemainingTime == 0; } }
        public void SecondPassed()
        {
            PassedTime++;
        }

        private int _totalTime;
    }
}
