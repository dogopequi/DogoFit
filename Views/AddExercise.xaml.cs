using GymTracker.Models;

namespace GymTracker.Views;

public partial class AddExercise : ContentPage
{
	public AddExercise()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }
}