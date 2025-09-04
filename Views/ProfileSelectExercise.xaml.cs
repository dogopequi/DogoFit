using GymTracker.Models;

namespace GymTracker.Views;

public partial class ProfileSelectExercise : ContentPage
{
	public ProfileSelectExercise()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;
    }
    private void ExerciseName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (BindingContext is ProfileViewModel VM)
        {
            VM.FilterByCategory(e.NewTextValue, true);
        }
    }
}