using CommunityToolkit.Maui.Extensions;
using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class EditRoutine : ContentPage
{
    private EditRoutineModel vm;
	public EditRoutine()
	{
		InitializeComponent();
        vm = new EditRoutineModel();
        BindingContext = vm;
        Refresh();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Refresh();
    }
    private void CreateLayout()
    {
        foreach (Exercise exercise in vm.routine.Exercises)
        {
            Grid grid = new Grid { Padding = new Thickness(0, 0, 0, 10) };
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });

            Label name = new Label { Text = exercise.Name, FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#008cff"),
                    HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0,0,0,20)};
            Button delete = new Button { Text = "X", FontSize = 20, Margin = new Thickness(0,-20,0,0), BackgroundColor = Colors.Transparent,
                    TextColor = Colors.Red, HorizontalOptions = LayoutOptions.End};
            delete.Clicked += async (s, e) => { await vm.OnRemoveExercise(exercise); Refresh(); };
            Grid.SetColumn(name, 0);
            Grid.SetColumn(delete, 1);
            grid.Children.Add(name);
            grid.Children.Add(delete);
            vstack.Children.Add(grid);

            Entry desc = new Entry { Placeholder = "Enter your exercise description here", WidthRequest = 300, HorizontalOptions = LayoutOptions.Center, FontSize = 15 };
            desc.BindingContext = exercise;
            desc.SetBinding(Entry.TextProperty, new Binding("Description", mode: BindingMode.TwoWay));
            vstack.Children.Add(desc);

            if (exercise.IsUnilateral)
            {
                Label leftHeader = new Label { Text = "Left Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, Padding = 20 };
                vstack.Children.Add(leftHeader);
                foreach (var set in exercise.Sets.Where(s => s.Side == SideType.Left))
                    SetLayout(set, exercise);
                Button addSetLeft = new Button { Text = "Add Set", FontSize = 14, BackgroundColor = Color.FromRgba("#2b2b2b"), WidthRequest = 275, Padding = 5,
                        TextColor = Colors.White, FontAttributes = FontAttributes.Bold};
                addSetLeft.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Left); Refresh(); };
                vstack.Children.Add(addSetLeft);


                Label rightHeader = new Label { Text = "Right Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, Padding = 20 };
                vstack.Children.Add(rightHeader);
                foreach (var set in exercise.Sets.Where(s => s.Side == SideType.Right))
                    SetLayout(set, exercise);
                Button addSetRight = new Button { Text = "Add Set", FontSize = 14, BackgroundColor = Color.FromRgba("#2b2b2b"), WidthRequest = 275, Padding = 5,
                        TextColor = Colors.White, FontAttributes = FontAttributes.Bold};
                addSetRight.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Right); Refresh(); };
                vstack.Children.Add(addSetRight);
            }
            else
            {
                foreach (Set set in exercise.Sets)
                {
                    SetLayout(set, exercise);
                }
                Button addSet = new Button { Text = "Add Set", FontSize = 14, BackgroundColor = Color.FromRgba("#2b2b2b"), WidthRequest = 275, Padding = 5,
                        TextColor = Colors.White, FontAttributes = FontAttributes.Bold};
                addSet.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.None); Refresh(); };
                vstack.Children.Add(addSet);
            }

            BoxView box = new BoxView { HeightRequest = 5, BackgroundColor = Color.FromArgb("#2b2b2b"), HorizontalOptions = LayoutOptions.FillAndExpand,
                    Margin = new Thickness(0,20,0,0)};
            vstack.Children.Add(box);
        }
    }

    private void Refresh()
    {
        vstack.Clear();
        CreateLayout();
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
        setbutton.Clicked += async (s, e) =>
        {
            await SetOptions_Clicked(s, e);
            Refresh();
        };

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


        vstack.Children.Add(setsHstack);
    }
    private async Task SetOptions_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button
            && button.BindingContext is Set set
            && button.CommandParameter is Exercise exercise
            && BindingContext is EditRoutineModel vm)
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
        }
    }
}