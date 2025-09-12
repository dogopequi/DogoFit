using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class ProfileEditDeleteExercises : ContentPage
{
	public ProfileEditDeleteExercises()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;
	}

	private void ExerciseName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is ProfileViewModel VM)
        {
            AppState.FilterByCategory(e.NewTextValue, true);
            AppState.FillDisplayedExercises(VM.DisplayedExercises);
        }
    }
}