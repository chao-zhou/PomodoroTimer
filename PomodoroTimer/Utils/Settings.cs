﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using PomodoroTimer.Extension;

namespace PomodoroTimer
{
    class Settings
    {
        public int BreakLength { get; set; }
        public int LongBreakLength { get; set; }
        public int PomodoroLength { get; set; }
        public bool IsAutoSwich { get; set; }


        public Settings()
        {
            ReLoad();
        }

        public void ReLoad()
        {
            var store = ApplicationData.Current.LocalSettings;
            BreakLength = store.Values.GetValue<int>("breakLen", 5);
            LongBreakLength = store.Values.GetValue<int>("lbreakLen", 15);
            PomodoroLength = store.Values.GetValue<int>("workLen", 25);
            IsAutoSwich = store.Values.GetValue<bool>("autoSwich", true);
        }

        public void Save()
        {
            var store = ApplicationData.Current.LocalSettings;
            store.Values["breakLen"] = 5;
            store.Values["lbreakLen"] = 15;
            store.Values["workLen"] = 25;
            store.Values["autoSwich"] = IsAutoSwich;
        }
        


    }
}
