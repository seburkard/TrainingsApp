using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Training.Services
{
    class TimerClass
    {
        string time;
        public string Time { get { return time; } set { if (time != value) { time = value; OnPropertyChanged(); } } }
        public int TimerValue { get {return timerValue; } set { if (value == 0) { timerValue = int.MaxValue; } else timerValue=value;Reset();} }
        int start,interval,timeElapsed,timerValue;
        bool timerEnabled;

        public event EventHandler ThresholdReached;
        public event PropertyChangedEventHandler PropertyChanged; 
        public TimerClass(int timerValue=int.MaxValue,int interval=10)
        {
            this.interval = interval;
            TimerValue = timerValue;
        }
        public void Start()
        {
            timerEnabled = true;
            start = Environment.TickCount-timeElapsed;
            Timer timer = new Timer(new TimerCallback(TimerProc));
            timer.Change(interval, interval);
        }
        public void Stop()
        {
            timerEnabled = false;
        }
        public void Reset()
        {
            timeElapsed = 0;
            start = Environment.TickCount;
            ResetString();
        }
        private void ResetString()
        {
            if (TimerValue != int.MaxValue)
            {
                SetTime(TimerValue);
            }
            else
                SetTime(0);
        }
        private void TimerProc(object state)
        {
            timeElapsed = Environment.TickCount - start;
            int t;
            if (TimerValue == int.MaxValue)
            {
                t = timeElapsed;
            }
            else
            {
                t = TimerValue - timeElapsed;
            }
            if (t<0||!timerEnabled)
            {
                Timer timer = (Timer)state;
                timer.Dispose();
                if (t < 0)
                {
                    t = 0;
                    OnThresholdReached(EventArgs.Empty);
                }
            }
            SetTime(t);
        }
        private void SetTime(int t)
        {
            Time = new TimeSpan(10000 * (long)t).ToString(@"mm\:ss\:ff");
        }
        protected virtual void OnThresholdReached(EventArgs e)
        {
            ThresholdReached?.Invoke(this, e);
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
