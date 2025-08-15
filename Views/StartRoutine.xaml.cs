using GymTracker.Models;

namespace GymTracker.Views;

public partial class StartRoutine : ContentPage
{
	public StartRoutine()
	{
		InitializeComponent();
		BindingContext = new StartRoutineViewModel();
    }
}