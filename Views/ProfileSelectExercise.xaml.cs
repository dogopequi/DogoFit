using GymTracker.Models;

namespace GymTracker.Views;

public partial class ProfileSelectExercise : ContentPage
{
	public ProfileSelectExercise()
	{
		InitializeComponent();
        BindingContext = new ProfileViewModel();
	}
    private void ExerciseName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is ProfileViewModel VM)
        {
            VM.FilterByCategory(e.NewTextValue, true);
        }
    }
}