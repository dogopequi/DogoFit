using CommunityToolkit.Maui.Extensions;
using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class EditRoutine : ContentPage
{
	public EditRoutine()
	{
		InitializeComponent();
		BindingContext = new EditRoutineModel();
	}

    private async void SetOptions_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button
            && button.BindingContext is Set set
            && button.CommandParameter is Exercise exercise
            && BindingContext is EditRoutineModel vm)
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