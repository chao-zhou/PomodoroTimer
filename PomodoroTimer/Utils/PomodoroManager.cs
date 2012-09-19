namespace PomodoroTimer.Utils
{
    class PomodoroManager
    {
        public enum Step { 
            None,
            Pomodoro,
            Break,
            LongBreak
        }

        public PomodoroManager(Settings settings)
        {
            _settings = settings;
        }

        public Step CurrentStep { get; private set; }
        public int CurrentLength { get { return GetCurrentLength(); } }

       
        public int PomodoroCount { get; private set; }
        public int BreakCount { get; private set; }
        public int LongBreakCount { get; private set; }

        public void Start(Step step)
        {
            CurrentStep = step;
        }

        public void Next()
        {
            if(CurrentStep == Step.Pomodoro)
                PomodoroComplete();

            if (ShouldLongBreak())
            {
                CurrentStep = Step.LongBreak;
                LongBreakCount++;
            }
            else if (ShouldBreak())
            {
                CurrentStep = Step.Break;
                BreakCount++;
            }
            else
            {
                CurrentStep = Step.Pomodoro;
            }

        }

        public void Next(Step step)
        {
            if (CurrentStep == Step.Pomodoro)
            {
                PomodoroComplete();
            }

            CurrentStep = step;
        }

        private void PomodoroComplete()
        {
            PomodoroCount++;
        }

        private bool ShouldLongBreak()
        {
            return CurrentStep == Step.Pomodoro
                && PomodoroCount % 4 == 0;
        }

        private bool ShouldBreak()
        {
            return CurrentStep == Step.Pomodoro;
        }

        private int GetCurrentLength()
        {
            switch (CurrentStep) {
                case Step.Pomodoro: return _settings.PomodoroLength;
                case Step.Break: return _settings.BreakLength;
                case Step.LongBreak: return _settings.LongBreakLength;
                default: return 0;
            }
        }

        private readonly Settings _settings;
     
        
    }
}
