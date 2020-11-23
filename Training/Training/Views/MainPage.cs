using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Training.Views
{
    [DesignTimeVisible(false)]
    public partial class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            DeviceDisplay.KeepScreenOn = true;
            Label header = new Label
            {
                Text = "Menu",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center
            };
            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem { Title = "Plan", PageType = typeof(PlanPage) });
            masterPageItems.Add(new MasterPageItem { Title = "Profile", PageType = typeof(SettingsPage) });
            masterPageItems.Add(new MasterPageItem { Title = "Progress", PageType = typeof(ProgressPage) });
            masterPageItems.Add(new MasterPageItem { Title = "Workout", PageType = typeof(WorkoutPage) });
            ListView listView = new ListView { ItemsSource = masterPageItems, ItemTemplate=new DataTemplate(()=> {
                var textCell = new TextCell { TextColor = Color.Black };
                textCell.SetBinding(TextCell.TextProperty, "Title");
                return textCell; })};
            this.Master = new ContentPage
            {
                Title = header.Text,
                Content = new StackLayout
                {
                    Children =
                    {
                        header,
                        listView
                    }
                }
            };
            //this.Detail = new NavigationPage(new ContentPage { Title = "Trainingapp" });
            listView.ItemSelected += (s, e) => 
            {
                var item = e.SelectedItem as MasterPageItem;
                if(item != null)
                {
                    Detail = new NavigationPage((Page)Activator.CreateInstance(item.PageType));
                    listView.SelectedItem = null;
                    IsPresented = false;
                }
            };
            listView.SelectedItem = masterPageItems[3];
        }
    }
    public class MasterPageItem
    {
        public string Title { get; set; }
        public Type PageType { get; set; }
    }
}
