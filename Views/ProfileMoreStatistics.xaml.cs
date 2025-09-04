using GymTracker.Models;

namespace GymTracker.Views;

public partial class ProfileMoreStatistics : ContentPage
{
	public ProfileMoreStatistics()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
	}
}