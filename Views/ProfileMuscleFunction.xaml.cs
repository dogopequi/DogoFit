using GymTracker.Models;
using GymTracker.Services;
namespace GymTracker.Views;

public partial class ProfileMuscleFunction : ContentPage
{
	public ProfileMuscleFunction()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        vm.AddTimeButton(TimeChoices.Month, "Current Month", MF_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.ThreeMonths, "Last 3 Months", MF_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.Year, "Last Year", MF_StatsButtonContainer);
        vm.AddTimeButton(TimeChoices.All, "All Time", MF_StatsButtonContainer);


        foreach(MuscleFunctions mf in AppState.MuscleFunctionsList)
        {
            Button button = new Button { Text = AppState.MuscleFunctionToString(mf), TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
            button.Clicked += (s, e) => vm.MF = mf;
            MF_ChoiceButtonContainer.Children.Add(button);
            vm.MuscleFunctionButtons.Add((button, mf));
        }

        vm.AddDisplayButton(DisplayChoices.Intensity, "Intensity", MF_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Volume, "Volume", MF_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Sets, "Sets", MF_DisplayButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Reps, "Reps", MF_DisplayButtonContainer);

        vm.AddLabel(E_StatisticsLabels.MFIntensity, MF_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.MFIntensityTrend, MF_IntensityContainer);
        vm.AddLabel(E_StatisticsLabels.MFVolume, MF_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.MFVolumeTrend, MF_VolumeContainer);
        vm.AddLabel(E_StatisticsLabels.MFSets, MF_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.MFSetsTrend, MF_SetsContainer);
        vm.AddLabel(E_StatisticsLabels.MFReps, MF_RepsContainer);
        vm.AddLabel(E_StatisticsLabels.MFRepsTrend, MF_RepsContainer);

        vm.PieChartLabel = PieChartLabel;
        vm.MF_InfoLabel = MF_InfoLabel;
        vm.MF_InfoLabel.TextColor = Colors.White;
        vm.MF_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.MF_InfoLabel.FontSize = 20;
        vm.MF_InfoLabel.Margin = new Thickness(0, 20, 0, 0);
        vm.MFChart = MuscleFunctionChart;

        vm.DC = DisplayChoices.Volume;
        vm.MF = MuscleFunctions.Push;
        vm.TC = TimeChoices.Month;
    }
}