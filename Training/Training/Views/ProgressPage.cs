using System;
using System.Collections.Generic;
using System.Text;
using Training.ViewModels;
using Xamarin.Forms;

namespace Training.Views
{
    class ProgressPage:ContentPage
    {
        public ProgressPage()
        {
            ProgressViewModel viewModel;
            BindingContext =viewModel=new ProgressViewModel();
            ListView listView = new ListView
            {
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell {TextColor=Color.Black};
                    textCell.SetBinding(TextCell.TextProperty, "Title");
                    textCell.SetBinding(TextCell.DetailProperty, "Exercises");
                    return textCell;
                })
            };
            listView.SetBinding(ListView.ItemsSourceProperty, "Workouts");
            listView.ItemSelected += async (s, e) =>
            {
                var item = e.SelectedItem as ProgressInfo;
                if (item != null)
                {
                    await Navigation.PushAsync(new DetailedProgressPage(item.ExerciseDetails,viewModel));
                    listView.SelectedItem = null;
                }
            };
            Title = "Progress Overview";
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
