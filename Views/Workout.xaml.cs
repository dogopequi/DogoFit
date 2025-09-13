using CommunityToolkit.Maui.Extensions;
using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker;

public partial class Workout : ContentPage
{
	public Workout()
	{
		InitializeComponent();
        BindingContext = new WorkoutViewModel();

        UpdateRoutineCountLabel();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateRoutineCountLabel();
        var vm = BindingContext as WorkoutViewModel;
        if (vm != null)
        {
            vm.WorkoutInProgress = AppState.WorkoutInProgress;
        }
    }

    private void UpdateRoutineCountLabel()
    {
        RoutineCount.Text = "My Routines (" + AppState.Routines.Count().ToString() + ")";
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
                        await vm.OnDeleteRoutine(routine);
                        UpdateRoutineCountLabel();
                        break;
                }
            }
        }
    }
}