using System;
using System.Collections.Generic;
using System.Text;
using Training.ViewModels;
using Xamarin.Forms;

namespace Training.Views
{
    class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            int size = 80;
            BindingContext = new SettingsViewModel();
            var toolbar = new ToolbarItem { Text = "Save Changes" };
            toolbar.SetBinding(ToolbarItem.CommandProperty, "SaveChangesCommand");
            this.ToolbarItems.Add(toolbar);

            Title = "Profile";
            var template = new DataTemplate(() =>
             {
                 var grid = new Grid { HeightRequest = size };
                 var textLabel = new Label {FontAttributes=FontAttributes.Bold,VerticalTextAlignment=TextAlignment.Center,HorizontalTextAlignment=TextAlignment.End };
                 textLabel.SetBinding(Label.TextProperty, "Text");
                 var valueLabel = new Label {VerticalTextAlignment = TextAlignment.Center };
                 valueLabel.SetBinding(Label.TextProperty, "Value");
                 grid.Children.Add(textLabel);
                 grid.Children.Add(valueLabel, 1, 0);
                 return new ViewCell { View = grid };
             });
            ListView listView = new ListView
            {
                ItemTemplate = template,
                VerticalOptions = LayoutOptions.End,
                HeightRequest = 3 * size
            };
            listView.SetBinding(ListView.ItemsSourceProperty, "Items");
            listView.ItemSelected += async (s, e) =>
            {
                var it = e.SelectedItem as ProfileItem;
                MessagingCenter.Send(this, "ChangedValue", new Tuple<string, string>(it.Text, await DisplayPromptAsync("Enter new Value", "", maxLength: 3, keyboard: Keyboard.Numeric)));
            };
            ListView listView2 = new ListView
            {
                ItemTemplate = template
            };
            listView2.SetBinding(ListView.ItemsSourceProperty, "MaxReps");
            listView2.ItemSelected += (s, e) =>
            {
                listView2.SelectedItem = null;
            };
            var button = new Button { Text = "Export Progress" };
            button.SetBinding(Button.CommandProperty, "ExportProgress");
            Content = new StackLayout
            {
                Children =
                {
                    new Label{Text="Settings",FontSize=30,VerticalOptions=LayoutOptions.Start},listView,new Label{Text="Max Reps",FontSize=30},listView2,button
                }
            };
        }
    }
}
