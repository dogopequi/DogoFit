using GymTracker.Models;
using GymTracker.Services;

namespace GymTracker;

public partial class Profile : ContentPage
{
	public Profile()
	{
		InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        vm.WeeklyStatsVolumeButton = new Button { Text = "Volume", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#008cff") };
        vm.WeeklyStatsDurationButton = new Button { Text = "Duration", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        vm.WeeklyStatsSetsButton = new Button { Text = "Sets", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        vm.WeeklyStatsRepsButton = new Button { Text = "Reps", Padding = 10, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };

        vm.WeeklyStatsVolumeButton.Clicked += (s, e) => vm.WGC = WeeklyGraphChoice.VOLUME;
        vm.WeeklyStatsDurationButton.Clicked += (s, e) => vm.WGC = WeeklyGraphChoice.DURATION;
        vm.WeeklyStatsSetsButton.Clicked += (s, e) => vm.WGC = WeeklyGraphChoice.SETS;
        vm.WeeklyStatsRepsButton.Clicked += (s, e) => vm.WGC = WeeklyGraphChoice.REPS;

        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsVolumeButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsDurationButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsSetsButton);
        WeeklyStatsButtonContainer.Children.Add(vm.WeeklyStatsRepsButton);

        vm.WeeklyChart = WeeklyChart;
        vm.UpdateWeeklyChartChoices(WeeklyGraphChoice.VOLUME);
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