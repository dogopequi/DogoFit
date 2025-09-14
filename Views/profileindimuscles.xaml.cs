using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker.Views;

public partial class profileindimuscles : ContentPage
{
	public profileindimuscles()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        vm.AddTimeButton(TimeChoices.Month, "Current Month", IM_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.ThreeMonths, "Last 3 Months", IM_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.Year, "Last Year", IM_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.All, "All Time", IM_StatsButtonContainer);

        foreach(Muscles m in AppState.MusclesList)
        {
            Button button = new Button { Text = AppState.MuscleToString(m), TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
            button.Clicked += (s, e) => {  vm.DoesUpdatePieChart = false; vm.MC = m;};
            IM_ChoiceButtonContainer.Children.Add(button);
            vm.MusclesButtons.Add((button, m));
        }

        vm.AddDisplayButton(DisplayChoices.Intensity, "Intensity", IM_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Volume, "Volume", IM_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Sets, "Sets", IM_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Reps, "Reps", IM_DisplayButtonContainer);

        vm.AddLabel(E_StatisticsLabels.IMIntensity, IM_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.IMIntensityTrend, IM_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.IMVolume, IM_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.IMVolumeTrend, IM_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.IMSets, IM_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.IMSetsTrend, IM_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.IMReps, IM_RepsContainer);
        vm.AddLabel(E_StatisticsLabels.IMRepsTrend, IM_RepsContainer);

        vm.PieChartLabel = PieChartLabel;
        vm.IM_InfoLabel = IM_InfoLabel;
        vm.IM_InfoLabel.TextColor = Colors.White;
        vm.IM_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.IM_InfoLabel.FontSize = 20;
        vm.IM_InfoLabel.Margin = new Thickness(0, 20, 0, 0);

        vm.IMChart = IndividualMuscleChart;

        vm.DC = DisplayChoices.Intensity;
        vm.MC = Muscles.Biceps;
        vm.TC = TimeChoices.Month;
    }
}