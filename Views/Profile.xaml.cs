using GymTracker.Models;
using GymTracker.Services;
using Microsoft.Maui;
using System;
using System.Numerics;

namespace GymTracker;

public partial class Profile : ContentPage
{
    private ProfileViewModel vm;
	public Profile()
	{
		InitializeComponent();
        vm = new ProfileViewModel();
        BindingContext = vm;

        UpdateHeaderLabels();
        UpdateSplits(AppState.MuscleFunctionsList, (e, split) => e.Function == split, FunctionStack);
        UpdateSplits(AppState.MuscleGroupsList, (e, group) => e.MuscleGroup == group, GroupStack);
        UpdateSplits(AppState.MusclesList, (e, muscle) => e.TargetMuscle == muscle, IndividualStack);
        AppState.profileStat = ProfileStats.Weekly;

        vm.WeeklyStatsVolumeButton = new Button { Text = "Volume", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#008cff") };
        vm.WeeklyStatsDurationButton = new Button { Text = "Duration", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        vm.WeeklyStatsSetsButton = new Button { Text = "Sets", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        vm.WeeklyStatsRepsButton = new Button { Text = "Reps", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };

        vm.WeeklyStatsVolumeButton.Clicked += (s, e) => vm.DC = DisplayChoices.Volume;
        vm.WeeklyStatsDurationButton.Clicked += (s, e) => vm.DC = DisplayChoices.Duration;
        vm.WeeklyStatsSetsButton.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        vm.WeeklyStatsRepsButton.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsVolumeButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsDurationButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsSetsButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsRepsButton);
        vm.WeeklyChart = WeeklyChart;
        vm.UpdateWeeklyStats();
        vm.DC = DisplayChoices.Volume;
    }

    private void UpdateHeaderLabels()
    {
        double time = vm.Workouts.Sum(w => w.Duration.TotalSeconds);
        int workouts = vm.Workouts.Count();
        int sets = vm.Workouts.SelectMany(w => w.Exercises).SelectMany(w => w.CheckedSets).Count();
        int reps = vm.Workouts.SelectMany(w => w.Exercises).SelectMany(w => w.CheckedSets).Sum(s => s.Reps);
        double volume = vm.Workouts.SelectMany(w => w.Exercises).SelectMany(w => w.CheckedSets).Sum(s => s.Weight * s.Reps);
        WorkoutsLabel.Text = workouts.ToString();
        SetsLabel.Text = sets.ToString();
        RepsLabel.Text = reps.ToString();
        VolumeLabel.Text = volume.ToString("N0") + AppState.Profile.WeightUnit;
        if (time >= 0)
        {
            var ts = TimeSpan.FromSeconds(time);
            if (ts.TotalHours >= 24)
            {
                TimeLabel.Text = $"{ts.Days}d {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            }
            TimeLabel.Text = $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
        }
        else
            TimeLabel.Text = "00:00:00";
    }

    private void UpdateSplits<T>(IEnumerable<T> sourceList, Func<Exercise, T, bool> filter, VerticalStackLayout targetStack)
    {
        int totalSets = vm.Workouts
            .SelectMany(e => e.Exercises)
            .SelectMany(s => s.CheckedSets)
            .Count();

        targetStack.Children.Clear();

        foreach (var item in sourceList)
        {
            Grid grid = new Grid
            {
                Padding = 0,HorizontalOptions = LayoutOptions.FillAndExpand, ColumnSpacing = 10
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });

            var vstack = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center, Spacing = 2
            };
            Grid.SetColumn(vstack, 0);

            var pbar = new ProgressBar
            {
                ProgressColor = Color.FromArgb("#008cff"), BackgroundColor = Colors.Black, HeightRequest = 12, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(pbar, 1);

            var gridLabel = new Label
            {
                Text = "", HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center
            };
            Grid.SetColumn(gridLabel, 2);

            var itemLabel = new Label
            {
                Text = "", HorizontalOptions = LayoutOptions.Start, FontSize = 15
            };
            var bindItemLabel = new Label
            {
                Text = "", FontSize = 16, FontAttributes = FontAttributes.Bold, HorizontalTextAlignment = TextAlignment.Center
            };

            vstack.Children.Add(itemLabel);
            vstack.Children.Add(bindItemLabel);

            int sets = vm.Workouts.SelectMany(e => e.Exercises).Where(e => filter(e, item)).SelectMany(s => s.CheckedSets).Count();
            double percentage = totalSets != 0 ? (double)sets / totalSets : 0;

            if(item != null)
                itemLabel.Text = AppState.MGMToString(item);
            else
            {
                Console.WriteLine("ERROR ------------- MUSCLE ENUM IS NULL IF THAT'S EVEN POSSIBLE!!! WHY IS C# ANNOYING ME");
            }
            bindItemLabel.Text = sets.ToString();
            pbar.Progress = percentage;
            gridLabel.Text = (percentage * 100).ToString("N0") + "%";

            grid.Children.Add(vstack);
            grid.Children.Add(pbar);
            grid.Children.Add(gridLabel);

            targetStack.Children.Add(grid);
        }
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateHeaderLabels();
        UpdateSplits(AppState.MuscleFunctionsList, (e, split) => e.Function == split , FunctionStack);
        UpdateSplits(AppState.MuscleGroupsList, (e, group) => e.MuscleGroup == group, GroupStack);
        UpdateSplits(AppState.MusclesList, (e, muscle) => e.TargetMuscle == muscle, IndividualStack);
        vm.UpdateWeeklyStats();
    }
}