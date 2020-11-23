using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Training.ViewModels;

namespace Training.Views
{
    class PlanPage : ContentPage
    {
        public PlanPage()
        {
            BindingContext = new PlanViewModel();
            ListView listView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell { TextColor = Color.Black };
                    textCell.SetBinding(TextCell.TextProperty, "Title");
                    textCell.SetBinding(TextCell.DetailProperty, "Exercises");
                    textCell.SetBinding(TextCell.TextColorProperty, "FieldColor");
                    return textCell;
                })
            };
            listView.SetBinding(ListView.ItemsSourceProperty, "Workouts");
            listView.ItemSelected += async (s, e) =>
            {
                var item = e.SelectedItem as PlanInfo;
                if (item != null)
                {
                    await Navigation.PushAsync(new DetailedPlanPage(item.ExerciseDetails,item.Id));
                    listView.SelectedItem = null;
                }
            };
            Title = "Workout Plan";
            Padding = new Thickness(10, 0);
            Content = new StackLayout
            {
                Children =
                {
                    listView
                }
            };
        }
    }
}
