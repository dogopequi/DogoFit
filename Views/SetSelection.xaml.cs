using CommunityToolkit.Maui.Extensions;
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

    private async void SetOptions_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button
            && button.BindingContext is Set set
            && button.CommandParameter is Exercise exercise
            && BindingContext is WorkoutViewModel vm)
        {
            var popup = new Popups.SetOptionsPopup();
            await this.ShowPopupAsync(popup);

            string? action = await popup.WaitForResultAsync();
            if (action is null) return;

            switch (action)
            {
                case "Normal":
                    vm.OnEditSetNormal(set);
                    break;
                case "Failure":
                    vm.OnEditSetFailure(set);
                    break;
                case "Drop":
                    vm.OnEditSetDrop(set);
                    break;
                case "Warmup":
                    vm.OnEditSetWarmup(set);
                    break;

                case "Remove":
                    vm.OnRemoveSet(exercise, set);
                    break;
            }
        }
    }

}