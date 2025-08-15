using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker.Views;

public partial class SetSelection : ContentPage
{
	public SetSelection()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }
    private async void Button_Pressed(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await Task.WhenAll(
          button.ScaleTo(0.95, 50, Easing.CubicIn),
          button.ColorTo(Colors.Black, Colors.White, 50)
      );
        }
    }

    private async void Button_Released(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            await Task.WhenAll(
                button.ScaleTo(1, 50, Easing.CubicOut),
                button.ColorTo(Colors.White, Colors.Black, 50)
            );
        }
    }
}