using GymTracker.Models;
using Microsoft.Maui.Graphics.Text;
using static System.Net.Mime.MediaTypeNames;

namespace GymTracker.Views;

public partial class profilemonthlyreport : ContentPage
{
	public profilemonthlyreport()
	{
		InitializeComponent();
		var vm = new ProfileViewModel();
		BindingContext = vm;
        MR_Label.Text = DateTime.Now.ToString("MMMM") + " " + DateTime.Now.Year.ToString();
        Button MRWorkout = new Button { Text = "Workout", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MRVolume = new Button { Text = "Volume", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MRIntensity = new Button { Text = "Intensity", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MRDuration = new Button { Text = "Duration", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MRSets = new Button { Text = "Sets", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MRReps = new Button { Text = "Reps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        
		MRWorkout.Clicked += (s, e) => vm.DC = DisplayChoices.Workout;
        MRVolume.Clicked += (s, e) => vm.DC = DisplayChoices.Volume;
        MRIntensity.Clicked += (s, e) => vm.DC = DisplayChoices.Intensity;
        MRDuration.Clicked += (s, e) => vm.DC = DisplayChoices.Duration;
        MRSets.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        MRReps.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        MR_ButtonContainer.Children.Add(MRWorkout);
        MR_ButtonContainer.Children.Add(MRVolume);
        MR_ButtonContainer.Children.Add(MRIntensity);
        MR_ButtonContainer.Children.Add(MRDuration);
        MR_ButtonContainer.Children.Add(MRSets);
        MR_ButtonContainer.Children.Add(MRReps);

        vm.DisplayButtons.Add((MRWorkout, DisplayChoices.Workout));
        vm.DisplayButtons.Add((MRVolume, DisplayChoices.Volume));
        vm.DisplayButtons.Add((MRIntensity, DisplayChoices.Intensity));
        vm.DisplayButtons.Add((MRDuration, DisplayChoices.Duration));
        vm.DisplayButtons.Add((MRSets, DisplayChoices.Sets));
        vm.DisplayButtons.Add((MRReps, DisplayChoices.Reps));

        Label MR_Workout = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_Duration = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_Intensity = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_Volume = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_Sets = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_Reps = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_WorkoutTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_DurationTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_IntensityTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_VolumeTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_SetsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MR_RepsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };

        MR_WorkoutContainer.Children.Add(MR_Workout);
        MR_WorkoutContainer.Children.Add(MR_WorkoutTrend);
        MR_DurationContainer.Children.Add(MR_Duration);
        MR_DurationContainer.Children.Add(MR_DurationTrend);
        MR_IntensityContainer.Children.Add(MR_Intensity);
        MR_IntensityContainer.Children.Add(MR_IntensityTrend);
        MR_VolumeContainer.Children.Add(MR_Volume);
        MR_VolumeContainer.Children.Add(MR_VolumeTrend);
        MR_SetsContainer.Children.Add(MR_Sets);
        MR_SetsContainer.Children.Add(MR_SetsTrend);
        MR_RepsContainer.Children.Add(MR_Reps);
        MR_RepsContainer.Children.Add(MR_RepsTrend);

        Dictionary<E_StatisticsLabels, Label> labels = new Dictionary<E_StatisticsLabels, Label>();
        labels.Add(E_StatisticsLabels.MRWorkout, MR_Workout);
        labels.Add(E_StatisticsLabels.MRWorkoutTrend, MR_WorkoutTrend);
        labels.Add(E_StatisticsLabels.MRDuration, MR_Duration);
        labels.Add(E_StatisticsLabels.MRDurationTrend, MR_DurationTrend);
        labels.Add(E_StatisticsLabels.MRIntensity, MR_Intensity);
        labels.Add(E_StatisticsLabels.MRIntensityTrend, MR_IntensityTrend);  
        labels.Add(E_StatisticsLabels.MRVolume, MR_Volume);
        labels.Add(E_StatisticsLabels.MRVolumeTrend, MR_VolumeTrend);
        labels.Add(E_StatisticsLabels.MRSets, MR_Sets);
        labels.Add(E_StatisticsLabels.MRSetsTrend, MR_SetsTrend);
        labels.Add(E_StatisticsLabels.MRReps, MR_Reps);
        labels.Add(E_StatisticsLabels.MRRepsTrend, MR_RepsTrend);
        vm.SetMonthlyReportLabels(labels);
        vm.MRChart = MRChart;

        vm.DC = DisplayChoices.Workout;
    }
}