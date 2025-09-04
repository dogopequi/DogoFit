using GymTracker.Models;
using LiveChartsCore.Measure;
using Microsoft.Maui.Graphics.Text;

namespace GymTracker.Views;

public partial class profileindimuscles : ContentPage
{
	public profileindimuscles()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        Button IMLastMonth = new Button { Text = "Current Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMLastThreeMonths = new Button { Text = "Last 3 Month", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button IMLastYear = new Button { Text = "Last Year", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button IMAllTime = new Button { Text = "All Time", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        IMLastMonth.Clicked += (s, e) => vm.TC = TimeChoices.Month;
        IMLastThreeMonths.Clicked += (s, e) => vm.TC = TimeChoices.ThreeMonths;
        IMLastYear.Clicked += (s, e) => vm.TC = TimeChoices.Year;
        IMAllTime.Clicked += (s, e) => vm.TC = TimeChoices.All;

        IM_StatsButtonContainer.Children.Add(IMLastMonth);
        IM_StatsButtonContainer.Children.Add(IMLastThreeMonths);
        IM_StatsButtonContainer.Children.Add(IMLastYear);
        IM_StatsButtonContainer.Children.Add(IMAllTime);

        vm.TimeButtons.Add((IMLastMonth, TimeChoices.Month));
        vm.TimeButtons.Add((IMLastThreeMonths, TimeChoices.ThreeMonths));
        vm.TimeButtons.Add((IMLastYear, TimeChoices.Year));
        vm.TimeButtons.Add((IMAllTime, TimeChoices.All));

        Button IMBiceps = new Button { Text = "Biceps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMTriceps = new Button { Text = "Triceps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMForearms = new Button { Text = "Forearms", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMLats = new Button { Text = "Lats", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMTraps = new Button { Text = "Traps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button BChest = new Button { Text = "Chest", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMSideDelts = new Button { Text = "Lateral Delts", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMFrontDelts = new Button { Text = "Front Delts", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMRearDelts = new Button { Text = "Rear Delts", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMQuads = new Button { Text = "Quadriceps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMHams = new Button { Text = "Hamstrings", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMGlutes = new Button { Text = "Glutes", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button BCalves = new Button { Text = "Calves", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMAbs = new Button { Text = "Abdominals", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMObliques = new Button { Text = "Obliques", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };

        IMBiceps.Clicked += (s, e) => vm.MC = MuscleChoices.Biceps;
        IMTriceps.Clicked += (s, e) => vm.MC = MuscleChoices.Triceps;
        IMForearms.Clicked += (s, e) => vm.MC = MuscleChoices.Forearms;
        IMLats.Clicked += (s, e) => vm.MC = MuscleChoices.Lats;
        IMTraps.Clicked += (s, e) => vm.MC = MuscleChoices.Traps;
        BChest.Clicked += (s, e) => vm.MC = MuscleChoices.Chest;
        IMSideDelts.Clicked += (s, e) => vm.MC = MuscleChoices.SideDelts;
        IMFrontDelts.Clicked += (s, e) => vm.MC = MuscleChoices.FrontDelts;
        IMRearDelts.Clicked += (s, e) => vm.MC = MuscleChoices.RearDelts;
        IMQuads.Clicked += (s, e) => vm.MC = MuscleChoices.Quads;
        IMHams.Clicked += (s, e) => vm.MC = MuscleChoices.Hams;
        IMGlutes.Clicked += (s, e) => vm.MC = MuscleChoices.Glutes;
        BCalves.Clicked += (s, e) => vm.MC = MuscleChoices.Calves;
        IMAbs.Clicked += (s, e) => vm.MC = MuscleChoices.Abs;
        IMObliques.Clicked += (s, e) => vm.MC = MuscleChoices.Obliques;

        IM_ChoiceButtonContainer.Children.Add(IMBiceps);
        IM_ChoiceButtonContainer.Children.Add(IMTriceps);
        IM_ChoiceButtonContainer.Children.Add(IMForearms);
        IM_ChoiceButtonContainer.Children.Add(IMLats);
        IM_ChoiceButtonContainer.Children.Add(IMTraps);
        IM_ChoiceButtonContainer.Children.Add(BChest);
        IM_ChoiceButtonContainer.Children.Add(IMSideDelts);
        IM_ChoiceButtonContainer.Children.Add(IMFrontDelts);
        IM_ChoiceButtonContainer.Children.Add(IMRearDelts);
        IM_ChoiceButtonContainer.Children.Add(IMQuads);
        IM_ChoiceButtonContainer.Children.Add(IMHams);
        IM_ChoiceButtonContainer.Children.Add(IMGlutes);
        IM_ChoiceButtonContainer.Children.Add(BCalves);
        IM_ChoiceButtonContainer.Children.Add(IMAbs);
        IM_ChoiceButtonContainer.Children.Add(IMObliques);

        vm.MusclesButtons.Add((IMBiceps, MuscleChoices.Biceps));
        vm.MusclesButtons.Add((IMTriceps, MuscleChoices.Triceps));
        vm.MusclesButtons.Add((IMForearms, MuscleChoices.Forearms));
        vm.MusclesButtons.Add((IMLats, MuscleChoices.Lats));
        vm.MusclesButtons.Add((IMTraps, MuscleChoices.Traps));
        vm.MusclesButtons.Add((BChest, MuscleChoices.Chest));
        vm.MusclesButtons.Add((IMSideDelts, MuscleChoices.SideDelts));
        vm.MusclesButtons.Add((IMFrontDelts, MuscleChoices.FrontDelts));
        vm.MusclesButtons.Add((IMRearDelts, MuscleChoices.RearDelts));
        vm.MusclesButtons.Add((IMQuads, MuscleChoices.Quads));
        vm.MusclesButtons.Add((IMHams, MuscleChoices.Hams));
        vm.MusclesButtons.Add((IMGlutes, MuscleChoices.Glutes));
        vm.MusclesButtons.Add((BCalves, MuscleChoices.Calves));
        vm.MusclesButtons.Add((IMAbs, MuscleChoices.Abs));
        vm.MusclesButtons.Add((IMObliques, MuscleChoices.Obliques));

        Button IMIntensity = new Button { Text = "Intensity", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
        Button IMVolume = new Button { Text = "Volume", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button IMSets = new Button { Text = "Sets", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button IMReps = new Button { Text = "Reps", TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#2b2b2b") };

        IMIntensity.Clicked += (s, e) => vm.DC = DisplayChoices.Intensity;
        IMVolume.Clicked += (s, e) => vm.DC = DisplayChoices.Volume;
        IMSets.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        IMReps.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        IM_DisplayButtonContainer.Children.Add(IMIntensity);
        IM_DisplayButtonContainer.Children.Add(IMVolume);
        IM_DisplayButtonContainer.Children.Add(IMSets);
        IM_DisplayButtonContainer.Children.Add(IMReps);

        vm.DisplayButtons.Add((IMIntensity, DisplayChoices.Intensity));
        vm.DisplayButtons.Add((IMVolume, DisplayChoices.Volume));
        vm.DisplayButtons.Add((IMSets, DisplayChoices.Sets));
        vm.DisplayButtons.Add((IMReps, DisplayChoices.Reps));

        Label IM_Intensity = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_Volume = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_Sets = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_Reps = new Label { Text = "-", TextColor = Colors.White, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_IntensityTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_VolumeTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_SetsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
        Label IM_RepsTrend = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };

        IM_IntensityContainer.Children.Add(IM_Intensity);
        IM_IntensityContainer.Children.Add(IM_IntensityTrend);
        IM_VolumeContainer.Children.Add(IM_Volume);
        IM_VolumeContainer.Children.Add(IM_VolumeTrend);
        IM_SetsContainer.Children.Add(IM_Sets);
        IM_SetsContainer.Children.Add(IM_SetsTrend);
        IM_RepsContainer.Children.Add(IM_Reps);
        IM_RepsContainer.Children.Add(IM_RepsTrend);

        vm.StatisticsLabels.Add(E_StatisticsLabels.IMIntensity, IM_Intensity);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMIntensityTrend, IM_IntensityTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMVolume, IM_Volume);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMVolumeTrend, IM_VolumeTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMSets, IM_Sets);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMSetsTrend, IM_SetsTrend);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMReps, IM_Reps);
        vm.StatisticsLabels.Add(E_StatisticsLabels.IMRepsTrend, IM_RepsTrend);

        vm.IM_InfoLabel = IM_InfoLabel;
        vm.IM_InfoLabel.TextColor = Colors.White;
        vm.IM_InfoLabel.HorizontalTextAlignment = TextAlignment.Center;
        vm.IM_InfoLabel.FontSize = 20;
        vm.IM_InfoLabel.Margin = new Thickness(0, 20, 0, 0);

        vm.IMChart = IndividualMuscleChart;

        vm.DC = DisplayChoices.Intensity;
        vm.MC = MuscleChoices.Biceps;
        vm.TC = TimeChoices.Month;
    }
}