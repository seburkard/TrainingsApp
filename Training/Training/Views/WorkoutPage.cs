using System;
using System.Collections.Generic;
using System.Text;
using Training.ViewModels;
using Xamarin.Forms;

namespace Training.Views
{
    class CustomLabel : Label
    {
        public CustomLabel() : base()
        {
            FontAttributes = FontAttributes.Bold;
            HorizontalOptions = LayoutOptions.CenterAndExpand;
            VerticalOptions = LayoutOptions.CenterAndExpand;
            HorizontalTextAlignment = TextAlignment.Center; VerticalTextAlignment = TextAlignment.Center;
        }
    }
    class WorkoutPage : ContentPage
    {
        public WorkoutPage()
        {
            var ID = Guid.NewGuid().ToString();
            string family = "materialdesignicons-webfont.ttf#Material Design Icons";
            BindingContext = new WorkoutViewModel(ID);
            var toolbar = new ToolbarItem { Text = "Save Workout"};
            toolbar.SetBinding(ToolbarItem.CommandProperty, "SaveCommand");
            this.ToolbarItems.Add(toolbar);
            ProgressBar progressBar = new ProgressBar();
            progressBar.SetBinding(ProgressBar.ProgressProperty, "Progress");
            var label1 = new CustomLabel { FontSize = 36,VerticalOptions=LayoutOptions.EndAndExpand };
            label1.SetBinding(Label.TextProperty, "Titles[0].Text");
            var label2 = new CustomLabel { FontSize = 36};
            label2.SetBinding(Label.TextProperty, "Titles[1].Text");
            var grid = new Grid {VerticalOptions = LayoutOptions.StartAndExpand };
            for (int i = 0; i < 3; i++)
            {
                var label = new Label {FontFamily= family,HorizontalOptions=LayoutOptions.End,FontSize=40,VerticalOptions=LayoutOptions.Center };
                label.SetBinding(Label.TextProperty, "Picture[" + i.ToString() + "].Text");
                grid.Children.Add(label, i * 2, 0);
                label = new Label { HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center,FontSize=30 };
                label.SetBinding(Label.TextProperty, "Text[" + i.ToString() + "].Text");
                grid.Children.Add(label, i * 2+1, 0);
            }
            var label4 = new CustomLabel { FontSize = 20 };
            label4.SetBinding(Label.TextProperty, "Titles[3].Text");
            var label5 = new CustomLabel { FontSize = 20 };
            label5.SetBinding(Label.TextProperty, "Titles[4].Text");
            var timerLabel = new CustomLabel { FontSize = 50,TextColor=Color.Black, VerticalOptions = LayoutOptions.EndAndExpand };
            timerLabel.SetBinding(Label.TextProperty, "Time");
            // f40a or f40d
            Button buttonStart = new Button { Text = "\uf40a", FontFamily = family,FontSize=50, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonStart.SetBinding(Button.CommandProperty, "StartTimerCommand");
            // f3e6 or f3e4
            Button buttonStop = new Button { Text = "\uf3e4",FontFamily=family, FontSize = 50, BackgroundColor=Color.Transparent, HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonStop.SetBinding(Button.CommandProperty, "StopTimerCommand");
            // f667 or F99A
            Button buttonReset = new Button { Text = "\uf99a", FontFamily = family, FontSize = 50, BackgroundColor = Color.Transparent, HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonReset.SetBinding(Button.CommandProperty, "ResetTimerCommand");


            var leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
            leftSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "NextCommand");
            var rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
            rightSwipeGesture.SetBinding(SwipeGestureRecognizer.CommandProperty, "LastCommand");

            Button buttonLast = new Button { Text = "Last",HorizontalOptions=LayoutOptions.FillAndExpand};
            buttonLast.SetBinding(Button.CommandProperty, "LastCommand");
            Button buttonNext = new Button { Text = "Next", HorizontalOptions = LayoutOptions.FillAndExpand };
            buttonNext.SetBinding(Button.CommandProperty, "NextCommand");

            Title = "Workout";
            var content= new StackLayout
            {
                Children =
                {
                    progressBar,label1,label2,grid,label4,label5,
                    timerLabel,new StackLayout{BackgroundColor=Color.DarkGray, Children={timerLabel,new StackLayout{Orientation=StackOrientation.Horizontal,Children={buttonStart,buttonStop,buttonReset}} } }
                    //,new StackLayout{Orientation=StackOrientation.Horizontal,Children={buttonLast,buttonNext} }
                }
            };
            content.GestureRecognizers.Add(leftSwipeGesture);
            content.GestureRecognizers.Add(rightSwipeGesture);
            Content = content;
            MessagingCenter.Subscribe<WorkoutViewModel>(this, ID, async (s) => {
                MessagingCenter.Send(this, "PopUpResult"+ID,await DisplayPromptAsync("Enter Value", "", maxLength: 3, keyboard: Keyboard.Numeric));
            });
        }
    }
}
