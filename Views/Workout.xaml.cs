using GymTracker.Models;

namespace GymTracker;

public partial class Workout : ContentPage
{
	public Workout()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }
}