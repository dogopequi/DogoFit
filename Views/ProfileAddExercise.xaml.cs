using GymTracker.Services;
namespace GymTracker.Views;

public partial class ProfileAddExercise : ContentPage
{
	private string exerciseName = "";
	public string ExerciseName
	{
	    get => exerciseName;
	    set
	    {
	        if (exerciseName != value)
	        {
	            exerciseName = value;
	            OnPropertyChanged(nameof(ExerciseName));
	        }
	    }
	}
	private bool isUnilateral = false;
	public bool IsUnilateral
	{
	    get => isUnilateral;
	    set
	    {
	        if (isUnilateral != value)
	        {
	            isUnilateral = value;
	            OnPropertyChanged(nameof(IsUnilateral));
	        }
	    }
	}

	public Muscles SelectedMuscle = Muscles.Biceps;
	public MuscleFunctions SelectedFunction = MuscleFunctions.Pull;
	public MuscleGroups SelectedGroup = MuscleGroups.Arms;
	public List<Muscles> SelectedSecondaryMuscles = new List<Muscles>();

	public Dictionary<Button, Muscles> MuscleButtons = new Dictionary<Button, Muscles>();
	public Dictionary<Button, MuscleFunctions> FunctionButtons = new Dictionary<Button, MuscleFunctions>();
	public Dictionary<Button, MuscleGroups> GroupButtons = new Dictionary<Button, MuscleGroups>();
	public Dictionary<Button, Muscles> SecondaryMuscleButtons = new Dictionary<Button, Muscles>();
	public ProfileAddExercise()
	{
		InitializeComponent();
		if(AppState.profileExercise == ProfileExercise.Edit)
		{
			SelectedMuscle = AppState.EditedExercise.TargetMuscle;
			SelectedFunction = AppState.EditedExercise.Function;
			SelectedGroup = AppState.EditedExercise.MuscleGroup;
			SelectedSecondaryMuscles = AppState.EditedExercise.SecondaryMuscles;
			IsUnilateral = AppState.EditedExercise.IsUnilateral;
		}
		foreach(Muscles muscle in AppState.MusclesList)
		{
			Button muscleButton = new Button { Text = AppState.MuscleToString(muscle), TextColor = Colors.White, FontAttributes = FontAttributes.None, FontSize = 15, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.FromRgba("#2b2b2b") };
			muscleButton.Clicked += (s, e) => {
				SelectedMuscle = muscle;
				UpdateButtonColors();
			};
			MuscleButtons.Add(muscleButton, muscle);
		}
		foreach(MuscleFunctions function in AppState.MuscleFunctionsList)
		{
			Button muscleButton = new Button { Text = AppState.MuscleFunctionToString(function), TextColor = Colors.White, FontAttributes = FontAttributes.None, FontSize = 15, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.FromRgba("#2b2b2b") };
			muscleButton.Clicked += (s, e) => {
				SelectedFunction = function;
				UpdateButtonColors();
			};
			FunctionButtons.Add(muscleButton, function);
		}
		foreach(MuscleGroups group in AppState.MuscleGroupsList)
		{
			Button muscleButton = new Button { Text = AppState.MuscleGroupToString(group), TextColor = Colors.White,FontAttributes = FontAttributes.None, FontSize = 15, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.FromRgba("#2b2b2b") };
			muscleButton.Clicked += (s, e) => {
				SelectedGroup = group;
				UpdateButtonColors();
			};
			GroupButtons.Add(muscleButton, group);
		}
		foreach(Muscles secmuscle in AppState.MusclesList)
		{
			Button muscleButton = new Button { Text = AppState.MuscleToString(secmuscle), TextColor = Colors.White,FontAttributes = FontAttributes.None, FontSize = 15, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.FromRgba("#2b2b2b") };
			muscleButton.Clicked += (s, e) => {
				if (SelectedSecondaryMuscles.Contains(secmuscle))
				    SelectedSecondaryMuscles.Remove(secmuscle);
				else
				    SelectedSecondaryMuscles.Add(secmuscle);
				UpdateButtonColors();
			};
			SecondaryMuscleButtons.Add(muscleButton, secmuscle);
		}
		CreateLayout();
		UpdateButtonColors();
	}

	private void UpdateButtonColors()
	{
		foreach(var m in MuscleButtons)
		{
			if (SelectedMuscle != m.Value)
				m.Key.BackgroundColor = Color.FromRgba("#2b2b2b");
			else
				m.Key.BackgroundColor = Color.FromRgba("#008cff");
		}
		foreach(var f in FunctionButtons)
		{
			if (SelectedFunction != f.Value)
				f.Key.BackgroundColor = Color.FromRgba("#2b2b2b");
			else
				f.Key.BackgroundColor = Color.FromRgba("#008cff");
		}
		foreach(var g in GroupButtons)
		{
			if (SelectedGroup != g.Value)
				g.Key.BackgroundColor = Color.FromRgba("#2b2b2b");
			else
				g.Key.BackgroundColor = Color.FromRgba("#008cff");
		}
		foreach (var sm in SecondaryMuscleButtons)
		{
		    if (SelectedSecondaryMuscles.Contains(sm.Value))
		        sm.Key.BackgroundColor = Color.FromRgba("#008cff");
		    else
		        sm.Key.BackgroundColor = Color.FromRgba("#2b2b2b");
		}
	}

	private void CreateLayout()
	{
		string placeholder = "Enter the exercise name here";
		if (AppState.profileExercise == ProfileExercise.Edit)
			placeholder = AppState.EditedExercise.Name;
		Entry name = new Entry { Placeholder = placeholder, WidthRequest = 300, HorizontalOptions = LayoutOptions.Start,
				HorizontalTextAlignment = TextAlignment.Start, FontSize = 15};
		name.BindingContext = this;
		name.SetBinding(Entry.TextProperty, new Binding("ExerciseName", mode: BindingMode.TwoWay));
		vstack.Children.Add(name);

		AddControls<Button, Muscles>("Target Muscle", MuscleButtons);
		AddControls<Button, MuscleFunctions>("Muscle Function", FunctionButtons);
		AddControls<Button, MuscleGroups>("Muscle Group", GroupButtons);
		AddControls<Button, Muscles>("Secondary Muscles", SecondaryMuscleButtons);


		HorizontalStackLayout checkHstack = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
		Label checkLabel = new Label { Text = "Is this exercise unilateral?",  TextColor = Colors.White, FontSize = 15, FontAttributes = FontAttributes.None,
                    HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(0,0,0,12), VerticalOptions = LayoutOptions.End};
		checkHstack.Children.Add(checkLabel);

		CheckBox checkbox = new CheckBox { Margin = new Thickness(0, 15, 0, 0), Scale = 1.2, IsChecked = IsUnilateral };
		checkbox.BindingContext = this;
        checkbox.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsUnilateral", mode: BindingMode.TwoWay));
		checkHstack.Children.Add(checkbox);

		vstack.Children.Add(checkHstack);

		Button addExercise = new Button { Text = "Continue", TextColor = Colors.White, FontAttributes = FontAttributes.None, FontSize = 15, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.FromRgba("#008cff") };
		addExercise.Clicked += async (s, e) => {
			Exercise exercise = new Exercise { Name = ExerciseName, Function = SelectedFunction, MuscleGroup = SelectedGroup, TargetMuscle = SelectedMuscle,
					SecondaryMuscles = new List<Muscles>(SelectedSecondaryMuscles), IsUnilateral = IsUnilateral};
			if (AppState.profileExercise == ProfileExercise.Add)
			{
				var result = AppState.AddExercise(exercise);
				if (result.Item1 == false)
				{
					string error = "Can't add exercise due to the following errors: \n";
					error += result.Item2;

					await Shell.Current.DisplayAlert("Error", error, "OK");
					return;
				}
				await Shell.Current.DisplayAlert("Success", "Exercise has been added", "OK");
			}
			else if(AppState.profileExercise == ProfileExercise.Edit)
			{
				var r = AppState.UpdateExercise(exercise);
				await Shell.Current.DisplayAlert("Info", r, "OK");
			}
			else
				await Shell.Current.DisplayAlert("Error", "DogoFit has encountered an unknown error. Please restart.", "OK");
			await Shell.Current.Navigation.PopToRootAsync();
		};
		vstack.Children.Add(addExercise);
	}

	private void AddControls<K, V>(string text, Dictionary<K, V> buttons) where K : View
	{
		Label label = new Label { Text = text,  TextColor = Colors.White, FontSize = 18, FontAttributes = FontAttributes.Bold,
                    HorizontalTextAlignment = TextAlignment.Center, Margin = new Thickness(20,0,0,20)};
		vstack.Children.Add(label);

		ScrollView scroll = new ScrollView { Orientation = ScrollOrientation.Horizontal };
		HorizontalStackLayout hstack = new HorizontalStackLayout { Spacing = 10, HorizontalOptions = LayoutOptions.Center, Margin = new Thickness(0,0,0,30)};
		foreach(var button in buttons)
			hstack.Children.Add(button.Key);
		scroll.Content = hstack;
		vstack.Children.Add(scroll);
	}
}