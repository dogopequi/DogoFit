using GymTracker.Models;

namespace GymTracker.Views;

public partial class CreateRoutine : ContentPage
{
	public CreateRoutine()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }
}