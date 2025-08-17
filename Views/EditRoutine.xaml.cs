using GymTracker.Models;
namespace GymTracker.Views;

public partial class EditRoutine : ContentPage
{
	public EditRoutine()
	{
		InitializeComponent();
		BindingContext = new EditRoutineModel();
	}
}