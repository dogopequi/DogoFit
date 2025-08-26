using GymTracker.Models;

namespace GymTracker.Views;

public partial class Settings : ContentPage
{
	public Settings()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
    }
}