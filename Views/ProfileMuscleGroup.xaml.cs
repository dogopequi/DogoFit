using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class ProfileMuscleGroup : ContentPage
{
    private ProfileViewModel vm;
	public ProfileMuscleGroup()
	{
		InitializeComponent();
		vm = new ProfileViewModel();
        BindingContext = vm;

        vm.AddTimeButton(TimeChoices.Month, "Current Month", MG_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.ThreeMonths, "Last 3 Months", MG_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.Year, "Last Year", MG_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.All, "All Time", MG_StatsButtonContainer);

        foreach(MuscleGroups mg in AppState.MuscleGroupsList)
        {
            Button button = new Button { Text = AppState.MuscleGroupToString(mg), TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
            button.Clicked += (s, e) => vm.MG = mg;
            MG_ChoiceButtonContainer.Children.Add(button);
            vm.MuscleGroupsButtons.Add((button, mg));
        }

        vm.AddDisplayButton(DisplayChoices.Intensity, "Intensity", MG_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Volume, "Volume", MG_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Sets, "Sets", MG_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Reps, "Reps", MG_DisplayButtonContainer);

        vm.AddLabel(E_StatisticsLabels.MGIntensity, MG_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.MGIntensityTrend, MG_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.MGVolume, MG_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.MGVolumeTrend, MG_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.MGSets, MG_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.MGSetsTrend, MG_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.MGReps, MG_RepsContainer);
        vm.AddLabel(E_StatisticsLabels.MGRepsTrend, MG_RepsContainer);

        vm.PieChartLabel = PieChartLabel;
        vm.MG_InfoLabel = MG_InfoLabel;
        vm.MG_InfoLabel.TextColor = Colors.White;
        vm.MG_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.MG_InfoLabel.FontSize = 20;
        vm.MG_InfoLabel.Margin = new Thickness(0, 20, 0, 0);
        vm.MGChart = MuscleGroupChart;

        vm.DC = DisplayChoices.Intensity;
        vm.MG = MuscleGroups.Arms;
        vm.TC = TimeChoices.Month;
    }

}