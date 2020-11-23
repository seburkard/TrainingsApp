using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Training.Models;
using Training.Services;
using Training.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Training.ViewModels
{
    class SettingsViewModel
    {
        public ObservableCollection<ProfileItem> Items { get; set; }
        public ObservableCollection<ProfileItem> MaxReps { get; set; }
        private Profile profile;
        public ICommand SaveChangesCommand { get; private set; }
        public ICommand ExportProgress { get { return new Command(Export); } }
        private bool changed=false;
        public SettingsViewModel()
        {
            SaveChangesCommand = new Command(execute:SaveChanges, canExecute: () => { return changed; }) ;
            profile = DataManager.LoadProfile();
            var progress = DataManager.LoadProgress();
            profile.UpdateMaxReps(progress);
            Items = new ObservableCollection<ProfileItem>();
            Items.Add(new ProfileItem { Text = "Workout Level", Value = profile.WorkoutLevel });
            Items.Add(new ProfileItem { Text = "Week", Value = profile.Week });
            Items.Add(new ProfileItem { Text = "Day", Value = profile.Day });
            MaxReps = new ObservableCollection<ProfileItem>();
            foreach (var item in profile.maxReps.Select(t => new ProfileItem { Text = t.Key, Value = t.Value }).ToList())
            {
                MaxReps.Add(item);
            }
            MessagingCenter.Subscribe<SettingsPage,Tuple<string,string>>(this, "ChangedValue",(sender, args) =>
            {
                int val;
                if (int.TryParse(args.Item2, out val))
                {
                    foreach (var item in Items.Union(MaxReps))
                    {
                        if (item.Text == args.Item1)
                        {
                            item.Value = val;
                        }
                    }
                    changed = true;
                    (SaveChangesCommand as Command).ChangeCanExecute();
                }

            });
        }
        private async void Export()
        {
            try
            {
                DataManager.SaveProfile(profile);
                var message = new EmailMessage
                {
                    Subject = "TrainingData",
                    Body = DateTime.Now.ToString(),
                    To = new List<string>(new string[] { "severinburkard@gmail.com" }),
                    //Cc = ccRecipients,
                    //Bcc = bccRecipients
                };
                foreach (var item in DataManager.GetPath())
                {
                    message.Attachments.Add(new EmailAttachment(item));
                }
                await Email.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device
            }
            catch (Exception ex)
            {
                // Some other exception occurred
            }
        }
        private void SaveChanges()
        {
            UpdateProfile();
            DataManager.SaveProfile(profile);
            changed = false;
            (SaveChangesCommand as Command).ChangeCanExecute();
        }
        private void UpdateProfile()
        {
            profile.WorkoutLevel = Items[0].Value;
            profile.Week = Items[1].Value;
            profile.Day = Items[2].Value;
            foreach (var key in profile.maxReps.Keys)
            {
                foreach (var item in Items)
                {
                    if (key == item.Text)
                    {
                        profile.maxReps[key] = item.Value;
                    }
                }
            }
        }
    }
    public class ProfileItem:INotifyPropertyChanged
    {
        private string text;
        public string Text { get { return text; } set { text = value; OnPropertyChanged(); } }
        private int val;
        public int Value { get { return val; } set { val = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
