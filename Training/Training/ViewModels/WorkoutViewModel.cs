using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Training.Models;
using Training.Services;
using Training.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Training.ViewModels
{
    class WorkoutViewModel:BaseViewModel
    {
        private ObservableCollection<StringItem> titles;
        public ICommand StopTimerCommand { get; private set; }
        public ICommand StartTimerCommand { get; private set; }
        public ICommand ResetTimerCommand { get; private set; }
        public ICommand NextCommand { get; private set; }
        public ICommand LastCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        private TimerClass timer=new TimerClass();
        private string time;
        public string Time { get { return time;} set { SetProperty(ref time, value); } }
        private Workout workout;
        private bool saved = false;
        public ObservableCollection<StringItem> Titles { get { return titles; } set {SetProperty(ref titles,value); } }
        ObservableCollection<StringItem> picture;
        public ObservableCollection<StringItem> Picture { get { return picture; }set { SetProperty(ref picture, value); } }

        ObservableCollection<StringItem> text;
        public ObservableCollection<StringItem> Text { get { return text; } set { SetProperty(ref text, value); } }
        private float progress;
        public float Progress { get { return progress; }set { SetProperty(ref progress, value); } }
        private string ID;
        public WorkoutViewModel(string ID)
        {
            this.ID = ID;
            var workouts = DataManager.LoadPlan();
            var profile = DataManager.LoadProfile();
            foreach (var item in workouts)
            {
                if (item.Day == profile.Day && item.Week == profile.Week)
                {
                    workout = new Workout(item);
                }
            }
            Time = timer.Time;
            timer.PropertyChanged += (e, a) => Time = timer.Time;
            timer.ThresholdReached += (e, a) => { Vibration.Vibrate(1000); Next2(); };

            StopTimerCommand = new Command(execute: timer.Stop);
            StartTimerCommand = new Command(execute: timer.Start);
            ResetTimerCommand = new Command(timer.Reset);
            if (workout == null)
            {
                Titles = new ObservableCollection<StringItem>();
                Titles.Add(new StringItem { Text = "No Workout exists for this day." });
            }
            else
            {
                Titles = new ObservableCollection<StringItem>();
                NextCommand = new Command(execute: Next, canExecute: () => { return !workout.Finished; });
                LastCommand = new Command(execute: Last, canExecute: () => { return workout.Current > 0; });
                SaveCommand = new Command(execute: Save, canExecute: () => { return workout.Finished && !saved; });
                Update();
                MessagingCenter.Subscribe<WorkoutPage, string>(this, "PopUpResult"+ID, (s, a) => { int rep; if (int.TryParse(a, out rep)) { Next2(rep); } else if (a == string.Empty) Next2(0); });
            }
        }
        private void Next()
        {
            if (workout.NeedsInput)
            {
                MessagingCenter.Send(this, ID);
            }
            else
            {
                Next2();
            }
        }
        private void Next2(int reps = -1)
        {
            workout.Next(reps);
            if (workout.Current == 1)
            {
                (LastCommand as Command).ChangeCanExecute();
            }
            if (workout.Finished)
            {
                (NextCommand as Command).ChangeCanExecute();
                (SaveCommand as Command).ChangeCanExecute();
            }
            Update();
        }
        private void Update()
        {
            UpdateTitles();
            timer.Stop();
            if (workout.OnBreak||workout.NeedsTimer)
            {
                timer.TimerValue = workout.Time*1000;
            }
            else
            {
                timer.TimerValue = 0;
            }
            timer.Reset();
            if (workout.OnBreak)
                timer.Start();
        }
        private void Last()
        {
            //StopTimer();
            bool nextExe = workout.Finished;
            workout.Last();
            if (nextExe)
            {
                (NextCommand as Command).ChangeCanExecute();
                if(!saved)
                    (SaveCommand as Command).ChangeCanExecute();
            }
            if (workout.Current == 0)
            {
                (LastCommand as Command).ChangeCanExecute();
            }
            Update();
        }
        private void Save()
        {
            DataManager.SaveWorkout(workout.Data);
            var profile = DataManager.LoadProfile();
            if (profile.Day < 7)
            {
                profile.Day++;
            }
            else
            {
                profile.Week++;
            }
            DataManager.SaveProfile(profile);
            if (!saved)
            {
                saved = true;
                (SaveCommand as Command).ChangeCanExecute();
            }
        }
        private void UpdateTitles()
        {
            var outputs = workout.Output;
            for (int i = 0; i < 5; i++)
            {
                if (Titles.Count<=i)
                {
                    Titles.Add(new StringItem());
                }
                Titles[i].Text=outputs[i];
            }
            Progress = workout.Progress;
            var holds = new string[] { "\uf060", "\uf667", "\uf048" };
            var holdTimes = outputs[2];
            if (Text == null)
            {
                Text = new ObservableCollection<StringItem>(new StringItem[] { new StringItem { Text = "" }, new StringItem { Text = "" }, new StringItem { Text = "" } });
                Picture = new ObservableCollection<StringItem>(new StringItem[] { new StringItem { Text = "" }, new StringItem { Text = "" }, new StringItem { Text = "" } });
            }
            if (holdTimes != string.Empty)
            {
                var strings = holdTimes.Split(':').Select(t => holds[(int.Parse(t) / 10) - 1]).ToArray();
                var timeStr = holdTimes.Split(':').Select(t => t[1]+"s").ToArray();
                for (int i = 0; i < 3; i++)
                {
                    Picture[i].Text = strings[i];
                    Text[i].Text = timeStr[i];
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    Text[i].Text = "";
                    Picture[i].Text = "";
                }
            }
        }
    }
    public class StringItem : BaseViewModel
    {
        private string text;
        public string Text { get {return text; } set { if (text != value) {SetProperty(ref text,value); } } }
    }
}
