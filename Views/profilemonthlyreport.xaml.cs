using GymTracker.Models;

namespace GymTracker.Views;

public partial class profilemonthlyreport : ContentPage
{
	public profilemonthlyreport()
	{
		InitializeComponent();
		var vm = new ProfileViewModel();
		BindingContext = vm;
        MR_Label.Text = DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString();

        vm.AddDisplayButton(DisplayChoices.Workout, "Workout", MR_ButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Volume, "Volume", MR_ButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Intensity, "Intensity", MR_ButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Duration, "Duration", MR_ButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Sets, "Sets", MR_ButtonContainer);
        vm.AddDisplayButton(DisplayChoices.Reps, "Reps", MR_ButtonContainer);

        Dictionary<E_StatisticsLabels, Label> labels = new Dictionary<E_StatisticsLabels, Label>();
        AddLabel(labels, E_StatisticsLabels.MRWorkout, MR_WorkoutContainer);
        AddLabel(labels, E_StatisticsLabels.MRWorkoutTrend, MR_WorkoutContainer);
        AddLabel(labels, E_StatisticsLabels.MRDuration, MR_DurationContainer);
        AddLabel(labels, E_StatisticsLabels.MRDurationTrend, MR_DurationContainer);
        AddLabel(labels, E_StatisticsLabels.MRIntensity, MR_IntensityContainer);
        AddLabel(labels, E_StatisticsLabels.MRIntensityTrend, MR_IntensityContainer);
        AddLabel(labels, E_StatisticsLabels.MRVolume, MR_VolumeContainer);
        AddLabel(labels, E_StatisticsLabels.MRVolumeTrend, MR_VolumeContainer);
        AddLabel(labels, E_StatisticsLabels.MRSets, MR_SetsContainer);
        AddLabel(labels, E_StatisticsLabels.MRSetsTrend, MR_SetsContainer);
        AddLabel(labels, E_StatisticsLabels.MRReps, MR_RepsContainer);
        AddLabel(labels, E_StatisticsLabels.MRRepsTrend, MR_RepsContainer);

        vm.SetMonthlyReportLabels(labels);
        vm.MRChart = MRChart;

        vm.DC = DisplayChoices.Workout;
    }

    private void AddLabel(Dictionary<E_StatisticsLabels, Label> labels, E_StatisticsLabels e, VerticalStackLayout container)
    {
        Label label = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        container.Children.Add(label);
        labels.Add(e, label);
    }
}