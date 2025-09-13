using CommunityToolkit.Maui.Extensions;
using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker.Views;

public partial class StartRoutine : ContentPage
{
    private StartRoutineViewModel vm;
	public StartRoutine()
	{
		InitializeComponent();
        vm = new StartRoutineViewModel();
        BindingContext = vm;
        addexercisebutton.Clicked += async (s, e) =>
        {
            vm.OnAddExercise();
            Refresh();
        };
        Refresh();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }

    private void Refresh()
    {
        UpdateHeaderLabels();
        eVstack.Clear();
        CreateWorkoutLayout();
    }

    private async void SetOptions_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button
            && button.BindingContext is Set set
            && button.CommandParameter is Exercise exercise
            && BindingContext is StartRoutineViewModel vm)
        {
            var popup = new Popups.SetOptionsPopup();
            await this.ShowPopupAsync(popup);

            string? action = await popup.WaitForResultAsync();
            if (action is null) return;

            switch (action)
            {
                case "Normal":
                    vm.OnEditSetNormal(set);
                    break;
                case "Failure":
                    vm.OnEditSetFailure(set);
                    break;
                case "Drop":
                    vm.OnEditSetDrop(set);
                    break;
                case "Warmup":
                    vm.OnEditSetWarmup(set);
                    break;

                case "Remove":
                    vm.OnRemoveSet(exercise, set);
                    break;
            }

            Refresh();
        }
    }

    private void CreateWorkoutLayout()
    {
        if (vm.routine == null)
            return;
        foreach(var exercise in vm.routine.Exercises)
        {
            Grid eheader = new Grid { Padding = new Thickness(0,0,0,-20) };
            eheader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            eheader.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            Label name = new Label { Text = exercise.Name, TextColor = Color.FromArgb("#008cff"), FontSize = 20, FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(20,0,0,20)};
            name.BindingContext = exercise;
            Button remove = new Button { Text = "❌", FontSize = 15, Margin = new Thickness(0,-20,0,0), BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Red, HorizontalOptions = LayoutOptions.End};
            remove.Clicked += (s, e) => {
                vm.OnRemoveExercise(exercise); Refresh();
            };
            Grid.SetColumn(name, 0);
            Grid.SetColumn(remove, 1);
            eheader.Children.Add(name);
            eheader.Children.Add(remove);
            eVstack.Children.Add(eheader);


            Entry desc = new Entry { Placeholder = "Enter your exercise description here", WidthRequest = 300, HorizontalOptions = LayoutOptions.Center, FontSize = 15 };
            desc.BindingContext = exercise;
            desc.SetBinding(Entry.TextProperty, new Binding("Description", mode: BindingMode.TwoWay));
            eVstack.Children.Add(desc);

            if (exercise.IsUnilateral)
            {
                Label leftHeader = new Label { Text = "Left Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
                eVstack.Children.Add(leftHeader);
                SetLayout(exercise.Sets.Where(s => s.Side == SideType.Left), exercise);

                Button addsetbuttonLeft = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                    WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbuttonLeft.BindingContext = exercise;
                addsetbuttonLeft.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Left); Refresh(); };
                eVstack.Children.Add(addsetbuttonLeft);

                Label rightHeader = new Label { Text = "Right Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
                eVstack.Children.Add(rightHeader);
                SetLayout(exercise.Sets.Where(s => s.Side == SideType.Right), exercise);

                Button addsetbuttonRight = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                    WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbuttonRight.BindingContext = exercise;
                addsetbuttonRight.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Right); Refresh(); };
                eVstack.Children.Add(addsetbuttonRight);
            }
            else
            {
                SetLayout(exercise.Sets, exercise);
                Button addsetbutton = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                        WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbutton.BindingContext = exercise;
                addsetbutton.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.None); Refresh(); };
                eVstack.Children.Add(addsetbutton);
            }

            BoxView box = new BoxView { HeightRequest = 5, BackgroundColor = Color.FromArgb("#2b2b2b"), HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(0,20,0,0)};
            eVstack.Children.Add(box);


        }
    }

    private void SetLayout(IEnumerable<Set> sets, Exercise exercise)
    {
        
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            RowDefinitions = { },
            HorizontalOptions = LayoutOptions.Fill,
            ColumnSpacing = 0,
            RowSpacing = 20,
            BackgroundColor = Colors.Transparent
        };

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        Label setname = new Label { Text = "SET", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.Transparent };
        Label lastname = new Label { Text = "LAST", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.Transparent };
        Label repsname = new Label { Text = "REPS", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.Transparent };
        Label weightname = new Label { Text = "WEIGHT", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.Transparent };
        Label empty = new Label { Text = "", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold, BackgroundColor = Colors.Transparent };
        grid.Add(setname, 0, 0);
        grid.Add(lastname, 1, 0);
        grid.Add(repsname, 2, 0);
        grid.Add(weightname, 3, 0);
        grid.Add(empty, 4, 0);
        int row = 1;
        foreach (Set set in sets)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(35) });
            Button setbutton = new Button { FontAttributes = FontAttributes.Bold, FontSize = 16, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Colors.Black };
            switch(set.Type)
            {
                case SetType.Normal:
                    setbutton.Text = set.ID.ToString();
                    setbutton.TextColor = Colors.White;
                    break;
                case SetType.Warmup:
                    setbutton.Text = "W";
                    setbutton.TextColor = Colors.Orange;
                    break;
                case SetType.Failure:
                    setbutton.Text = "F";
                    setbutton.TextColor = Colors.Red;
                    break;
                case SetType.Drop:
                    setbutton.Text = "D";
                    setbutton.TextColor = Color.FromArgb("#008cff");
                    break;
            }
            setbutton.BindingContext = set;
            setbutton.CommandParameter = exercise;
            setbutton.Clicked += SetOptions_Clicked;
            setbutton.Clicked += (s, e) => Refresh();


            
            Label lastvalue = new Label { Text = set.LastSet, BackgroundColor = Colors.Transparent, TextColor = Colors.Gray, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 3, 0, 0), FontSize = 15, WidthRequest = 50, MaxLines = 1};
           
            Entry repsvalue = new Entry { Placeholder = "0", BackgroundColor = Colors.Transparent, Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 16,
                    TextColor = Colors.White, HeightRequest = 40};
            repsvalue.BindingContext = set;
            repsvalue.SetBinding(Entry.TextProperty, new Binding("Reps", mode: BindingMode.TwoWay));

            
            Entry weightvalue = new Entry { Placeholder = "0", BackgroundColor = Colors.Transparent, Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 16,
                    TextColor = Colors.White, HeightRequest = 40};
            weightvalue.BindingContext = set;
            weightvalue.SetBinding(Entry.TextProperty, new Binding("Weight", mode: BindingMode.TwoWay));
           

            CheckBox checkbox = new CheckBox { HeightRequest = 40, BackgroundColor = Colors.Transparent, Scale = 1, IsChecked = set.IsChecked };
            checkbox.BindingContext = set;
            checkbox.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsChecked", mode: BindingMode.TwoWay));
            checkbox.CheckedChanged += (s, e) => { UpdateHeaderLabels(); };

            grid.Add(setbutton, 0, row);
            grid.Add(lastvalue, 1, row);
            grid.Add(repsvalue, 2, row);
            grid.Add(weightvalue, 3, row);
            grid.Add(checkbox, 4, row);
            row++;
        }
        eVstack.Children.Add(grid);

    }
    private void UpdateHeaderLabels()
    {
        if (vm.routine != null)
        {
            int sets = vm.routine.Exercises.Sum(s => s.CheckedSets.Count());
            int reps = vm.routine.Exercises.SelectMany(s => s.CheckedSets).Sum(r => r.Reps);
            double volume = vm.routine.Exercises.SelectMany(w => w.CheckedSets).Sum(s => s.Weight * s.Reps);
            SetsLabel.Text = sets.ToString();
            RepsLabel.Text = reps.ToString();
            VolumeLabel.Text = volume.ToString();
        }
    }
}