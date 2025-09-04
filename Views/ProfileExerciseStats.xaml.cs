using GymTracker.Models;
using Microsoft.Maui.Graphics.Text;

namespace GymTracker.Views;

public partial class ProfileExerciseStats : ContentPage
{

    public ProfileExerciseStats()
    {
        InitializeComponent();
        var vm = new ProfileViewModel();
        BindingContext = vm;

        Button ExerciseStatsHW = new Button { Text = "Heaviest Weight", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#008cff") };
        Button ExerciseStatsSV = new Button { Text = "Session Volume", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button ExerciseStatsBS = new Button { Text = "Best Set Volume", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button ExerciseStatsORM = new Button { Text = "One Rep Max", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button ExerciseStatsS = new Button { Text = "Sets", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };
        Button ExerciseStatsR = new Button { Text = "Reps", HorizontalOptions = LayoutOptions.Center, TextColor = Colors.White, BackgroundColor = Color.FromArgb("#2b2b2b") };

        ExerciseStatsHW.Clicked += (s, e) => vm.DC = DisplayChoices.Heavy;
        ExerciseStatsSV.Clicked += (s, e) => vm.DC = DisplayChoices.Session;
        ExerciseStatsBS.Clicked += (s, e) => vm.DC = DisplayChoices.BestSet;
        ExerciseStatsORM.Clicked += (s, e) => vm.DC = DisplayChoices.OneRepMax;
        ExerciseStatsS.Clicked += (s, e) => vm.DC = DisplayChoices.Sets;
        ExerciseStatsR.Clicked += (s, e) => vm.DC = DisplayChoices.Reps;

        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsHW);
        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsSV);
        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsBS);
        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsORM);
        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsS);
        ExerciseStatsButtonContainer.Children.Add(ExerciseStatsR);

        vm.DisplayButtons.Add((ExerciseStatsHW, DisplayChoices.Heavy));
        vm.DisplayButtons.Add((ExerciseStatsSV, DisplayChoices.Session));
        vm.DisplayButtons.Add((ExerciseStatsBS, DisplayChoices.BestSet));
        vm.DisplayButtons.Add((ExerciseStatsORM, DisplayChoices.OneRepMax));
        vm.DisplayButtons.Add((ExerciseStatsS, DisplayChoices.Sets));
        vm.DisplayButtons.Add((ExerciseStatsR, DisplayChoices.Reps));


        Label ExerciseStats_HW = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_ORM = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_SetV = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_SessionV = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_S = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_R = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
        Label ExerciseStats_V = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };

        Grid_HW.Children.Add(ExerciseStats_HW);
        Grid_ORM.Children.Add(ExerciseStats_ORM);
        Grid_SetV.Children.Add(ExerciseStats_SetV);
        Grid_SessionV.Children.Add(ExerciseStats_SessionV);
        Grid_S.Children.Add(ExerciseStats_S);
        Grid_R.Children.Add(ExerciseStats_R);
        Grid_V.Children.Add(ExerciseStats_V);

        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_HW, ExerciseStats_HW);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_ORM, ExerciseStats_ORM);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_SetV, ExerciseStats_SetV);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_SessionV, ExerciseStats_SessionV);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_S, ExerciseStats_S);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_R, ExerciseStats_R);
        vm.StatisticsLabels.Add(E_StatisticsLabels.ExerciseStats_V, ExerciseStats_V);

        vm.ExerciseChart = ExerciseChart;
        vm.DC = DisplayChoices.Heavy;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var vm = BindingContext as ProfileViewModel;
        if (vm != null)
        {
            vm.UpdateExerciseStats();
        }
    }
}