using System;
using System.Collections.Generic;
using System.Text;
using Training.Services;
using Training.Models;
using System.Linq;
using Xamarin.Forms;
using Training.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Training.ViewModels
{
    public class PlanViewModel
    {
        public ObservableCollection<PlanInfo> Workouts { get; set; }
        private WorkoutData[] plan;
        private string[] ids;
        private Color c1 = Color.Black;
        private Color c2 = Color.DarkGray;
        public PlanViewModel()
        {
            MessagingCenter.Subscribe<DetailedPlanPage,string>(this, "SetDay", (s, a) => {
                for (int i = 0; i < ids.Length; i++)
                {
                    if (ids[i] == a)
                    {
                        var prof = DataManager.LoadProfile();
                        prof.Day = plan[i].Day;
                        prof.Week = plan[i].Week;
                        DataManager.SaveProfile(prof);
                        Workouts[i].FieldColor = c1;
                    }
                    else
                    {
                        Workouts[i].FieldColor = c2;
                    }
                }
            });
            plan = DataManager.LoadPlan();
            var profile = DataManager.LoadProfile();
            Workouts = new ObservableCollection<PlanInfo>();
            ids = new string[plan.Length];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = Guid.NewGuid().ToString();
                var color = c2;
                if (profile.Day == plan[i].Day && profile.Week == plan[i].Week)
                    color = c1;
                Workouts.Add(new PlanInfo(plan[i], ids[i],color));
            }
        }
    }
    public class PlanInfo:INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Exercises { get; set; }
        public List<DetailedInfo> ExerciseDetails { get; set; }
        public string Id { get; set; }
        private Color color;
        public Color FieldColor { get { return color; } set {color=value; OnPropertyChanged(); } }
        public PlanInfo(WorkoutData data,string id,Color color)
        {
            Title = String.Format("{0} (Week {1}, Day {2})", data.Title, data.Week, data.Day);
            Exercises = "";
            Id = id;
            FieldColor = color;
            if (!data.IsNull)
            {
                Exercises = string.Join(", ", data.Exercises.Select(t => t.Name));
                ExerciseDetails = new List<DetailedInfo>();
                for (int i = 0; i < data.WorkoutMatrix[0].Length; i++)
                {
                    if (data.WorkoutMatrix[5][i] == 0)
                    {
                        var info = new DetailedInfo();
                        info.ExName = data.Exercises[data.WorkoutMatrix[0][i]].Name;

                        string repText = data.RepText[data.WorkoutMatrix[1][i]];
                        var reps = data.Reps[data.WorkoutMatrix[2][i]];
                        if (repText[0] == '-')
                            repText = String.Format("Max Reps -{0}", reps);
                        else if (repText[0] == '%')
                            repText = String.Format("{0}% of Max Reps", reps);
                        info.RepText = repText;
                        ExerciseDetails.Add(info);
                    }
                }
            }

        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class DetailedInfo
    {
        public string ExName { get; set; }
        public string RepText { get; set; }
    }
}
