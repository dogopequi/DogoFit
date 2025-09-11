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
                vm.OnRemoveExercise(exercise);
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
                foreach (var set in exercise.Sets.Where(s => s.Side == SideType.Left))
                    SetLayout(set, exercise);

                Button addsetbuttonLeft = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                    WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbuttonLeft.BindingContext = exercise;
                addsetbuttonLeft.Clicked += (s, e) => { vm.OnAddSetToExercise(exercise, SideType.Left); Refresh(); };
                eVstack.Children.Add(addsetbuttonLeft);

                Label rightHeader = new Label { Text = "Right Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center };
                eVstack.Children.Add(rightHeader);
                foreach (var set in exercise.Sets.Where(s => s.Side == SideType.Right))
                    SetLayout(set, exercise);

                Button addsetbuttonRight = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                    WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbuttonRight.BindingContext = exercise;
                addsetbuttonRight.Clicked += (s, e) => { vm.OnAddSetToExercise(exercise, SideType.Right); Refresh(); };
                eVstack.Children.Add(addsetbuttonRight);
            }
            else
            {
                foreach (Set set in exercise.Sets)
                {
                    SetLayout(set, exercise);
                }
                Button addsetbutton = new Button {Text = "Add set", FontSize = 14, BackgroundColor = Color.FromArgb("#2b2b2b"), BorderWidth = 0, TextColor = Colors.White, Padding = 5,
                        WidthRequest = 275, FontAttributes = FontAttributes.Bold};
                addsetbutton.BindingContext = exercise;
                addsetbutton.Clicked += (s, e) => { vm.OnAddSetToExercise(exercise, SideType.None); Refresh(); };
                eVstack.Children.Add(addsetbutton);
            }

            BoxView box = new BoxView { HeightRequest = 5, BackgroundColor = Color.FromArgb("#2b2b2b"), HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(0,20,0,0)};
            eVstack.Children.Add(box);


        }
    }

    private void SetLayout(Set set, Exercise exercise)
    {
        HorizontalStackLayout setsHstack = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };

        VerticalStackLayout setVstack = new VerticalStackLayout { Spacing = 10 };

        Label setname = new Label { Text = "Set", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };

        Button setbutton = new Button { FontAttributes = FontAttributes.Bold, FontSize = 20, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Colors.Black };
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

        setVstack.Children.Add(setname);
        setVstack.Children.Add(setbutton);
        setsHstack.Children.Add(setVstack);

        VerticalStackLayout lastVstack = new VerticalStackLayout { Spacing = 10 };
        Label lastname = new Label { Text = "Last", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        Label lastvalue = new Label { Text = set.LastSet, TextColor = Colors.Gray, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 3, 0, 0), FontSize = 15, WidthRequest = 50, MaxLines = 1};
        lastVstack.Children.Add(lastname);
        lastVstack.Children.Add(lastvalue);
        setsHstack.Children.Add(lastVstack);

        VerticalStackLayout repsVstack = new VerticalStackLayout { Spacing = 10 };
        Label repsname = new Label { Text = "Reps", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        Entry repsvalue = new Entry { Placeholder = "0", Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 20,
                TextColor = Colors.White};
        repsvalue.BindingContext = set;
        repsvalue.SetBinding(Entry.TextProperty, new Binding("Reps", mode: BindingMode.TwoWay));
        repsVstack.Children.Add(repsname);
        repsVstack.Children.Add(repsvalue);
        setsHstack.Children.Add(repsVstack);

        VerticalStackLayout weightVstack = new VerticalStackLayout { Spacing = 10 };
        Label weightname = new Label { Text = "Weight", HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        Entry weightvalue = new Entry { Placeholder = "0", Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 20,
                TextColor = Colors.White};
        weightvalue.BindingContext = set;
        weightvalue.SetBinding(Entry.TextProperty, new Binding("Weight", mode: BindingMode.TwoWay));
        weightVstack.Children.Add(weightname);
        weightVstack.Children.Add(weightvalue);
        setsHstack.Children.Add(weightVstack);

        CheckBox checkbox = new CheckBox { Margin = new Thickness(0, 15, 0, 0), Scale = 1.2, IsChecked = set.IsChecked };
        setsHstack.Children.Add(checkbox);
        checkbox.BindingContext = set;
        checkbox.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsChecked", mode: BindingMode.TwoWay));
        checkbox.CheckedChanged += (s, e) => { UpdateHeaderLabels(); };

        eVstack.Children.Add(setsHstack);
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