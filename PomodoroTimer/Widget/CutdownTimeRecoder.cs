using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer.Widget
{
    class CutdownTimeRecoder
    {
        public int TotalTime
        {
            get { return _TotalTime; }
            set
            {
                PassedTime = 0;
                _TotalTime = value;
            }
        }
        public int PassedTime { get; set; }
        public int RemainingTime { get { return TotalTime - PassedTime; } }
        public bool IsTimeOff { get { return RemainingTime == 0; } }
        public void SecondPassed()
        {
            PassedTime++;
        }

        private int _TotalTime;
    }
}
