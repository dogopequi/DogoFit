using GymTracker.Models;
using CommunityToolkit.Maui.Extensions;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class Settings : ContentPage
{

    public static bool IsExport = false;
	public Settings()
	{
		InitializeComponent();
		BindingContext = new ProfileViewModel();
    }

    private async void Import(object sender, EventArgs e)
    {
        if (sender is Button button && BindingContext is ProfileViewModel vm)
        {
            Settings.IsExport = false;
            ImportExportPopup_Clicked(sender, e);
        }
    }

        private async void Export(object sender, EventArgs e)
    {
        if (sender is Button button && BindingContext is ProfileViewModel vm)
        {
            Settings.IsExport = true;
            ImportExportPopup_Clicked(sender, e);
        }
    }

	private async void ExercisePopup_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && BindingContext is ProfileViewModel vm)
        {
            var popup = new Popups.ExercisePopup();
            await this.ShowPopupAsync(popup);

            string? action = await popup.WaitForResultAsync();
            if (action is null) return;

            switch (action)
            {
                case "Add":
                    AppState.profileExercise = ProfileExercise.Add;
                    vm.OnAddExerciseToList();
                    break;
                case "EditDelete":
                    AppState.profileExercise = ProfileExercise.Edit;
                    vm.OnEditDeleteExercises();
                    break;
            }
        }
    }

    private async void ImportExportPopup_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && BindingContext is ProfileViewModel vm)
        {
            var popup = new Popups.ImportExportPopup();
            await this.ShowPopupAsync(popup);

            string? action = await popup.WaitForResultAsync();
            if (action is null) return;

            switch (action)
            {
                case "Workouts":
                    vm.OnImportExportWorkouts();
                    break;
                case "Routines":
                    vm.OnImportExportRoutines();
                    break;
                case "Exercises":
                    vm.OnImportExportExercises();
                    break;
                case "All":
                    vm.OnImportExportAll();
                    break;
            }
        }
    }
}