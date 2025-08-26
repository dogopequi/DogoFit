using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker.Views;

public partial class AddExercise : ContentPage
{
	public AddExercise()
	{
		InitializeComponent();
		if (AppState.IsWorkoutInProgress)
            BindingContext = new StartRoutineViewModel();
		else if (AppState.IsEditingRoutine)
			BindingContext = new EditRoutineModel();
        else
            BindingContext = new WorkoutViewModel();
    }

    private void ExerciseName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is StartRoutineViewModel startVM)
        {
            startVM.FilterByCategory(e.NewTextValue, true);
        }
        else if (BindingContext is EditRoutineModel editVM)
        {
            editVM.FilterByCategory(e.NewTextValue, true);
        }
        else if (BindingContext is WorkoutViewModel workoutVM)
        {
            workoutVM.FilterByCategory(e.NewTextValue, true);
        }
    }

}