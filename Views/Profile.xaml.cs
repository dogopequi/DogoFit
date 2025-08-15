using GymTracker.Models;

namespace GymTracker;

public partial class Profile : ContentPage
{
	public Profile()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
    }
}