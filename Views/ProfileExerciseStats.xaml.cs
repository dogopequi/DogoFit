using GymTracker.Models;
using Microsoft.Maui.Graphics.Text;

namespace GymTracker.Views;

public partial class ProfileExerciseStats : ContentPage
{

    public ProfileExerciseStats()
    {
        InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        vm.AddDisplayButton(DisplayChoices.Heavy, "Heaviest Weight", ExerciseStatsButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Session, "Session Volume", ExerciseStatsButtonContainer);
        vm.AddDisplayButton(DisplayChoices.BestSet, "Best Set Volume", ExerciseStatsButtonContainer);
        vm.AddDisplayButton(DisplayChoices.OneRepMax, "One Rep Max", ExerciseStatsButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Sets, "Sets", ExerciseStatsButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Reps, "Reps", ExerciseStatsButtonContainer);

        vm.AddLabel(E_StatisticsLabels.ExerciseStats_HW, Grid_HW);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_ORM, Grid_ORM);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_SetV, Grid_SetV);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_SessionV, Grid_SessionV);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_S, Grid_S);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_R, Grid_R);
        vm.AddLabel(E_StatisticsLabels.ExerciseStats_V, Grid_V);

        vm.ExerciseChart = ExerciseChart;
        vm.DC = DisplayChoices.Heavy;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
        {
            vm.UpdateExerciseStats();
        }
    }
}