using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker;

public partial class Workout : ContentPage
{
	public Workout()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as WorkoutViewModel;
        if (vm != null)
        {
            vm.WorkoutInProgress = AppState.WorkoutInProgress;
            if (AppState.IsWorkoutInProgress == false)
                vm.OnDiscard();
        }
    }
}