using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using GymTracker.Models;
using GymTracker.Services;
using Microsoft.Maui.Controls.Shapes;

namespace GymTracker;

public partial class Workout : ContentPage
{
	public Workout()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as WorkoutViewModel;
        if (vm != null)
        {
            vm.WorkoutInProgress = AppState.WorkoutInProgress;
            if (AppState.IsWorkoutInProgress == false)
                vm.OnDiscard();
        }
    }

    private async void OptionsButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is Routine routine)
        {
            if (BindingContext is WorkoutViewModel vm)
            {
                var popup = new Popups.OptionsPopup();

                await this.ShowPopupAsync(popup);

                string? action = await popup.WaitForResultAsync();

                if (action == null)
                    return;

                switch (action)
                {
                    case "Edit":
                        vm.OnEditRoutine(routine);
                        break;

                    case "Delete":
                        vm.OnDeleteRoutine(routine);
                        break;
                }
            }
        }
    }
}