using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PomodoroTimer
{
    class Settings
    {
        public int BreakLength { get; set; }
        public int LongBreakLength { get; set; }
        public int PomodoroLength { get; set; }

        public Settings()
        {
            BreakLength = 5;
            LongBreakLength = 15;
            PomodoroLength = 25;
        }
    }
}
