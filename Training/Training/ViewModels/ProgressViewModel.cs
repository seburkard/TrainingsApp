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
using System.Windows.Input;

namespace Training.ViewModels
{
    public class ProgressViewModel
    {
        public ObservableCollection<ProgressInfo> Workouts { get; set; }
        private WorkoutData[] progress;
        private Dictionary<string, Tuple<int, int>> planDict;
        public ICommand SaveChangesCommand { get; private set; }
        private bool changed = false;
        public ProgressViewModel()
        {
            SaveChangesCommand = new Command(execute: SaveChanges, canExecute: () => { return changed; });
            MessagingCenter.Subscribe<DetailedProgressPage, Tuple<string, string>>(this, "ChangedValue", (sender, args) =>
            {
                int val;
                if (int.TryParse(args.Item2, out val))
                {
                    for (int i = 0; i < Workouts.Count; i++)
                    {
                        for (int j = 0; j < Workouts[i].ExerciseDetails.Count; j++)
                        {
                            if (Workouts[i].ExerciseDetails[j].Id == args.Item1)
                            {
                                Workouts[i].ExerciseDetails[j].RepText = val.ToString();
                                changed = true;
                                (SaveChangesCommand as Command).ChangeCanExecute();
                            }
                        }
                    }
                }
            });
            planDict = new Dictionary<string, Tuple<int, int>>();
            progress = DataManager.LoadProgress();
            var profile = DataManager.LoadProfile();
            Workouts = new ObservableCollection<ProgressInfo>();

            for (int i = 0; i < progress.Length; i++)
            {
                if (!progress[i].IsNull)
                {
                    var ids = new string[progress[i].WorkoutMatrix[5].Length - progress[i].WorkoutMatrix[5].Sum()];
                    int pos = 0;
                    for (int j = 0; j < progress[i].WorkoutMatrix[5].Length; j++)
                    {
                        if (progress[i].WorkoutMatrix[5][j] == 0)
                        {
                            ids[pos] = Guid.NewGuid().ToString();
                            planDict.Add(ids[pos], new Tuple<int, int>(i, j));
                            pos++;
                        }
                    }
                    Workouts.Add(new ProgressInfo(progress[i], ids));
                }
                else
                {
                    Workouts.Add(new ProgressInfo(progress[i], new string[0]));
                }
            }

        }
        public void Delete(List<DetailedProgressInfo> info)
        {
            int len = Workouts.Count;
            int ind=-1;
            for (int i = 0; i < len; i++)
            {
                bool delete = true;
                for (int j = 0; j < Workouts[i].ExerciseDetails.Count; j++)
                {
                    if (Workouts[i].ExerciseDetails.Count != info.Count)
                    {
                        delete = false;
                        break;
                    }
                    if (Workouts[i].ExerciseDetails[j].Id != info[j].Id)
                    {
                        delete = false;
                        break;
                    }
                }
                if (delete)
                {
                    ind = i;
                }
            }
            if(ind>=0)
            {
                DeleteItem(ind);
            }
        }
        private void SaveChanges()
        {
            UpdateProgress();
            DataManager.SaveProgress(progress);
            changed = false;
            (SaveChangesCommand as Command).ChangeCanExecute();
        }
        private void DeleteItem(int ind)
        {
            var progressList = new List<WorkoutData>();
            for (int i = 0; i < progress.Length; i++)
            {
                if (i != ind)
                {
                    progressList.Add(progress[i]);
                }
            }
            Workouts.Remove(Workouts[ind]);
            progress = progressList.ToArray();
            foreach (var workout in Workouts)
            {
                foreach (var item in workout.ExerciseDetails)
                {
                    if (planDict.ContainsKey(item.Id))
                    {
                        if (planDict[item.Id].Item1 == ind)
                        {
                            planDict.Remove(item.Id);
                        }
                        if (planDict[item.Id].Item1 > ind)
                        {
                            planDict[item.Id] = new Tuple<int, int>(planDict[item.Id].Item1 - 1, planDict[item.Id].Item2);
                        }
                    }
                }
            }
            UpdateProgress();
            DataManager.SaveProgress(progress);
        }
        private void UpdateProgress()
        {
            foreach (var workouts in Workouts)
            {
                foreach (var item in workouts.ExerciseDetails)
                {
                    progress[planDict[item.Id].Item1].WorkoutMatrix[8][planDict[item.Id].Item2] = int.Parse(item.RepText);
                    if (progress[planDict[item.Id].Item1].Title == "Test")
                    {
                        progress[planDict[item.Id].Item1].MaxReps[progress[planDict[item.Id].Item1].WorkoutMatrix[0][planDict[item.Id].Item2]] = int.Parse(item.RepText);
                    }
                }
            }
        }
    }
    public class ProgressInfo:INotifyPropertyChanged
    {
        public string Title { get; set; }
        public string Exercises { get; set; }
        public List<DetailedProgressInfo> ExerciseDetails { get; set; }
        public ProgressInfo(WorkoutData data,string[] ids)
        {
            Title = String.Format("{0}, ({1})", data.Title, DateTime.FromBinary(data.Completion).ToString("MMMM dd"));
            Exercises = "";
            ExerciseDetails = new List<DetailedProgressInfo>();
            if (!data.IsNull)
            {
                Exercises = string.Join(", ", data.Exercises.Select(t => t.Name));
                int pos = 0;
                for (int i = 0; i < data.WorkoutMatrix[5].Length; i++)
                {
                    if (data.WorkoutMatrix[5][i] == 0)
                    {
                        var info = new DetailedProgressInfo();
                        info.ExName = data.Exercises[data.WorkoutMatrix[0][i]].Name;
                        string repText = data.WorkoutMatrix[8][i].ToString();
                        info.RepText = repText;
                        info.Id = ids[pos];
                        pos++;
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
    public class DetailedProgressInfo : INotifyPropertyChanged
    {
        string exName;
        public string ExName { get { return exName; } set { exName = value; OnPropertyChanged(); } }
        string repText;
        public string RepText { get { return repText; } set {repText=value;OnPropertyChanged(); } }
        string id;
        public string Id { get { return id; } set { id = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
