using GymTracker.Models;
namespace GymTracker.Views;

public partial class ProfileMuscleFunction : ContentPage
{
	public ProfileMuscleFunction()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        Button MFLastMonth = new Button { Text = "Current Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MFLastThreeMonths = new Button { Text = "Last 3 Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFLastYear = new Button { Text = "Last Year", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFAllTime = new Button { Text = "All Time", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MFLastMonth.Clicked += (s, e) => vm.TC = TimeChoices.Month;
        MFLastThreeMonths.Clicked += (s, e) => vm.TC = TimeChoices.ThreeMonths;
        MFLastYear.Clicked += (s, e) => vm.TC = TimeChoices.Year;
        MFAllTime.Clicked += (s, e) => vm.TC = TimeChoices.All;

        MF_StatsButtonContainer.Children.Add(MFLastMonth);
        MF_StatsButtonContainer.Children.Add(MFLastThreeMonths);
        MF_StatsButtonContainer.Children.Add(MFLastYear);
        MF_StatsButtonContainer.Children.Add(MFAllTime);

        vm.TimeButtons.Add((MFLastMonth, TimeChoices.Month));
        vm.TimeButtons.Add((MFLastThreeMonths, TimeChoices.ThreeMonths));
        vm.TimeButtons.Add((MFLastYear, TimeChoices.Year));
        vm.TimeButtons.Add((MFAllTime, TimeChoices.All));

        Button MFPush = new Button { Text = "Push", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MFPull = new Button { Text = "Pull", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFLegs = new Button { Text = "Legs", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFCore = new Button { Text = "Core", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MFPush.Clicked += (s, e) => vm.MC = MuscleChoices.Push;
        MFPull.Clicked += (s, e) => vm.MC = MuscleChoices.Pull;
        MFLegs.Clicked += (s, e) => vm.MC = MuscleChoices.LegsFunction;
        MFCore.Clicked += (s, e) => vm.MC = MuscleChoices.CoreFunction;

        MF_ChoiceButtonContainer.Children.Add(MFPush);
        MF_ChoiceButtonContainer.Children.Add(MFPull);
        MF_ChoiceButtonContainer.Children.Add(MFLegs);
        MF_ChoiceButtonContainer.Children.Add(MFCore);

        vm.MusclesButtons.Add((MFPush, MuscleChoices.Push));
        vm.MusclesButtons.Add((MFPull, MuscleChoices.Pull));
        vm.MusclesButtons.Add((MFLegs, MuscleChoices.LegsFunction));
        vm.MusclesButtons.Add((MFCore, MuscleChoices.CoreFunction));

        Button MFIntensity = new Button { Text = "Intensity", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MFVolume = new Button { Text = "Volume", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFSets = new Button { Text = "Sets", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MFReps = new Button { Text = "Reps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MFIntensity.Clicked += (s, e) => vm.DC = DisplayChoices.Intensity;
        MFVolume.Clicked += (s, e) => vm.DC = DisplayChoices.Volume;
        MFSets.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        MFReps.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        MF_DisplayButtonContainer.Children.Add(MFIntensity);
        MF_DisplayButtonContainer.Children.Add(MFVolume);
        MF_DisplayButtonContainer.Children.Add(MFSets);
        MF_DisplayButtonContainer.Children.Add(MFReps);

        vm.DisplayButtons.Add((MFIntensity, DisplayChoices.Intensity));
        vm.DisplayButtons.Add((MFVolume, DisplayChoices.Volume));
        vm.DisplayButtons.Add((MFSets, DisplayChoices.Sets));
        vm.DisplayButtons.Add((MFReps, DisplayChoices.Reps));

        Label MF_Intensity = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_Volume = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_Sets = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_Reps = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_IntensityTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_VolumeTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_SetsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MF_RepsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };

        MF_IntensityContainer.Children.Add(MF_Intensity);
        MF_IntensityContainer.Children.Add(MF_IntensityTrend);
        MF_VolumeContainer.Children.Add(MF_Volume);
        MF_VolumeContainer.Children.Add(MF_VolumeTrend);
        MF_SetsContainer.Children.Add(MF_Sets);
        MF_SetsContainer.Children.Add(MF_SetsTrend);
        MF_RepsContainer.Children.Add(MF_Reps);
        MF_RepsContainer.Children.Add(MF_RepsTrend);

        vm.StatisticsLabels.Add(E_StatisticsLabels.MFIntensity, MF_Intensity);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFIntensityTrend, MF_IntensityTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFVolume, MF_Volume);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFVolumeTrend, MF_VolumeTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFSets, MF_Sets);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFSetsTrend, MF_SetsTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFReps, MF_Reps);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MFRepsTrend, MF_RepsTrend);

        vm.MF_InfoLabel = MF_InfoLabel;
        vm.MF_InfoLabel.TextColor = Colors.White;
        vm.MF_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.MF_InfoLabel.FontSize = 20;
        vm.MF_InfoLabel.Margin = new Thickness(0, 20, 0, 0);
        vm.MFChart = MuscleFunctionChart;

        vm.DC = DisplayChoices.Volume;
        vm.MC = MuscleChoices.Push;
        vm.TC = TimeChoices.Month;
    }
}