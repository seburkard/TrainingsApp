using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Training.ViewModels;
using Xamarin.Forms;

namespace Training.Views
{
    class DetailedPlanPage : ContentPage
    {
        private string id;
        public DetailedPlanPage(List<DetailedInfo> info,string id)
        {
            this.id = id;
            var toolbar = new ToolbarItem { Text = "Set as Workout" };
            toolbar.Clicked += Toolbar_Clicked;
            this.ToolbarItems.Add(toolbar);
            ListView listView = new ListView
            {
                ItemsSource=info,
                ItemTemplate = new DataTemplate(() =>
                {
                    var textCell = new TextCell { TextColor = Color.Black };
                    textCell.SetBinding(TextCell.TextProperty, "ExName");
                    textCell.SetBinding(TextCell.DetailProperty, "RepText");
                    return textCell;
                })
            };

            listView.ItemSelected += async (s, e) =>
            {
                var item = e.SelectedItem as DetailedInfo;
                if (item != null)
                {
                    await Navigation.PushAsync(new ContentPage());
                    listView.SelectedItem = null;
                }
            };

            Title = "Detailed Plan";
            Padding = new Thickness(10, 0);
            Content = new StackLayout
            {
                Children =
                {
                    listView
                }
            };
        }

        async private void Toolbar_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "SetDay",id);
            await Navigation.PopAsync();
        }
    }
}
