using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker.Views;

public partial class AddExercise : ContentPage
{
	public AddExercise()
	{
		InitializeComponent();
        switch(AppState.WorkoutState)
        {
            case WorkoutStates.EmptyWorkout:
                BindingContext = new WorkoutViewModel();
                break;
            case WorkoutStates.WorkoutInProgress:
                BindingContext = new StartRoutineViewModel();
                break;
            case WorkoutStates.EditingRoutine:
                BindingContext = new EditRoutineModel();
                break;
            case WorkoutStates.NewRoutine:
                BindingContext = new WorkoutViewModel();
                break;
            default:
                BindingContext = new WorkoutViewModel();
                break;
        }
    }

    private void ExerciseName_TextChanged(object sender, TextChangedEventArgs e)
    {
        switch (BindingContext)
        {
            case WorkoutViewModel workoutVM:
                AppState.FilterByCategory(e.NewTextValue, true);
                AppState.FillDisplayedExercises(workoutVM.DisplayedExercises);
                break;

            case StartRoutineViewModel startVM:
                AppState.FilterByCategory(e.NewTextValue, true);
                AppState.FillDisplayedExercises(startVM.DisplayedExercises);
                break;

            case EditRoutineModel editVM:
                AppState.FilterByCategory(e.NewTextValue, true);
                AppState.FillDisplayedExercises(editVM.DisplayedExercises);
                break;
        }
    }

}