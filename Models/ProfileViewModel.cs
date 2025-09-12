using GymTracker.Services;
using CommunityToolkit.Maui.Extensions;
using GymTracker.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Input;
using GymTracker.Views;

namespace GymTracker.Models
{
    public enum E_StatisticsLabels
    {
        ExerciseStats_HW, ExerciseStats_ORM, ExerciseStats_SetV, ExerciseStats_SessionV, ExerciseStats_S, 
        ExerciseStats_R, ExerciseStats_V, IMIntensity, IMVolume, IMSets, IMReps, IMIntensityTrend,
        IMVolumeTrend, IMSetsTrend, IMRepsTrend, MRWorkout, MRDuration, MRIntensity, MRVolume, MRSets, MRReps,
        MRWorkoutTrend, MRDurationTrend, MRIntensityTrend, MRVolumeTrend, MRSetsTrend, MRRepsTrend, MR_, MFIntensity,
        MFVolume, MFSets, MFReps, MFIntensityTrend, MFVolumeTrend, MFSetsTrend, MFRepsTrend, MGIntensity,
        MGVolume, MGSets, MGReps, MGIntensityTrend, MGVolumeTrend, MGSetsTrend, MGRepsTrend, 
    }


    public enum DisplayChoices
    {
        Empty, Heavy, Session, BestSet, OneRepMax, Sets, Reps, Intensity, Volume, Workout, Duration
    }

    public enum TimeChoices
    {
        Empty, Month, ThreeMonths, Year, All
    }
    public class ProfileViewModel
    {
        public SolidColorPaint LegendTextPaint { get; set; } =
        new SolidColorPaint
        {
            Color = new SKColor(255, 255, 255),
            SKTypeface = SKTypeface.FromFamilyName("Impact")
        };
        public ICommand OpenSettingsCommand { get; }
        public ICommand GitHubCommand { get; }
        public ICommand UnitSystemCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand ExercisesCommand { get; }
        public ICommand FilterCommand { get; }
        public ICommand EditSelectedExercise { get; }
        public ICommand DeleteSelectedExercises { get; }
        public ICommand SelectExercisesCommand { get; }
        public ICommand LoadMoreExercisesCommand { get; }
        public String EnteredExerciseName { get; set; }
        public Exercise SelectedExercise => AppState.SelectedExercise;
        public string SelectedExerciseName => SelectedExercise?.Name;

        public Label PieChartLabel = new Label();

        public List<(Button, Muscles)> MusclesButtons { get; set; } = new List<(Button, Muscles)>();
        public List<(Button, MuscleFunctions)> MuscleFunctionButtons { get; set; } = new List<(Button, MuscleFunctions)>();
        public List<(Button, MuscleGroups)> MuscleGroupsButtons { get; set; } = new List<(Button, MuscleGroups)>();

        public List<(Button, TimeChoices)> TimeButtons { get; set; } = new List<(Button, TimeChoices)>();
        public List<(Button, DisplayChoices)> DisplayButtons { get; set; } = new List<(Button, DisplayChoices)>();
        public Dictionary<E_StatisticsLabels, Label> StatisticsLabels { get; set; } = new Dictionary<E_StatisticsLabels, Label>();
        public CartesianChart ExerciseChart { get; set; }
        public CartesianChart MRChart { get; set; }
        public ISeries[] MRWorkout_Series { get; set; }
        public Axis[] MRWorkout_XAxes { get; set; }
        public Axis[] MRWorkout_YAxes { get; set; }
        public ISeries[] MRVolume_Series { get; set; }
        public Axis[] MRVolume_XAxes { get; set; }
        public Axis[] MRVolume_YAxes { get; set; }
        public ISeries[] MRIntensity_Series { get; set; }
        public Axis[] MRIntensity_XAxes { get; set; }
        public Axis[] MRIntensity_YAxes { get; set; }
        public ISeries[] MRDuration_Series { get; set; }
        public Axis[] MRDuration_XAxes { get; set; }
        public Axis[] MRDuration_YAxes { get; set; }
        public ISeries[] MRSets_Series { get; set; }
        public Axis[] MRSets_XAxes { get; set; }
        public Axis[] MRSets_YAxes { get; set; }
        public ISeries[] MRReps_Series { get; set; }
        public Axis[] MRReps_XAxes { get; set; }
        public Axis[] MRReps_YAxes { get; set; }

        public ISeries[] EHW_Series { get; set; }
        public Axis[] EHW_XAxes { get; set; }
        public Axis[] EHW_YAxes { get; set; }
        public ISeries[] ESV_Series { get; set; }
        public Axis[] ESV_XAxes { get; set; }
        public Axis[] ESV_YAxes { get; set; }
        public ISeries[] EBS_Series { get; set; }
        public Axis[] EBS_XAxes { get; set; }
        public Axis[] EBS_YAxes { get; set; }
        public ISeries[] EORM_Series { get; set; }
        public Axis[] EORM_XAxes { get; set; }
        public Axis[] EORM_YAxes { get; set; }
        public ISeries[] ES_Series { get; set; }
        public Axis[] ES_XAxes { get; set; }
        public Axis[] ES_YAxes { get; set; }
        public ISeries[] ER_Series { get; set; }
        public Axis[] ER_XAxes { get; set; }
        public Axis[] ER_YAxes { get; set; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> DisplayedExercises { get; set; }
        public ICommand MuscleGroupDistCommand { get; set; }
        public ICommand MuscleFunctionDistCommand { get; set; }
        public ICommand IndividualMuscleDistCommand { get; set; }
        public ICommand MainExercisesCommand { get; set; }
        public ICommand MonthlyReportCommand { get; set; }
        private Muscles _mc;
        public Muscles MC
        {
            get => _mc;
            set
            {
                if (_mc != value)
                {
                    _mc = value;
                    OnPropertyChanged(nameof(MC));
                    OnStatisticsButton();
                }
            }
        }
        private MuscleFunctions _mf;
        public MuscleFunctions MF
        {
            get => _mf;
            set
            {
                if (_mf != value)
                {
                    _mf = value;
                    OnPropertyChanged(nameof(MF));
                    OnStatisticsButton();
                }
            }
        }
        private MuscleGroups _mg;
        public MuscleGroups MG
        {
            get => _mg;
            set
            {
                if (_mg != value)
                {
                    _mg = value;
                    OnPropertyChanged(nameof(MG));
                    OnStatisticsButton();
                }
            }
        }
        private TimeChoices _tc;
        public TimeChoices TC
        {
            get => _tc;
            set
            {
                if (_tc != value)
                {
                    _tc = value;
                    OnPropertyChanged(nameof(TC));
                    OnStatisticsButton();
                }
            }
        }
        private DisplayChoices _dc;
        public DisplayChoices DC
        {
            get => _dc;
            set
            {
                if (_dc != value)
                {
                    _dc = value;
                    OnPropertyChanged(nameof(DC));
                    OnStatisticsButton();
                }
            }
        }
        public void OnStatisticsButton()
        {
            UpdateButtonColors();
            switch (AppState.profileStat)
            {
                case ProfileStats.MuscleGroup:
                    UpdateMuscleGroupChoices();
                    break;
                case ProfileStats.MuscleFunction:
                    UpdateMuscleFunctionChoices();
                    break;
                case ProfileStats.IndividualMuscle:
                    UpdateIndividualMuscleChoices();
                    break;
                case ProfileStats.Exercise:
                    UpdateExerciseStatsChoices();
                    break;
                case ProfileStats.Weekly:
                    UpdateWeeklyChartButtonColors();
                    UpdateWeeklyChartChoices();
                    break;
                case ProfileStats.MonthlyReport:
                    UpdateMonthlyReportChoices();
                    break;
            }
        }
        public Label IM_InfoLabel { get; set; }

        public PieChart IMChart { get; set; }

        public Label MF_InfoLabel { get; set; }

        public PieChart MFChart { get; set; }

        public Label MG_InfoLabel { get; set; }
        public PieChart MGChart { get; set; }
        public ICommand StatisticsCommand { get; }
        public Button WeeklyStatsVolumeButton { get; set; }
        public Button WeeklyStatsDurationButton { get; set; }
        public Button WeeklyStatsSetsButton { get; set; }
        public Button WeeklyStatsRepsButton { get; set; }

        public CartesianChart WeeklyChart { get; set; }
        public Services.Profile Profile { get; set; }

        public ObservableCollection<Routine> Workouts { get; }
        private List<WeekStat> WeeklyStats { get; set; }
        private List<ExerciseRecord> ExerciseRecords { get; set; }

        public ProfileViewModel()
        {
            DisplayedExercises = new ObservableCollection<Exercise>();
            AppState.MaxExercises = 20;
            AppState.FillDisplayedExercises(DisplayedExercises);
            Profile = AppState.Profile;
            OpenSettingsCommand = new Command(OpenSettings);
            GitHubCommand = new Command(OpenGitHub);
            UnitSystemCommand = new Command(ToggleWeightUnit);
            ExercisesCommand = new Command(OnExercises);
            StatisticsCommand = new Command(OnStatistics);
            LoadMoreExercisesCommand = new Command(() => { 
                AppState.MaxExercises += 20;
                AppState.FillDisplayedExercises(DisplayedExercises);
            });
            SelectExerciseCommand = new Command<Exercise>(async (Exercise exercise) => {
                AppState.SelectedExercise = new Exercise(exercise);
                OnPropertyChanged(nameof(SelectedExercise));
                AppState.profileStat = ProfileStats.Exercise;
                await Shell.Current.GoToAsync("profileexercisestats");
            });
            SelectExercisesCommand = new Command<Exercise>((Exercise exercise) => {
                
                exercise.IsSelected = !exercise.IsSelected;

                if (exercise.IsSelected)
                {
                    if (!AppState.SelectedExercises.Any(e => e.Name == exercise.Name))
                    {
                        AppState.SelectedExercises.Add(new Exercise(exercise));
                        AppState.SelectedExerciseIds.Add(exercise.Name);
                    }
                }
                else
                {
                    var toRemove = AppState.SelectedExercises.FirstOrDefault(e => e.Name == exercise.Name);
                    if (toRemove != null)
                        AppState.SelectedExercises.Remove(toRemove);

                    AppState.SelectedExerciseIds.Remove(exercise.Name);
                }
            });
            MuscleGroupDistCommand = new Command( async() => {
                AppState.profileStat = ProfileStats.MuscleGroup;
                await Shell.Current.GoToAsync("profilemusclegroup"); 
            });
            MuscleFunctionDistCommand = new Command(async () => {
                AppState.profileStat = ProfileStats.MuscleFunction;
                await Shell.Current.GoToAsync("profilemusclefunction");
            });
            IndividualMuscleDistCommand = new Command(async () => {
                AppState.profileStat = ProfileStats.IndividualMuscle;
                await Shell.Current.GoToAsync("profileindimuscle");
            });
            MonthlyReportCommand = new Command(async () => {
                AppState.profileStat = ProfileStats.MonthlyReport;
                await Shell.Current.GoToAsync("profilemonthlyreport");
            });

            EditSelectedExercise = new Command(async () => {
                if(AppState.SelectedExercises?.Count() < 1)
                {
                    await Shell.Current.DisplayAlert("Error", "No exercise is selected.", "OK");
                    return;
                }
                else if(AppState.SelectedExercises?.Count > 1)
                {
                    await Shell.Current.DisplayAlert("Error", "It is only possible to edit 1 exercise at a time.", "OK");
                    return;
                }
                else if(!AppState.AllExercises.Any(e => e.Name.Equals(AppState.SelectedExercises.ElementAt(0).Name, StringComparison.OrdinalIgnoreCase)))
                {
                    await Shell.Current.DisplayAlert("Error", "DogoFit's exercise list does not contain an exercise with the same name.", "OK");
                    return;
                }
                AppState.profileExercise = ProfileExercise.Edit;
                AppState.EditedExercise = AppState.SelectedExercises.ElementAt(0);
                await Shell.Current.GoToAsync("profileaddexercise");
            });
            DeleteSelectedExercises = new Command(async () => {
                if(AppState.SelectedExercises?.Count() < 1)
                {
                    await Shell.Current.DisplayAlert("Error", "No exercise is selected.", "OK");
                    return;
                }
                string error = AppState.RemoveExercises(AppState.SelectedExercises);
                await Shell.Current.DisplayAlert("Info", error, "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            });
            Categories = AppState.Categories;
            AppState.FilterByCategory("All", false);
            FilterCommand = new Command<Category>((Category cat) => { AppState.SelectCategory(cat); AppState.FillDisplayedExercises(DisplayedExercises); });
            Workouts = AppState.Workouts;
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

        public void UpdateButtonColors()
        {
            foreach(var s in MusclesButtons)
            {
                if(s.Item1 != null)
                {
                    s.Item1.BackgroundColor = MC == s.Item2 ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
                }
            }
            foreach(var s in MuscleFunctionButtons)
            {
                if(s.Item1 != null)
                {
                    s.Item1.BackgroundColor = MF == s.Item2 ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
                }
            }
            foreach(var s in MuscleGroupsButtons)
            {
                if(s.Item1 != null)
                {
                    s.Item1.BackgroundColor = MG == s.Item2 ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
                }
            }
            foreach (var s in TimeButtons)
            {
                if (s.Item1 != null)
                {
                    s.Item1.BackgroundColor = TC == s.Item2 ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
                }
            }
            foreach (var s in DisplayButtons)
            {
                if (s.Item1 != null)
                {
                    s.Item1.BackgroundColor = DC == s.Item2 ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
                }
            }
        }
        public void UpdateMonthlyReportChoices()
        {
            if (DC != DisplayChoices.Workout && DC != DisplayChoices.Volume && DC != DisplayChoices.Intensity
                && DC != DisplayChoices.Duration && DC != DisplayChoices.Sets && DC != DisplayChoices.Reps)
                return;
            if (MRChart == null)
                return;

            UpdateMRChart();

        }
        public void SetMonthlyReportLabels(Dictionary<E_StatisticsLabels, Label> labels)
        {
            if (AppState.Workouts == null)
                return;
            var now = DateTime.Now;
            var filteredWorkoutsNow = AppState.Workouts
                .Where(w => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year)
                .ToList();
            var lastMonth = now.AddMonths(-1);
            var filteredWorkoutsLastMonth = AppState.Workouts
                .Where(w => w.StartTime.Month == lastMonth.Month && w.StartTime.Year == lastMonth.Year)
                .ToList();


            var workoutsCountNow = filteredWorkoutsNow.Count();
            var workoutsCountLastMonth = filteredWorkoutsLastMonth.Count();

            var durationNow = filteredWorkoutsNow.Sum(w => w.Duration.TotalSeconds);
            var durationLastMonth = filteredWorkoutsLastMonth.Sum(w => w.Duration.TotalSeconds);

            var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises).ToList();
            var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises).ToList();

            int setsNow = exercisesNow.SelectMany(e => e.CheckedSets).Count();
            int repsNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
            double volumeNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
            double intensityNow = repsNow != 0 ? volumeNow / repsNow : 0;

            int setsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Count();
            int repsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
            double volumeLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
            double intensityLastMonth = repsLastMonth != 0 ? volumeLastMonth / repsLastMonth : 0;

            labels[E_StatisticsLabels.MRIntensity].Text = intensityNow.ToString("F2") + Profile.WeightUnit + " per rep";
            labels[E_StatisticsLabels.MRVolume].Text = volumeNow.ToString("F2") + Profile.WeightUnit;
            labels[E_StatisticsLabels.MRReps].Text = repsNow.ToString();
            labels[E_StatisticsLabels.MRSets].Text = setsNow.ToString();
            labels[E_StatisticsLabels.MRWorkout].Text = workoutsCountNow.ToString();
            labels[E_StatisticsLabels.MRDuration].Text = FormatDuration(TimeSpan.FromSeconds(Math.Abs(durationNow)));

            int workoutsDiff = workoutsCountNow - workoutsCountLastMonth;
            double durationDiff = durationNow - durationLastMonth;
            double intensityDiff = intensityNow - intensityLastMonth;
            double volumeDiff = volumeNow - volumeLastMonth;
            int repsDiff = repsNow - repsLastMonth;
            int setsDiff = setsNow - setsLastMonth;
            UpdateLabel(intensityDiff, E_StatisticsLabels.MRIntensityTrend, labels);
            UpdateLabel(volumeDiff, E_StatisticsLabels.MRVolumeTrend, labels);
            UpdateLabel(repsDiff, E_StatisticsLabels.MRRepsTrend, labels);
            UpdateLabel(setsDiff, E_StatisticsLabels.MRSetsTrend, labels);
            UpdateLabel(workoutsDiff, E_StatisticsLabels.MRWorkoutTrend, labels);

            var ts = TimeSpan.FromSeconds(durationDiff);
            if (durationDiff >= 0)
            {
                labels[E_StatisticsLabels.MRDurationTrend].TextColor = Colors.Green;
                if(ts.Hours >= 24)
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2191 " + FormatDuration(ts);
                else
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2191 " + FormatDuration(ts);
            }
            else
            {
                labels[E_StatisticsLabels.MRDurationTrend].TextColor = Colors.Red;
                if (ts.Hours >= 24)
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2193 " + FormatDuration(ts);
                else
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2193 " + FormatDuration(ts);
            }
        }

        private void UpdateLabel(double diff, E_StatisticsLabels label, Dictionary<E_StatisticsLabels, Label> labels)
        {
            if (diff >= 0)
            {
                labels[label].TextColor = Colors.Green;
                labels[label].Text = "\u2191 " + Math.Abs(diff).ToString("N0");
            }
            else
            {
                labels[label].TextColor = Colors.Red;
                labels[label].Text = "\u2193 " + Math.Abs(diff).ToString("N0");
            }
        }

        private string FormatDuration(TimeSpan ts)
        {
            if (ts == TimeSpan.Zero)
                return "00:00:00";
        
            var parts = new List<string>();
        
            if (ts.TotalDays >= 1)
                parts.Add($"{(int)ts.TotalDays}d");
            if (ts.Hours > 0)
                parts.Add($"{ts.Hours}h");
            if (ts.Minutes > 0)
                parts.Add($"{ts.Minutes} min");
            if (ts.TotalDays < 1 && ts.TotalHours < 1 && ts.Seconds > 0)
                parts.Add($"{ts.Seconds} sec");
        
            return string.Join(" ", parts);
        }




        public void UpdateMRChart()
        {
            if (AppState.Workouts == null || MRChart == null || DC == DisplayChoices.Empty)
                return;

            var filteredWorkouts = AppState.Workouts
                .Where(w => w.StartTime <= DateTime.Now)
                .ToList();

            if (!filteredWorkouts.Any())
                return;

            var firstMonth = new DateTime(filteredWorkouts.Min(w => w.StartTime).Year, filteredWorkouts.Min(w => w.StartTime).Month, 1);
            var lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            int monthsCount = ((lastMonth.Year - firstMonth.Year) * 12) + lastMonth.Month - firstMonth.Month + 1;

            Func<Routine, double> statSelector = DC switch
            {
                DisplayChoices.Workout => w => 1,
                DisplayChoices.Duration => w => w.Duration.TotalMinutes,
                DisplayChoices.Volume => w => w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps) * w.Exercises.SelectMany(e => e.CheckedSets).Count(),
                DisplayChoices.Intensity => w => { var reps = w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                    var volume = w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps) * w.Exercises.SelectMany(e => e.CheckedSets).Count();
                    return reps != 0 ? volume / reps : 0;
                    },
                DisplayChoices.Sets => w => w.Exercises.SelectMany(e => e.CheckedSets).Count(),
                DisplayChoices.Reps => w => w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps),
                _ => w => 0
            }; 

            var labels = new List<string>();
            var values = new List<double>();

            for (int i = 0; i < monthsCount; i++)
            {
                var monthDate = firstMonth.AddMonths(i);

                var monthWorkouts = filteredWorkouts
                    .Where(w => w.StartTime.Year == monthDate.Year && w.StartTime.Month == monthDate.Month);

                double monthValue = monthWorkouts.Sum(statSelector);

                labels.Add(monthDate.ToString("MMM yyyy"));
                values.Add(monthValue);
            }

            MRChart.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = values,
                    Name = DC.ToString()
                }
            };

            MRChart.XAxes = new[]
            {
                new Axis
                {
                    Labels = labels,
                    Name = "Month"
                }
            };

            MRChart.YAxes = new[]
            {
                new Axis
                {
                    Name = DC.ToString()
                }
            };
        }


        public void UpdateIndividualMuscleChoices()
        {
            if (IMChart == null)
                return;
            IM_InfoLabel.Text = AppState.MGMToString(MC) + " Current versus Last Month";
            RecalculateIMChart();
            UpdateIndividualMuscleLabels();
        }

        private void UpdateIndividualMuscleLabels()
        {
            if (AppState.Workouts == null)
                return;
            var now = DateTime.Now;
            var filteredWorkoutsNow = AppState.Workouts
                .Where(w => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year)
                .ToList();
            var filteredWorkoutsLastMonth = AppState.Workouts
                .Where(w => w.StartTime >= now.AddMonths(-1))
                .ToList();


            void UpdateLabelsForIndividualMuscle(string im)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.TargetMuscle.ToString() == im)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.TargetMuscle.ToString() == im)
                                                                  .ToList();

                int setsNow = exercisesNow.SelectMany(e => e.CheckedSets).Count();
                int repsNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityNow = repsNow != 0 ? volumeNow / repsNow : 0;

                int setsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Count();
                int repsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityLastMonth = repsLastMonth != 0 ? volumeLastMonth / repsLastMonth : 0;

                StatisticsLabels[E_StatisticsLabels.IMIntensity].Text = intensityNow.ToString("F2") + Profile.WeightUnit + " per rep";
                StatisticsLabels[E_StatisticsLabels.IMVolume].Text = volumeNow.ToString("F2") + Profile.WeightUnit;
                StatisticsLabels[E_StatisticsLabels.IMReps].Text = repsNow.ToString();
                StatisticsLabels[E_StatisticsLabels.IMSets].Text = setsNow.ToString();

                double intensityDiff = intensityNow - intensityLastMonth;
                double volumeDiff = volumeNow - volumeLastMonth;
                int repsDiff = repsNow - repsLastMonth;
                int setsDiff = setsNow - setsLastMonth;
                UpdateLabel(intensityDiff, E_StatisticsLabels.IMIntensityTrend, StatisticsLabels);
                UpdateLabel(volumeDiff, E_StatisticsLabels.IMVolumeTrend, StatisticsLabels);
                UpdateLabel(repsDiff, E_StatisticsLabels.IMRepsTrend, StatisticsLabels);
                UpdateLabel(setsDiff, E_StatisticsLabels.IMSetsTrend, StatisticsLabels);
            }
            UpdateLabelsForIndividualMuscle(AppState.MGMToString(MC));
        }
        private void RecalculateIMChart()
        {
            if (AppState.Workouts == null)
                return;
            if (DC == DisplayChoices.Empty || TC == TimeChoices.Empty)
                return;
            var now = DateTime.Now;
            var filteredWorkouts = AppState.Workouts
                .Where(w => TC switch
                {
                    TimeChoices.Month => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year,
                    TimeChoices.ThreeMonths => w.StartTime >= now.AddMonths(-3),
                    TimeChoices.Year => w.StartTime.Year == now.Year,
                    TimeChoices.All => true,
                    _ => false
                })
                .ToList();
            var allExercises = filteredWorkouts.SelectMany(w => w.Exercises).ToList();
            UpdatePieChart<Muscles>(AppState.MusclesList, IMChart, allExercises);
        }
        
        public void UpdateMuscleFunctionChoices()
        {

            if (MFChart == null)
                return;
            MF_InfoLabel.Text = AppState.MuscleFunctionToString(MF) + " Current versus Last Month";
            RecalculateMFChart();
            UpdateMuscleFunctionLabels();
        }

        private void UpdateMuscleFunctionLabels()
        {
            if (AppState.Workouts == null)
                return;
            var now = DateTime.Now;
            var filteredWorkoutsNow = AppState.Workouts
                .Where(w => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year)
                .ToList();
            var filteredWorkoutsLastMonth = AppState.Workouts
                .Where(w => w.StartTime >= now.AddMonths(-1))
                .ToList();


            void UpdateLabelsForMuscleFunction(string mf)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.Function.ToString() == mf)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.Function.ToString() == mf)
                                                                  .ToList();

                int setsNow = exercisesNow.SelectMany(e => e.CheckedSets).Count();
                int repsNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityNow = repsNow != 0 ? volumeNow / repsNow : 0;

                int setsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Count();
                int repsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityLastMonth = repsLastMonth != 0 ? volumeLastMonth / repsLastMonth : 0;

                StatisticsLabels[E_StatisticsLabels.MFIntensity].Text = intensityNow.ToString("F2") + Profile.WeightUnit + " per rep";
                StatisticsLabels[E_StatisticsLabels.MFVolume].Text = volumeNow.ToString("F2") + Profile.WeightUnit;
                StatisticsLabels[E_StatisticsLabels.MFReps].Text = repsNow.ToString();
                StatisticsLabels[E_StatisticsLabels.MFSets].Text = setsNow.ToString();

                double intensityDiff = intensityNow - intensityLastMonth;
                double volumeDiff = volumeNow - volumeLastMonth;
                int repsDiff = repsNow - repsLastMonth;
                int setsDiff = setsNow - setsLastMonth;
                UpdateLabel(intensityDiff, E_StatisticsLabels.MFIntensityTrend, StatisticsLabels);
                UpdateLabel(volumeDiff, E_StatisticsLabels.MFVolumeTrend, StatisticsLabels);
                UpdateLabel(repsDiff, E_StatisticsLabels.MFRepsTrend, StatisticsLabels);
                UpdateLabel(setsDiff, E_StatisticsLabels.MFSetsTrend, StatisticsLabels);
            }

            UpdateLabelsForMuscleFunction(AppState.MuscleFunctionToString(MF));
        }
        private void RecalculateMFChart()
        {
            if (AppState.Workouts == null)
                return;
            if (TC == TimeChoices.Empty || DC == DisplayChoices.Empty)
                return;
            var now = DateTime.Now;
            var filteredWorkouts = AppState.Workouts
                .Where(w => TC switch
                {
                    TimeChoices.Month => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year,
                    TimeChoices.ThreeMonths => w.StartTime >= now.AddMonths(-3),
                    TimeChoices.Year => w.StartTime.Year == now.Year,
                    TimeChoices.All => true,
                    _ => false
                })
                .ToList();

            var allExercises = filteredWorkouts.SelectMany(w => w.Exercises).ToList();
            UpdatePieChart<MuscleFunctions>(AppState.MuscleFunctionsList, MFChart, allExercises);
        }

        private void UpdatePieChart<T>(IEnumerable<T> Muscles, PieChart chart, List<Exercise> exercises)
        {
            PieChartLabel.IsVisible = false;
            chart.HeightRequest = 500;
            if(exercises.Count <= 0)
            {
                chart.HeightRequest = 0;
                PieChartLabel.IsVisible = true;
                return;
            }
            Dictionary<String, double> muscleSets = new Dictionary<String, double>();
            Dictionary<String, double> muscleReps = new Dictionary<String, double>();
            Dictionary<String, double> muscleVolume = new Dictionary<String, double>();
            Dictionary<String, double> muscleIntensity = new Dictionary<String, double>();
            foreach(var e in Muscles)
            {
                List<Exercise> mExercises = new List<Exercise>();
                switch(e)
                {
                    case Muscles m:
                        mExercises = exercises.Where(e => AppState.MGMToString(e.TargetMuscle) == AppState.MGMToString(m)).ToList();
                        break;
                    case MuscleFunctions f:
                        mExercises = exercises.Where(e => AppState.MGMToString(e.Function) == AppState.MGMToString(f)).ToList();
                        break;
                    case MuscleGroups g:
                        mExercises = exercises.Where(e => AppState.MGMToString(e.MuscleGroup) == AppState.MGMToString(g)).ToList();
                        break;
                    default:
                        return;

                }
                muscleSets.Add(AppState.MGMToString(e), mExercises.SelectMany(s => s.CheckedSets).Count());
                muscleReps.Add(AppState.MGMToString(e), mExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps));
                muscleVolume.Add(AppState.MGMToString(e), mExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight));
                muscleIntensity.Add(AppState.MGMToString(e), GetIntensity(mExercises));
            }
            switch (DC)
            {
                case DisplayChoices.Sets:
                    {
                        UpdateValuePieChart(chart, muscleSets);
                        break;
                    }

                case DisplayChoices.Reps:
                    {
                        UpdateValuePieChart(chart, muscleReps);
                        break;
                    }

                case DisplayChoices.Volume:
                    {
                        UpdateValuePieChart(chart, muscleVolume);
                        break;
                    }

                case DisplayChoices.Intensity:
                    {
                        UpdateValuePieChart(chart, muscleIntensity);
                        break;
                    }
            }
        }

        private void UpdateValuePieChart(PieChart chart, Dictionary<String, double> stats)
        {
            var max = stats.Values.Max();
            if(max <= 0)
            {
                chart.HeightRequest = 0;
                PieChartLabel.IsVisible = true;
                return;
            }
            chart.Series = stats.Where(kv => kv.Value > 0).Select(kv => new PieSeries<double>
            {           
                Name = kv.Key,
                Values = new[] { kv.Value },
                Pushout = kv.Value == max ? 30 : 0
            }).ToArray();
        }


        public void UpdateMuscleGroupChoices()
        {

            if (MGChart == null)
                return;

            MG_InfoLabel.Text = AppState.MuscleGroupToString(MG) + " Current versus Last Month";
            RecalculateMGChart();
            UpdateMuscleGroupLabels();
        }

        private void UpdateMuscleGroupLabels()
        {
            if (AppState.Workouts == null)
                return;
            var now = DateTime.Now;
            var filteredWorkoutsNow = AppState.Workouts
                .Where(w => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year)
                .ToList();
            var filteredWorkoutsLastMonth = AppState.Workouts
                .Where(w => w.StartTime >= now.AddMonths(-1))
                .ToList();

            void UpdateLabelsForMuscleGroup(string mg)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.MuscleGroup.ToString() == mg)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.MuscleGroup.ToString() == mg)
                                                                  .ToList();

                int setsNow = exercisesNow.SelectMany(e => e.CheckedSets).Count();
                int repsNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeNow = exercisesNow.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityNow = repsNow != 0 ? volumeNow / repsNow : 0;

                int setsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Count();
                int repsLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                double volumeLastMonth = exercisesLastMonth.SelectMany(e => e.CheckedSets).Sum(s => s.Reps * s.Weight);
                double intensityLastMonth = repsLastMonth != 0 ? volumeLastMonth / repsLastMonth : 0;

                StatisticsLabels[E_StatisticsLabels.MGIntensity].Text = intensityNow.ToString("F2") + Profile.WeightUnit + " per rep";
                StatisticsLabels[E_StatisticsLabels.MGVolume].Text = volumeNow.ToString("F2") + Profile.WeightUnit;
                StatisticsLabels[E_StatisticsLabels.MGReps].Text = repsNow.ToString();
                StatisticsLabels[E_StatisticsLabels.MGSets].Text = setsNow.ToString();

                double intensityDiff = intensityNow - intensityLastMonth;
                double volumeDiff = volumeNow - volumeLastMonth;
                int repsDiff = repsNow - repsLastMonth;
                int setsDiff = setsNow - setsLastMonth;
                UpdateLabel(intensityDiff, E_StatisticsLabels.MGIntensityTrend, StatisticsLabels);
                UpdateLabel(volumeDiff, E_StatisticsLabels.MGVolumeTrend, StatisticsLabels);
                UpdateLabel(repsDiff, E_StatisticsLabels.MGRepsTrend, StatisticsLabels);
                UpdateLabel(setsDiff, E_StatisticsLabels.MGSetsTrend, StatisticsLabels);
            }

            UpdateLabelsForMuscleGroup(AppState.MuscleGroupToString(MG));
        }

        private void RecalculateMGChart()
        {
            if (AppState.Workouts == null)
                return;
            if (TC == TimeChoices.Empty || DC == DisplayChoices.Empty)
                return;
            var now = DateTime.Now;
            var filteredWorkouts = AppState.Workouts
                .Where(w => TC switch
                {
                    TimeChoices.Month => w.StartTime.Month == now.Month && w.StartTime.Year == now.Year,
                    TimeChoices.ThreeMonths => w.StartTime >= now.AddMonths(-3),
                    TimeChoices.Year => w.StartTime.Year == now.Year,
                    TimeChoices.All => true,
                    _ => false
                })
                .ToList();

            var allExercises = filteredWorkouts.SelectMany(w => w.Exercises).ToList();
            UpdatePieChart<MuscleGroups>(AppState.MuscleGroupsList, MGChart, allExercises);
        }
        private double GetIntensity(IEnumerable<Exercise> exercises)
        {
            var totalVolume = exercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight);
            var totalReps = exercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps);
            return totalReps != 0 ? (double)totalVolume / totalReps : 0;
        }

        public void UpdateExerciseStatsChoices()
        {
            if (DC != DisplayChoices.Heavy && DC != DisplayChoices.Session && DC != DisplayChoices.BestSet && DC != DisplayChoices.OneRepMax &&
                DC != DisplayChoices.Sets && DC != DisplayChoices.Reps)
                return;
            if (ExerciseChart == null)
                return;
            UpdateExerciseStats();
            switch (DC)
            {
                case DisplayChoices.Heavy:
                    ExerciseChart.Series = EHW_Series;
                    ExerciseChart.XAxes = EHW_XAxes;
                    ExerciseChart.YAxes = EHW_YAxes;
                    break;
                case DisplayChoices.Session:
                    ExerciseChart.Series = ESV_Series;
                    ExerciseChart.XAxes = ESV_XAxes;
                    ExerciseChart.YAxes = ESV_YAxes;
                    break;
                case DisplayChoices.BestSet:
                    ExerciseChart.Series = EBS_Series;
                    ExerciseChart.XAxes = EBS_XAxes;
                    ExerciseChart.YAxes = EBS_YAxes;
                    break;
                case DisplayChoices.OneRepMax:
                    ExerciseChart.Series = EORM_Series;
                    ExerciseChart.XAxes = EORM_XAxes;
                    ExerciseChart.YAxes = EORM_YAxes;
                    break;
                case DisplayChoices.Sets:
                    ExerciseChart.Series = ES_Series;
                    ExerciseChart.XAxes = ES_XAxes;
                    ExerciseChart.YAxes = ES_YAxes;
                    break;
                case DisplayChoices.Reps:
                    ExerciseChart.Series = ER_Series;
                    ExerciseChart.XAxes = ER_XAxes;
                    ExerciseChart.YAxes = ER_YAxes;
                    break;
            }
            ExerciseChart?.InvalidateMeasure();
        }

        public void UpdateExerciseStats()
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var dfi = DateTimeFormatInfo.CurrentInfo;

            ExerciseRecords = Workouts
                .GroupBy(w => new
                {
                    Year = w.StartTime.Year,
                    Week = calendar.GetWeekOfYear(
                        w.StartTime,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday)
                })
                .Select(g => new ExerciseRecord
                {
                    WeekNumber = g.Key.Week,
                    Year = g.Key.Year,
                    StartDate = FirstDateOfWeekIso8601(g.Key.Year, g.Key.Week),
                    SessionVolume = g.SelectMany(e => e.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .SelectMany(s => s.CheckedSets)
                        .Sum(r => (double)r.Reps * r.Weight),
                    BestSet = g.SelectMany(e => e.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .SelectMany(s => s.CheckedSets)
                        .OrderByDescending(r => r.Weight * r.Reps)
                        .Select(r => (r.Weight, r.Reps))
                        .FirstOrDefault(),
                    HeaviestWeight = g.SelectMany(e => e.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .SelectMany(s => s.CheckedSets)
                        .Select(r => r.Weight)
                        .DefaultIfEmpty(0)
                        .Max(),
                    OneRepMax = g.SelectMany(e => e.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .SelectMany(s => s.CheckedSets)
                        .Select(r => r.Weight * (1 + r.Reps / 30.0))
                        .DefaultIfEmpty(0)
                        .Max(),
                    Reps = g.SelectMany(w => w.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .SelectMany(exe => exe.CheckedSets)
                        .Sum(s => s.Reps),
                    Sets = g.SelectMany(w => w.Exercises)
                        .Where(exe => exe.Name == SelectedExercise.Name)
                        .Sum(exe => exe.Sets.Count(s => s.IsChecked))
                })
                .OrderBy(s => s.Year)
                .ThenBy(s => s.WeekNumber)
                .ToList();


            UpdateExerciseCharts();
            if (ExerciseRecords.Any())
            {

                StatisticsLabels[E_StatisticsLabels.ExerciseStats_HW].Text = ExerciseRecords
                    .OrderByDescending(h => h.HeaviestWeight)
                    .FirstOrDefault()
                    .HeaviestWeight.ToString("0.##") + Profile.WeightUnit;

                StatisticsLabels[E_StatisticsLabels.ExerciseStats_ORM].Text = ExerciseRecords
                    .OrderByDescending(h => h.OneRepMax)
                    .FirstOrDefault()
                    .OneRepMax.ToString("0.##") + Profile.WeightUnit;

                var bestSet = ExerciseRecords
                    .OrderByDescending(h => h.BestSet.Item1 * h.BestSet.Item2)
                    .FirstOrDefault()
                    .BestSet;

                StatisticsLabels[E_StatisticsLabels.ExerciseStats_SetV].Text = $"{bestSet.Item1:0.##}{Profile.WeightUnit} x {bestSet.Item2}";

                StatisticsLabels[E_StatisticsLabels.ExerciseStats_SessionV].Text = ExerciseRecords
                    .OrderByDescending(h => h.SessionVolume)
                    .FirstOrDefault()
                    .SessionVolume.ToString("0.##") + Profile.WeightUnit;

                StatisticsLabels[E_StatisticsLabels.ExerciseStats_S].Text = ExerciseRecords.Sum(h => h.Sets).ToString();
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_R].Text = ExerciseRecords.Sum(h => h.Reps).ToString();
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_V].Text = ExerciseRecords.Sum(h => h.SessionVolume).ToString("0.##") + Profile.WeightUnit;
            }
            else
            {
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_HW].Text = "N/A";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_ORM].Text = "N/A";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_SetV].Text = "N/A";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_SessionV].Text = "N/A";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_S].Text = "0";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_R].Text = "0";
                StatisticsLabels[E_StatisticsLabels.ExerciseStats_V].Text = "0";
            }
        }

        public void UpdateExerciseCharts()
        {
            EHW_Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Heaviest Weight",
                    Values = ExerciseRecords.Select(s => s.HeaviestWeight).ToArray()
                }
            };

            EHW_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            EHW_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} {Profile.WeightUnit}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };

            ESV_Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Session Volume",
                    Values = ExerciseRecords.Select(s => s.SessionVolume).ToArray()
                }
            };

            ESV_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            ESV_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} {Profile.WeightUnit}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };

            EBS_Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Session Volume",
                    Values = ExerciseRecords.Select(s => s.BestSet.Item1 * s.BestSet.Item2).ToArray()
                }
            };

            EBS_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            EBS_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} {Profile.WeightUnit}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };

            EORM_Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "One Rep Max",
                    Values = ExerciseRecords.Select(s => s.OneRepMax).ToArray()
                }
            };

            EORM_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            EORM_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} {Profile.WeightUnit}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };

            ES_Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Sets",
                    Values = ExerciseRecords.Select(s => s.Sets).ToArray()
                }
            };

            ES_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            ES_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };

            ER_Series = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Name = "Reps",
                    Values = ExerciseRecords.Select(s => s.Reps).ToArray()
                }
            };

            ER_XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = ExerciseRecords.Select(s => s.StartDate.ToString("yyyy MMM d")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            ER_YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };
        }
        public void UpdateWeeklyChartChoices()
        {
            if (DC != DisplayChoices.Volume && DC != DisplayChoices.Duration && DC != DisplayChoices.Sets && DC != DisplayChoices.Reps)
                return;

            if (WeeklyChart == null) 
                return;
            UpdateWeeklyCharts();
            switch (DC)
            {
                case DisplayChoices.Volume:
                    WeeklyChart.Series = TotalVolumeSeries;
                    WeeklyChart.XAxes = TotalVolumeXAxes;
                    WeeklyChart.YAxes = TotalVolumeYAxes;
                    break;
                case DisplayChoices.Duration:
                    WeeklyChart.Series = TotalDurationSeries;
                    WeeklyChart.XAxes = TotalDurationXAxes;
                    WeeklyChart.YAxes = TotalDurationYAxes;
                    break;
                case DisplayChoices.Sets:
                    WeeklyChart.Series = TotalSetsSeries;
                    WeeklyChart.XAxes = TotalSetsXAxes;
                    WeeklyChart.YAxes = TotalSetsYAxes;
                    break;
                case DisplayChoices.Reps:
                    WeeklyChart.Series = TotalRepsSeries;
                    WeeklyChart.XAxes = TotalRepsXAxes;
                    WeeklyChart.YAxes = TotalRepsYAxes;
                    break;
            }
            WeeklyChart?.InvalidateMeasure();
        }
        private void UpdateWeeklyChartButtonColors()
        {
            WeeklyStatsVolumeButton.BackgroundColor = DC == DisplayChoices.Volume ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsDurationButton.BackgroundColor = DC == DisplayChoices.Duration ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsSetsButton.BackgroundColor = DC == DisplayChoices.Sets ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");
            WeeklyStatsRepsButton.BackgroundColor = DC == DisplayChoices.Reps ? Color.FromArgb("#008cff") : Color.FromArgb("#2b2b2b");

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
                    Labels = WeeklyStats.Select(s => s.StartDate.ToString("MMM d, yyyy")).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray)
                }
            };

            TotalVolumeYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => $"{value:N0} {Profile.WeightUnit}",
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };


            TotalDurationSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Name = "Duration",
                    Values = WeeklyStats.Select(s => s.Duration.TotalSeconds).ToArray()
                }
            };

            TotalDurationXAxes = TotalVolumeXAxes;
            TotalDurationYAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value =>
                    {
                        var ts = TimeSpan.FromSeconds(value);
                    
                        if (ts.TotalHours >= 24)
                            return $"{ts.Days}d {ts.Hours:00}:{ts.Minutes:00}";
                        else
                            return $"{(int)ts.TotalHours}:{ts.Minutes:00}:{ts.Seconds:00}";
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
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
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
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
                    SeparatorsPaint = new SolidColorPaint(SKColors.LightGray),
                    MinStep = 10
                }
            };
        }

        public void UpdateWeeklyStats()
        {
            var calendar = CultureInfo.CurrentCulture.Calendar;
            var dfi = DateTimeFormatInfo.CurrentInfo;

            WeeklyStats = Workouts
                .GroupBy(w => new
                {
                    Year = w.StartTime.Year,
                    Week = calendar.GetWeekOfYear(
                        w.StartTime,
                        CalendarWeekRule.FirstFourDayWeek,
                        DayOfWeek.Monday)
                })
                .Select(g => new WeekStat
                {
                    WeekNumber = g.Key.Week,
                    Year = g.Key.Year,
                    StartDate = FirstDateOfWeekIso8601(g.Key.Year, g.Key.Week),
                    Volume = g.Sum(w => w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps) * w.Exercises.SelectMany(e => e.CheckedSets).Count()),
                    Duration = TimeSpan.FromTicks(g.Sum(w => w.Duration.Ticks)),
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

        private async void OnStatistics()
        {
            await Shell.Current.GoToAsync("profilemorestatistics");
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
            public ObservableCollection<Exercise> Exercises { get; set; }
        }
        public static DateTime FirstDateOfWeekIso8601(int year, int weekOfYear)
        {
            DateTime jan4 = new DateTime(year, 1, 4);
            int dayOfWeek = (int)jan4.DayOfWeek;
            dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
            DateTime firstMonday = jan4.AddDays(1 - dayOfWeek);
            return firstMonday.AddDays((weekOfYear - 1) * 7);
        }

        public void AddTimeButton(TimeChoices tc, String text, HorizontalStackLayout container)
        {
            Button button = new Button { Text = text, TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
            button.Clicked += (s, e) => TC = tc;
            container.Children.Add(button);
            TimeButtons.Add((button, tc));
        }
        public void AddDisplayButton(DisplayChoices dc, String text, HorizontalStackLayout container)
        {
            Button button = new Button { Text = text, TextColor = Colors.White, FontSize = 15.0, BackgroundColor = Color.FromArgb("#008cff") };
            button.Clicked += (s, e) => DC = dc;
            container.Children.Add(button);
            DisplayButtons.Add((button, dc));
        }

        public void AddLabel(E_StatisticsLabels e, VerticalStackLayout container)
        {
            Label label = new Label { Text = "-", TextColor = Colors.Gray, HorizontalTextAlignment = TextAlignment.Start, FontAttributes = FontAttributes.Bold };
            container.Children.Add(label);
            StatisticsLabels.Add(e, label);
        }

        public void AddLabel(E_StatisticsLabels e, Grid container)
        {
            Label label = new Label { Text = "", TextColor = Color.FromRgba("#008cff"), BackgroundColor = Colors.Black, HorizontalOptions = LayoutOptions.End };
            container.Children.Add(label);
            StatisticsLabels.Add(e, label);
        }

        public async void OnAddExerciseToList()
        {
            await Shell.Current.GoToAsync("profileaddexercise");
        }
        public async void OnEditDeleteExercises()
        {
            await Shell.Current.GoToAsync("profileeditdeleteexercises");
        }
        public async void OnImportExportWorkouts()
        {
            if (Views.Settings.IsExport)
            {
                Helper_ExportData(AppState.Workouts, "DogoFit_workouts");
            }
            else
            {
                Helper_ImportWorkouts();
            }
        }
        public async void OnImportExportRoutines()
        {
            if (Views.Settings.IsExport)
            {
                Helper_ExportData(AppState.Routines, "DogoFit_routines");
            }
            else
            {
                Helper_ImportRoutines();
            }
        }
        public async void OnImportExportExercises()
        {
            if (Views.Settings.IsExport)
            {
                Helper_ExportData(AppState.AllExercises, "DogoFit_exercises");
            }
            else
            {
                Helper_ImportExercises();
            }
        }
        public async void OnImportExportAll()
        {
            if(Views.Settings.IsExport)
            {
                var exportData = new
                {
                    Routines = AppState.Routines,
                    Workouts = AppState.Workouts,
                    Exercises = AppState.AllExercises
                };

                try
                {
                    string json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });

                    string filePath = Path.Combine(FileSystem.AppDataDirectory, "DogoFit_all_data.json");
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
            else
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

                        Helper_ImportRoutines(importedData);
                        Helper_ImportWorkouts(importedData);
                        Helper_ImportExercises(importedData);

                        await Shell.Current.DisplayAlert("Success", "Data imported successfully!", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
                }
            }
        }

        private async void Helper_ExportData<T>(ObservableCollection<T> collection, string filename)
        {
                try
                {
                    string json = JsonSerializer.Serialize(collection, new JsonSerializerOptions { WriteIndented = true });

                    string filePath = Path.Combine(FileSystem.AppDataDirectory, filename + ".json");
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

        private async void Helper_ImportRoutines()
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
            
                    var importedData = await JsonSerializer.DeserializeAsync<IEnumerable<Routine>>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            IgnoreReadOnlyProperties = true,
                            IncludeFields = true,
                        });
                        foreach (var r in importedData ?? Enumerable.Empty<Routine>())
                        {
                            AppState.Routines.Add(r);
                            var template = DbHelper.ToDbRoutineTemplate(r);
                            App.Db.RoutineTemplates.Add(template);
                            App.Db.SaveChanges();
                        }
            
                    await Shell.Current.DisplayAlert("Success", "Data imported successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
            }
        }

        private void Helper_ImportRoutines(ExportDataType data)
        {
            if(data != null)
            {
                foreach (var r in data.Routines ?? Enumerable.Empty<Routine>())
                {
                    AppState.Routines.Add(r);
                    var template = DbHelper.ToDbRoutineTemplate(r);
                    App.Db.RoutineTemplates.Add(template);
                    App.Db.SaveChanges();
                }
            }
        }

        private void Helper_ImportWorkouts(ExportDataType data)
        {
            if(data != null)
            {
                foreach (var w in data.Workouts ?? Enumerable.Empty<Routine>())
                {
                    AppState.Workouts.Insert(0, w);
                    var workout = DbHelper.ToDbWorkout(w);
                    App.Db.Workouts.Add(workout);
                    App.Db.SaveChanges();
                
                    w.ID = workout.Id;
                }
            }
        }
        private async void Helper_ImportWorkouts()
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
            
                    var importedData = await JsonSerializer.DeserializeAsync<IEnumerable<Routine>>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            IgnoreReadOnlyProperties = true,
                            IncludeFields = true,
                        });
                    foreach (var w in importedData ?? Enumerable.Empty<Routine>())
                    {
                        AppState.Workouts.Insert(0, w);
                        var workout = DbHelper.ToDbWorkout(w);
                        App.Db.Workouts.Add(workout);
                        App.Db.SaveChanges();
                    
                        w.ID = workout.Id;
                    }
            
                    await Shell.Current.DisplayAlert("Success", "Data imported successfully!", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Import failed: {ex.Message}", "OK");
            }
        }

        private void Helper_ImportExercises(ExportDataType data)
        {
            if(data != null)
            {
                foreach (var e in data.Exercises ?? Enumerable.Empty<Exercise>())
                {
                    if (!AppState.AllExercises.Any(i => i.Name.Equals(e.Name, StringComparison.Ordinal)))
                    {
                        AppState.AllExercises.Add(e);
                        var exercise = DbHelper.ToDbExercise(e);
                        App.Db.Exercises.Add(exercise);
                        App.Db.SaveChanges();
                    }
                } 
            }
        }
        private async void Helper_ImportExercises()
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
            
                    var importedData = await JsonSerializer.DeserializeAsync<IEnumerable<Exercise>>(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            IgnoreReadOnlyProperties = true,
                            IncludeFields = true,
                        });
                    if(importedData != null)
                    {
                        foreach (var e in importedData ?? Enumerable.Empty<Exercise>())
                        {
                            if (!AppState.AllExercises.Any(i => i.Name.Equals(e.Name, StringComparison.Ordinal)))
                            {
                                AppState.AllExercises.Add(e);
                                var exercise = DbHelper.ToDbExercise(e);
                                App.Db.Exercises.Add(exercise);
                                App.Db.SaveChanges();
                            }
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

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

}
