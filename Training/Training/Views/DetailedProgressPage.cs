using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Training.ViewModels;
using Xamarin.Forms;

namespace Training.Views
{
    class DetailedProgressPage : ContentPage
    {
        public DetailedProgressPage(List<DetailedProgressInfo> info,ProgressViewModel viewModel)
        {
            BindingContext = viewModel;
            var toolbar = new ToolbarItem { Text = "Save Changes" };
            toolbar.SetBinding(MenuItem.CommandProperty, "SaveChangesCommand");
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
                var item = e.SelectedItem as DetailedProgressInfo;
                if (item != null)
                {
                    MessagingCenter.Send(this, "ChangedValue", new Tuple<string, string>(item.Id, 
                        await DisplayPromptAsync("Enter new Value", "", maxLength: 3, keyboard: Keyboard.Numeric)));
                    listView.SelectedItem = null;
                }
            };
            //FB64
            Button buttonDelete = new Button { Text = "\ufb64", FontFamily = "materialdesignicons-webfont.ttf#Material Design Icons"
                , FontSize = 50, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonDelete.CommandParameter = info;
            buttonDelete.Command = new Command<List<DetailedProgressInfo>>(async (infoList) => {
                bool answer = await DisplayAlert("Alert!", "Really delete this workout?", "Yes", "No");
                if (answer)
                {
                    viewModel.Delete(infoList);
                    await Navigation.PopAsync();
                }
            });
            Title = "Detailed Plan";
            Padding = new Thickness(10, 0);
            Content = new StackLayout
            {
                Children =
                {
                    listView,buttonDelete
                }
            };
        }
    }
}
