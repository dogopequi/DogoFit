using GymTracker.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics.Text;
using Microsoft.Maui.Storage;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GymTracker.Models
{
    public enum WeeklyGraphChoice
    {
        VOLUME, DURATION, SETS, REPS
    }
    public class ProfileViewModel
    {
        public ICommand OpenSettingsCommand { get; }
        public ICommand GitHubCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand UnitSystemCommand { get; }

        public ICommand ExercisesCommand { get; }
        public ICommand FilterCommand { get; }
        public String EnteredExerciseName { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }
        /// <summary>
        /// TODO exercise stats with ExerciseRecord
        /// statistics button: Set count per muscle group, Muslcle group distribuition, main exercises, monthly report
        /// (numbers or workouts, duration, volume, sets, reps) how many of each it has gained this month
        /// how many new records (ExeriseRecord query) (dont know how ill implement that, maybe a date for each new record) week streaks
        /// comparison of last month muscle distribuition versus current month
        /// 
        /// all and all, two new pages to implement
        /// </summary>
        public ICommand StatisticsCommand { get; }
        public Button WeeklyStatsVolumeButton { get; set; }
        public Button WeeklyStatsDurationButton { get; set; }
        public Button WeeklyStatsSetsButton { get; set; }
        public Button WeeklyStatsRepsButton { get; set; }


        public HorizontalStackLayout WeeklyStatsButtonContainer { get; set; }
        public CartesianChart WeeklyChart { get; set; }
        private WeeklyGraphChoice _wgc;
        public WeeklyGraphChoice WGC
        {
            get => _wgc;
            set
            {
                if (_wgc != value)
                {
                    _wgc = value;
                    OnPropertyChanged(nameof(WGC));
                    UpdateButtonColors();
                    UpdateWeeklyChartChoices(_wgc);
                }
            }
        }

        public Services.Profile Profile { get; set; }

        public ObservableCollection<Routine> Workouts { get; }
        private List<WeekStat> WeeklyStats { get; set; }
        private List<ExerciseRecord> ExerciseRecords { get; set; }
        public ProfileViewModel()
        {
            Profile = AppState.Profile;
            OpenSettingsCommand = new Command(OpenSettings);
            GitHubCommand = new Command(OpenGitHub);
            ExportCommand = new Command(ExportData);
            ImportCommand = new Command(ImportData);
            UnitSystemCommand = new Command(ToggleWeightUnit);
            ExercisesCommand = new Command(OnExercises);
            StatisticsCommand = new Command(OnStatistics);
            Categories = AppState.Categories;
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            FilterByCategory("All", false);
            FilterCommand = new Command<Category>(SelectCategory);
            Workouts = AppState.Workouts;

            WGC = WeeklyGraphChoice.VOLUME;
            UpdateWeeklyStats();
        }
        
        public ISeries[] TotalVolumeSeries { get; set; }
        public Axis[] TotalVolumeXAxes { get; set; }
        public Axis[] TotalVolumeYAxes { get; set; }

        public ISeries[] TotalDurationSeries { get; set; }
        public Axis[] TotalDurationXAxes { get; set; }
        public Axis[] TotalDurationYAxes { get; set; }

        public ISeries[] TotalRepsSeries { get; set; }
        public Axis[] TotalRepsXAxes { get; set; }
        public Axis[] TotalRepsYAxes { get; set; }

        public ISeries[] TotalSetsSeries { get; set; }
        public Axis[] TotalSetsXAxes { get; set; }
        public Axis[] TotalSetsYAxes { get; set; }
        public void UpdateWeeklyChartChoices(WeeklyGraphChoice choice)
        {
            WGC = choice;
            OnPropertyChanged(nameof(WGC));

            if (WeeklyChart == null) return;

            switch (choice)
            {
                case WeeklyGraphChoice.VOLUME:
                    WeeklyChart.Series = TotalVolumeSeries;
                    WeeklyChart.XAxes = TotalVolumeXAxes;
                    WeeklyChart.YAxes = TotalVolumeYAxes;
                    break;
                case WeeklyGraphChoice.DURATION:
                    WeeklyChart.Series = TotalDurationSeries;
                    WeeklyChart.XAxes = TotalDurationXAxes;
                    WeeklyChart.YAxes = TotalDurationYAxes;
                    break;
                case WeeklyGraphChoice.SETS:
                    WeeklyChart.Series = TotalSetsSeries;
                    WeeklyChart.XAxes = TotalSetsXAxes;
                    WeeklyChart.YAxes = TotalSetsYAxes;
                    break;
                case WeeklyGraphChoice.REPS:
                    WeeklyChart.Series = TotalRepsSeries;
                    WeeklyChart.XAxes = TotalRepsXAxes;
                    WeeklyChart.YAxes = TotalRepsYAxes;
                    break;
            }
        }
        private void UpdateButtonColors()
        {
            WeeklyStatsVolumeButton.BackgroundColor = WGC == WeeklyGraphChoice.VOLUME ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsDurationButton.BackgroundColor = WGC == WeeklyGraphChoice.DURATION ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsSetsButton.BackgroundColor = WGC == WeeklyGraphChoice.SETS ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsRepsButton.BackgroundColor = WGC == WeeklyGraphChoice.REPS ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");

        }

        private void UpdateWeeklyCharts()
        {

            TotalVolumeSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Volume",
                    Values = WeeklyStats.Select(s => s.Volume).ToArray()
                }
            };

            TotalVolumeXAxes = new Axis[]
            {
                new Axis
                {
                    Labels = WeeklyStats.Select(s => s.StartDate.ToString("MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            TotalVolumeYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} kg",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };


            TotalDurationSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Duration",
                    Values = WeeklyStats.Select(s => s.Duration).ToArray()
                }
            };

            TotalDurationXAxes = TotalVolumeXAxes;
            TotalDurationYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} min",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };


            TotalSetsSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Sets",
                    Values = WeeklyStats.Select(s => s.Sets).ToArray()
                }
            };

            TotalSetsXAxes = TotalVolumeXAxes;
            TotalSetsYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} sets",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };


            TotalRepsSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Reps",
                    Values = WeeklyStats.Select(s => s.Reps).ToArray()
                }
            };

            TotalRepsXAxes = TotalVolumeXAxes;
            TotalRepsYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} reps",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };
        }

        public void UpdateWeeklyStats()
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var dfi = DateTimeFormatInfo.CurrentInfo;

            WeeklyStats = Workouts
                 .GroupBy(w => calendar.GetWeekOfYear(
                     w.StartTime,
                     CalendarWeekRule.FirstFourDayWeek,
                     DayOfWeek.Monday))
                 .Select(g => new WeekStat
                 {
                     WeekNumber = g.Key,
                     Year = g.Min(w => w.StartTime.Year),
                     StartDate = FirstDateOfWeekIso8601(g.Min(w => w.StartTime.Year), g.Key),
                     Volume = g.Sum(w => w.Volume),
                     Duration = g.Sum(w => w.Duration.TotalMinutes),
                     Reps = g.Sum(w => w.Exercises.Sum(e => e.CheckedSets.Sum(s => s.Reps))),
                     Sets = g.Sum(w => w.Exercises.Sum(e => e.Sets.Count(s => s.IsChecked)))
                 })
                 .OrderBy(s => s.Year)
                 .ThenBy(s => s.WeekNumber)
                 .ToList();

            UpdateWeeklyCharts();
        }

        private async void OnExercises()
        {
            await Shell.Current.GoToAsync("profileselectexercise");
        }

        private void OnStatistics()
        {

        }
        private void ToggleWeightUnit()
        {
            Profile.UseMetric = !Profile.UseMetric;
            AppState.RecalculateWeights();
            DbHelper.SaveProfile(App.Db, Profile);
        }

        private async void OpenSettings()
        {
            await Shell.Current.GoToAsync("settings");
        }
        private async void OpenGitHub()
        {
            await Launcher.Default.OpenAsync("https://github.com/dogopequi/DogoFit");
        }

        public class ExportDataType
        {
            public ObservableCollection<Routine> Routines { get; set; }
            public ObservableCollection<Routine> Workouts { get; set; }
            public Services.Profile Profile { get; set; }
        }

        private async void ExportData()
        {
            var exportData = new
            {
                Routines = AppState.Routines,
                Workouts = AppState.Workouts,
                Profile = AppState.Profile
            };

            try
            {
                string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });

                string filePath = Path.Combine(FileSystem.AppDataDirectory, "exported_data.json");
                await File.WriteAllTextAsync(filePath, json);

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Export JSON Data",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Export failed: {ex.Message}", "OK");
            }
        }
        public static DateTime FirstDateOfWeekIso8601(int year, int weekOfYear)
        {
            DateTime jan4 = new DateTime(year, 1, 4);
            int dayOfWeek = (int)jan4.DayOfWeek;
            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
            DateTime firstMonday = jan4.AddDays(1 - dayOfWeek);
            return firstMonday.AddDays((weekOfYear - 1) * 7);
        }
        private async void ImportData()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select exported JSON file",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.json" } },
                { DevicePlatform.Android, new[] { "application/json" } },
                { DevicePlatform.WinUI, new[] { ".json" } },
                { DevicePlatform.MacCatalyst, new[] { "json" } }
            })
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();

                    var importedData = await JsonSerializer.DeserializeAsync<ExportDataType>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            IgnoreReadOnlyProperties = true,
                            IncludeFields = true,
                        });

                    if (importedData != null)
                    {
                        foreach (var r in importedData.Routines ?? Enumerable.Empty<Routine>())
                        {
                            AppState.Routines.Add(r);
                            var template = DbHelper.ToDbRoutineTemplate(r);
                            App.Db.RoutineTemplates.Add(template);
                            App.Db.SaveChanges();
                        }

                        foreach (var w in importedData.Workouts ?? Enumerable.Empty<Routine>())
                        {
                            AppState.Workouts.Insert(0, w);
                            w.CalculateSetDistribution();
                            AppState.RoutinesCount += 1;
                            var workout = DbHelper.ToDbWorkout(w);
                            App.Db.Workouts.Add(workout);
                            App.Db.SaveChanges();

                            w.ID = workout.Id;
                        }

                        if (importedData.Profile != null)
                        {
                            AppState.Profile.Lats += importedData.Profile.Lats;
                            AppState.Profile.Triceps += importedData.Profile.Triceps;
                            AppState.Profile.Biceps += importedData.Profile.Biceps;
                            AppState.Profile.Quadriceps += importedData.Profile.Quadriceps;
                            AppState.Profile.Hamstrings += importedData.Profile.Hamstrings;
                            AppState.Profile.Glutes += importedData.Profile.Glutes;
                            AppState.Profile.Calves += importedData.Profile.Calves;
                            AppState.Profile.Abdominals += importedData.Profile.Abdominals;
                            AppState.Profile.Obliques += importedData.Profile.Obliques;
                            AppState.Profile.Traps += importedData.Profile.Traps;
                            AppState.Profile.LateralDelts += importedData.Profile.LateralDelts;
                            AppState.Profile.FrontDelts += importedData.Profile.FrontDelts;
                            AppState.Profile.RearDelts += importedData.Profile.RearDelts;
                            AppState.Profile.Forearms += importedData.Profile.Forearms;
                            AppState.Profile.Push += importedData.Profile.Push;
                            AppState.Profile.Chest += importedData.Profile.Chest;
                            AppState.Profile.Back += importedData.Profile.Back;
                            AppState.Profile.Legs += importedData.Profile.Legs;
                            AppState.Profile.Arms += importedData.Profile.Arms;
                            AppState.Profile.Core += importedData.Profile.Core;
                            AppState.Profile.Shoulders += importedData.Profile.Shoulders;
                            AppState.Profile.Pull += importedData.Profile.Pull;
                            AppState.Profile.Reps += importedData.Profile.Reps;
                            AppState.Profile.Volume += importedData.Profile.Volume;
                            AppState.Profile.Sets += importedData.Profile.Sets;
                            AppState.Profile.Duration += importedData.Profile.Duration;
                            AppState.Profile.Workouts += 1;
                            DbHelper.SaveProfile(App.Db, Profile);
                        }

                    }

                    await Shell.Current.DisplayAlert("Success", "Data imported successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
            }
        }

        private void SelectCategory(Category category)
        {
            foreach (var c in Categories)
                c.IsSelected = false;

            category.IsSelected = true;

            FilterByCategory(category.Name, false);
        }
        public void FilterByCategory(string category, bool UseName)
        {
            FilteredExercises.Clear();
            IEnumerable<Exercise> filtered;
            if (!UseName)
            {

                if (new[] { "Pull", "Push", "Legs", "Core" }.Contains(category, StringComparer.OrdinalIgnoreCase))
                {
                    filtered = category == "All"
                        ? AllExercises
                        : AllExercises.Where(e => string.Equals(e.Function, category, StringComparison.OrdinalIgnoreCase));
                }
                else if (new[] { "Arms", "Shoulders", "Chest", "Back" }.Contains(category, StringComparer.OrdinalIgnoreCase))
                {
                    filtered = category == "All"
                        ? AllExercises
                        : AllExercises.Where(e => string.Equals(e.MuscleGroup, category, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    filtered = category == "All"
                        ? AllExercises
                        : AllExercises.Where(e => string.Equals(e.TargetMuscle, category, StringComparison.OrdinalIgnoreCase));
                }
            }
            else
            {
                filtered = category == "All"
                    ? AllExercises
                    : AllExercises.Where(e => e.Name.Contains(category, StringComparison.OrdinalIgnoreCase));
            }


            foreach (var ex in filtered)
            {
                var clone = new Exercise(ex);
                clone.IsSelected = AppState.SelectedExerciseIds.Contains(clone.Name);
                FilteredExercises.Add(clone);
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
