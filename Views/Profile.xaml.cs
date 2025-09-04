using GymTracker.Models;
using GymTracker.Services;
using System;

namespace GymTracker;

public partial class Profile : ContentPage
{
	public Profile()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
        {
            vm.UpdateWeeklyStats();
        }
    }
}