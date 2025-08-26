using GymTracker.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GymTracker.Models
{
    public class ProfileViewModel
    {
        public ICommand OpenSettingsCommand { get; }
        public ICommand GitHubCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand AdCommand { get; }
        public Services.Profile Profile { get; set; }

        public ObservableCollection<Routine> Workouts { get; }
        public ProfileViewModel()
        {
            Profile = AppState.Profile;
            OpenSettingsCommand = new Command(OpenSettings);
            GitHubCommand = new Command(OpenGitHub);
            ExportCommand = new Command(ExportData);
            ImportCommand = new Command(ImportData);
            Workouts = AppState.Workouts;
        }
        public ISeries[] Series { get; set; } = new ISeries[]
       {
            new ColumnSeries<int> { Values = new List<int> { 3, 4, 2 } },
            new ColumnSeries<int> { Values = new List<int> { 4, 2, 6 } },
            new ColumnSeries<double, DiamondGeometry> { Values = new List<double> { 4, 3, 4 } }
       };

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
    }
}
