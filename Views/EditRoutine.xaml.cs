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
                SetLayout(exercise.Sets.Where(s => s.Side == SideType.Left), exercise);
                Button addSetLeft = new Button { Text = "Add Set", FontSize = 14, BackgroundColor = Color.FromRgba("#2b2b2b"), WidthRequest = 275, Padding = 5,
                        TextColor = Colors.White, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0,10,0,0)};
                addSetLeft.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Left); Refresh(); };
                vstack.Children.Add(addSetLeft);


                Label rightHeader = new Label { Text = "Right Side", TextColor = Colors.White, FontSize = 18, HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, Padding = 20 };
                vstack.Children.Add(rightHeader);
                SetLayout(exercise.Sets.Where(s => s.Side == SideType.Right), exercise);
                Button addSetRight = new Button { Text = "Add Set", FontSize = 14, BackgroundColor = Color.FromRgba("#2b2b2b"), WidthRequest = 275, Padding = 5,
                        TextColor = Colors.White, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0,10,0,0)};
                addSetRight.Clicked += (s, e) => { AppState.OnAddSetToExercise(exercise, SideType.Right); Refresh(); };
                vstack.Children.Add(addSetRight);
            }
            else
            {
                SetLayout(exercise.Sets, exercise);
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

    private void SetLayout(IEnumerable<Set> sets, Exercise exercise)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
            },
            RowDefinitions = { },
            HorizontalOptions = LayoutOptions.Fill,
            ColumnSpacing = 0,
            RowSpacing = 20, BackgroundColor = Colors.Transparent
        };

        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        Label setname = new Label { Text = "Set", BackgroundColor = Colors.Transparent, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        Label repsname = new Label { Text = "Reps", BackgroundColor = Colors.Transparent, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        Label weightname = new Label { Text = "Weight", BackgroundColor = Colors.Transparent, HorizontalTextAlignment = TextAlignment.Center, TextColor = Colors.LightGray, FontAttributes = FontAttributes.Bold };
        grid.Add(setname, 0, 0);
        grid.Add(repsname, 1, 0);
        grid.Add(weightname, 2, 0);
        int row = 1;
        foreach(Set set in sets)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
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
            setbutton.Clicked += async (s, e) =>
            {
                await SetOptions_Clicked(s, e);
                Refresh();
            };

        
            Entry repsvalue = new Entry { Placeholder = "0", BackgroundColor = Colors.Transparent, Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 16,
                    TextColor = Colors.White, HeightRequest = 40};
            repsvalue.BindingContext = set;
            repsvalue.SetBinding(Entry.TextProperty, new Binding("Reps", mode: BindingMode.TwoWay));

        
            Entry weightvalue = new Entry { Placeholder = "0", BackgroundColor = Colors.Transparent, Keyboard = Keyboard.Numeric, WidthRequest = 50, HorizontalTextAlignment = TextAlignment.Center, FontSize = 16,
                    TextColor = Colors.White, HeightRequest = 40};
            weightvalue.BindingContext = set;
            weightvalue.SetBinding(Entry.TextProperty, new Binding("Weight", mode: BindingMode.TwoWay));
            grid.Add(setbutton, 0, row);
            grid.Add(repsvalue, 1, row);
            grid.Add(weightvalue, 2, row);
            row++;

        }
        vstack.Children.Add(grid);
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