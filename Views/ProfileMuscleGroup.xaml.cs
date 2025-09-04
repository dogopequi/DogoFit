using GymTracker.Models;
using LiveChartsCore.Measure;
using System;
namespace GymTracker.Views;

public partial class ProfileMuscleGroup : ContentPage
{
	public ProfileMuscleGroup()
	{
		InitializeComponent();
		var vm = new ProfileViewModel();
		BindingContext = vm;

        Button MGLastMonth = new Button { Text = "Current Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MGLastThreeMonths = new Button { Text = "Last 3 Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGLastYear = new Button { Text = "Last Year", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGAllTime = new Button { Text = "All Time", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MGLastMonth.Clicked += (s, e) => vm.TC = TimeChoices.Month;
        MGLastThreeMonths.Clicked += (s, e) => vm.TC = TimeChoices.ThreeMonths;
        MGLastYear.Clicked += (s, e) => vm.TC = TimeChoices.Year;
        MGAllTime.Clicked += (s, e) => vm.TC = TimeChoices.All;

        MG_StatsButtonContainer.Children.Add(MGLastMonth);
        MG_StatsButtonContainer.Children.Add(MGLastThreeMonths);
        MG_StatsButtonContainer.Children.Add(MGLastYear);
        MG_StatsButtonContainer.Children.Add(MGAllTime);

        vm.TimeButtons.Add((MGLastMonth, TimeChoices.Month));
        vm.TimeButtons.Add((MGLastThreeMonths, TimeChoices.ThreeMonths));
        vm.TimeButtons.Add((MGLastYear, TimeChoices.Year));
        vm.TimeButtons.Add((MGAllTime, TimeChoices.All));

        Button MGArms = new Button { Text = "Arms", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MGLegs = new Button { Text = "Legs", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGChest = new Button { Text = "Chest", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGBack = new Button { Text = "Back", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGShoulders = new Button { Text = "Shoulders", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGCore = new Button { Text = "Core", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MGArms.Clicked += (s, e) => vm.MC = MuscleChoices.Arms;
        MGLegs.Clicked += (s, e) => vm.MC = MuscleChoices.LegsGroup;
        MGChest.Clicked += (s, e) => vm.MC = MuscleChoices.ChestGroup;
        MGBack.Clicked += (s, e) => vm.MC = MuscleChoices.Back;
        MGShoulders.Clicked += (s, e) => vm.MC = MuscleChoices.Shoulders;
        MGCore.Clicked += (s, e) => vm.MC = MuscleChoices.CoreGroup;

        MG_ChoiceButtonContainer.Children.Add(MGArms);
        MG_ChoiceButtonContainer.Children.Add(MGLegs);
        MG_ChoiceButtonContainer.Children.Add(MGChest);
        MG_ChoiceButtonContainer.Children.Add(MGBack);
        MG_ChoiceButtonContainer.Children.Add(MGShoulders);
        MG_ChoiceButtonContainer.Children.Add(MGCore);

        vm.MusclesButtons.Add((MGArms, MuscleChoices.Arms));
        vm.MusclesButtons.Add((MGLegs, MuscleChoices.LegsGroup));
        vm.MusclesButtons.Add((MGChest, MuscleChoices.ChestGroup));
        vm.MusclesButtons.Add((MGBack, MuscleChoices.Back));
        vm.MusclesButtons.Add((MGShoulders, MuscleChoices.Shoulders));
        vm.MusclesButtons.Add((MGCore, MuscleChoices.CoreGroup));

        Button MGIntensity = new Button { Text = "Intensity", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button MGVolume = new Button { Text = "Volume", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGSets = new Button { Text = "Sets", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button MGReps = new Button { Text = "Reps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        MGIntensity.Clicked += (s, e) => vm.DC = DisplayChoices.Intensity;
        MGVolume.Clicked += (s, e) => vm.DC = DisplayChoices.Volume;
        MGSets.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        MGReps.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        MG_DisplayButtonContainer.Children.Add(MGIntensity);
        MG_DisplayButtonContainer.Children.Add(MGVolume);
        MG_DisplayButtonContainer.Children.Add(MGSets);
        MG_DisplayButtonContainer.Children.Add(MGReps);

        vm.DisplayButtons.Add((MGIntensity, DisplayChoices.Intensity));
        vm.DisplayButtons.Add((MGVolume, DisplayChoices.Volume));
        vm.DisplayButtons.Add((MGSets, DisplayChoices.Sets));
        vm.DisplayButtons.Add((MGReps, DisplayChoices.Reps));

        Label MG_Intensity = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_Volume = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_Sets = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_Reps = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };

        Label MG_IntensityTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_VolumeTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_SetsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label MG_RepsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };

        MG_IntensityContainer.Children.Add(MG_Intensity);
        MG_IntensityContainer.Children.Add(MG_IntensityTrend);
        MG_VolumeContainer.Children.Add(MG_Volume);
        MG_VolumeContainer.Children.Add(MG_VolumeTrend);
        MG_SetsContainer.Children.Add(MG_Sets);
        MG_SetsContainer.Children.Add(MG_SetsTrend);
        MG_RepsContainer.Children.Add(MG_Reps);
        MG_RepsContainer.Children.Add(MG_RepsTrend);

        vm.StatisticsLabels.Add(E_StatisticsLabels.MGIntensity, MG_Intensity);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGIntensityTrend, MG_IntensityTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGVolume, MG_Volume);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGVolumeTrend, MG_VolumeTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGSets, MG_Sets);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGSetsTrend, MG_SetsTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGReps, MG_Reps);
        vm.StatisticsLabels.Add(E_StatisticsLabels.MGRepsTrend, MG_RepsTrend);

        vm.MG_InfoLabel = MG_InfoLabel;
        vm.MG_InfoLabel.TextColor = Colors.White;
        vm.MG_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.MG_InfoLabel.FontSize = 20;
        vm.MG_InfoLabel.Margin = new Thickness(0, 20, 0, 0);
        vm.MGChart = MuscleGroupChart;

        vm.DC = DisplayChoices.Intensity;
        vm.MC = MuscleChoices.Arms;
        vm.TC = TimeChoices.Month;

    }
}