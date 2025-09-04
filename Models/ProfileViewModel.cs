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
    public enum MuscleChoices
    {
        Empty, Biceps, Triceps, Forearms, Chest, Lats, Traps, ChestGroup, SideDelts, FrontDelts, 
        RearDelts, Quads, Hams, Glutes, Calves, Abs, Obliques, Push, Pull, LegsFunction, CoreFunction, 
        Arms, Back, Shoulders, LegsGroup, CoreGroup
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
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }
        public ICommand UnitSystemCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand ExercisesCommand { get; }
        public ICommand FilterCommand { get; }
        public String EnteredExerciseName { get; set; }
        public Exercise SelectedExercise => AppState.SelectedExercise;
        public string SelectedExerciseName => SelectedExercise?.Name;

        public List<(Button, MuscleChoices)> MusclesButtons { get; set; } = new List<(Button, MuscleChoices)>();
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
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }


        public ICommand MuscleGroupDistCommand { get; set; }
        public ICommand MuscleFunctionDistCommand { get; set; }
        public ICommand IndividualMuscleDistCommand { get; set; }
        public ICommand MainExercisesCommand { get; set; }
        public ICommand MonthlyReportCommand { get; set; }
        private MuscleChoices _mc;
        public MuscleChoices MC
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
            Profile = AppState.Profile;
            OpenSettingsCommand = new Command(OpenSettings);
            GitHubCommand = new Command(OpenGitHub);
            ExportCommand = new Command(ExportData);
            ImportCommand = new Command(ImportData);
            UnitSystemCommand = new Command(ToggleWeightUnit);
            ExercisesCommand = new Command(OnExercises);
            StatisticsCommand = new Command(OnStatistics);
            SelectExerciseCommand = new Command<Exercise>(OnSelectExercise);
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
            Categories = AppState.Categories;
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            FilterByCategory("All", false);
            FilterCommand = new Command<Category>(SelectCategory);
            Workouts = AppState.Workouts;
        }

        public async void OnSelectExercise(Exercise exercise)
        {
            AppState.SelectedExercise = new Exercise(exercise);
            OnPropertyChanged(nameof(SelectedExercise));
            AppState.profileStat = ProfileStats.Exercise;
            await Shell.Current.GoToAsync("profileexercisestats");
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
            var filteredWorkoutsLastMonth = AppState.Workouts
                .Where(w => w.StartTime >= now.AddMonths(-1))
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
            if (intensityDiff >= 0)
            {
                labels[E_StatisticsLabels.MRIntensityTrend].TextColor = Colors.Green;
                labels[E_StatisticsLabels.MRIntensityTrend].Text = "\u2191 " + Math.Abs(intensityDiff).ToString("F2");
            }
            else
            {
                labels[E_StatisticsLabels.MRIntensityTrend].TextColor = Colors.Red;
                labels[E_StatisticsLabels.MRIntensityTrend].Text = "\u2193 " + Math.Abs(intensityDiff).ToString("F2");
            }

            if (volumeDiff >= 0)
            {
                labels[E_StatisticsLabels.MRVolumeTrend].TextColor = Colors.Green;
                labels[E_StatisticsLabels.MRVolumeTrend].Text = "\u2191 " + Math.Abs(volumeDiff).ToString("F2");
            }
            else
            {
                labels[E_StatisticsLabels.MRVolumeTrend].TextColor = Colors.Red;
                labels[E_StatisticsLabels.MRVolumeTrend].Text = "\u2193 " + Math.Abs(volumeDiff).ToString("F2");
            }

            if (repsDiff >= 0)
            {
                labels[E_StatisticsLabels.MRRepsTrend].TextColor = Colors.Green;
                labels[E_StatisticsLabels.MRRepsTrend].Text = "\u2191 " + Math.Abs(repsDiff).ToString();
            }
            else
            {
                labels[E_StatisticsLabels.MRRepsTrend].TextColor = Colors.Red;
                labels[E_StatisticsLabels.MRRepsTrend].Text = "\u2193 " + Math.Abs(repsDiff).ToString();
            }

            if (setsDiff >= 0)
            {
                labels[E_StatisticsLabels.MRSetsTrend].TextColor = Colors.Green;
                labels[E_StatisticsLabels.MRSetsTrend].Text = "\u2191 " + Math.Abs(setsDiff).ToString();
            }
            else
            {
                labels[E_StatisticsLabels.MRSetsTrend].TextColor = Colors.Red;
                labels[E_StatisticsLabels.MRSetsTrend].Text = "\u2193 " + Math.Abs(setsDiff).ToString();
            }
            if (workoutsDiff >= 0)
            {
                labels[E_StatisticsLabels.MRWorkoutTrend].TextColor = Colors.Green;
                labels[E_StatisticsLabels.MRWorkoutTrend].Text = "\u2191 " + Math.Abs(workoutsDiff).ToString();
            }
            else
            {
                labels[E_StatisticsLabels.MRWorkoutTrend].TextColor = Colors.Red;
                labels[E_StatisticsLabels.MRWorkoutTrend].Text = "\u2193 " + Math.Abs(workoutsDiff).ToString();
            }
            var ts = TimeSpan.FromSeconds(Math.Abs(durationDiff));
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
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2191 " + FormatDuration(ts);
                else
                    labels[E_StatisticsLabels.MRDurationTrend].Text = "\u2191 " + FormatDuration(ts);
            }
        }

        private string FormatDuration(TimeSpan ts)
        {
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
                DisplayChoices.Volume => w => w.Volume,
                DisplayChoices.Intensity => w => { var reps = w.Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
                    return reps != 0 ? w.Volume / reps : 0;
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
            if (MC != MuscleChoices.Biceps && MC != MuscleChoices.Triceps && MC != MuscleChoices.Forearms && MC != MuscleChoices.Lats &&  MC != MuscleChoices.Traps &&
                MC != MuscleChoices.Chest && MC != MuscleChoices.SideDelts && MC != MuscleChoices.FrontDelts && MC != MuscleChoices.RearDelts && MC != MuscleChoices.Quads &&
                MC != MuscleChoices.Hams && MC != MuscleChoices.Glutes && MC != MuscleChoices.Calves && MC != MuscleChoices.Abs && MC != MuscleChoices.Obliques)
                return;
            if (IMChart == null)
                return;
            switch(MC)
            {
                case MuscleChoices.Abs:
                    IM_InfoLabel.Text = "Abdominals Current versus Last Month";
                    break;
                case MuscleChoices.SideDelts:
                    IM_InfoLabel.Text = "Lateral Delts Current versus Last Month";
                    break;
                case MuscleChoices.FrontDelts:
                    IM_InfoLabel.Text = "Front Delts Current versus Last Month";
                    break;
                case MuscleChoices.RearDelts:
                    IM_InfoLabel.Text = "Rear Delts Current versus Last Month";
                    break;
                case MuscleChoices.Hams:
                    IM_InfoLabel.Text = "Hamstrings Current versus Last Month";
                    break;
                case MuscleChoices.Quads:
                    IM_InfoLabel.Text = "Quadriceps Current versus Last Month";
                    break;
                default:
                    IM_InfoLabel.Text = MC.ToString() + " Current versus Last Month";
                    break;
            }

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

            string individualMuscle = MC switch
            {
                MuscleChoices.Biceps => "Biceps",
                MuscleChoices.Triceps => "Triceps",
                MuscleChoices.Forearms => "Forearms",
                MuscleChoices.Lats => "Lats",
                MuscleChoices.Traps => "Traps",
                MuscleChoices.SideDelts => "Lateral Delts",
                MuscleChoices.FrontDelts => "Front Delts",
                MuscleChoices.RearDelts => "Rear Delts",
                MuscleChoices.Quads => "Quadriceps",
                MuscleChoices.Hams => "Hamstrings",
                MuscleChoices.Glutes => "Glutes",
                MuscleChoices.Calves => "Calves",
                MuscleChoices.Abs => "Abdominals",
                MuscleChoices.Obliques => "Obliques",
                _ => null
            };

            if (individualMuscle == null) return;

            void UpdateLabelsForIndividualMuscle(string im)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.TargetMuscle == im)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.TargetMuscle == im)
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

                if (intensityDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.IMIntensityTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.IMIntensityTrend].Text = "\u2191 " + Math.Abs(intensityDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.IMIntensityTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.IMIntensityTrend].Text = "\u2193 " + Math.Abs(intensityDiff).ToString("F2");
                }

                if (volumeDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.IMVolumeTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.IMVolumeTrend].Text = "\u2191 " + Math.Abs(volumeDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.IMVolumeTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.IMVolumeTrend].Text = "\u2193 " + Math.Abs(volumeDiff).ToString("F2");
                }

                if (repsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.IMRepsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.IMRepsTrend].Text = "\u2191 " + Math.Abs(repsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.IMRepsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.IMRepsTrend].Text = "\u2193 " + Math.Abs(repsDiff).ToString();
                }

                if (setsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.IMSetsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.IMSetsTrend].Text = "\u2191 " + Math.Abs(setsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.IMSetsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.IMSetsTrend].Text = "\u2193 " + Math.Abs(setsDiff).ToString();
                }
            }
            UpdateLabelsForIndividualMuscle(individualMuscle);
        }
        private void RecalculateIMChart()
        {
            if (AppState.Workouts == null)
                return;
            if (MC == MuscleChoices.Empty || DC == DisplayChoices.Empty || TC == TimeChoices.Empty)
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
            var BicepsExercises = allExercises.Where(e => e.TargetMuscle == "Biceps").ToList();
            var TricepsExercises = allExercises.Where(e => e.TargetMuscle == "Triceps").ToList();
            var ForearmsExercises = allExercises.Where(e => e.TargetMuscle == "Forearms").ToList();
            var LatsExercises = allExercises.Where(e => e.TargetMuscle == "Lats").ToList();
            var TrapsExercises = allExercises.Where(e => e.TargetMuscle == "Traps").ToList();
            var ChestExercises = allExercises.Where(e => e.TargetMuscle == "Chest").ToList();
            var SideDeltsExercises = allExercises.Where(e => e.TargetMuscle == "Lateral Delts").ToList();
            var FrontDeltsExercises = allExercises.Where(e => e.TargetMuscle == "Front Delts").ToList();
            var RearDeltsExercises = allExercises.Where(e => e.TargetMuscle == "Rear Delts").ToList();
            var QuadricepsExercises = allExercises.Where(e => e.TargetMuscle == "Quadriceps").ToList();
            var HamstringsExercises = allExercises.Where(e => e.TargetMuscle == "Hamstrings").ToList();
            var GlutesExercises = allExercises.Where(e => e.TargetMuscle == "Glutes").ToList();
            var CalvesExercises = allExercises.Where(e => e.TargetMuscle == "Calves").ToList();
            var AbsExercises = allExercises.Where(e => e.TargetMuscle == "Abdominals").ToList();
            var ObliquesExercises = allExercises.Where(e => e.TargetMuscle == "Obliques").ToList();


            switch (DC)
            {
                case DisplayChoices.Sets:
                    {           
                        var muscleSets = new Dictionary<string, double>
                        {           
                            { "Biceps", BicepsExercises.SelectMany(s => s.CheckedSets).Count() },
                            { "Triceps", TricepsExercises.SelectMany(s => s.CheckedSets).Count()},
                            { "Forearms", ForearmsExercises.SelectMany(s => s.CheckedSets).Count()},
                            { "Lats", LatsExercises.SelectMany(s => s.CheckedSets).Count() },           
                            { "Traps", TrapsExercises.SelectMany(s => s.CheckedSets).Count()},
                            { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Count() },         
                            { "Side Delts", SideDeltsExercises.SelectMany(s => s.CheckedSets).Count()},
                            { "Front Delts", FrontDeltsExercises.SelectMany(s => s.CheckedSets).Count()},
                            { "Rear Delts", RearDeltsExercises.SelectMany(s => s.CheckedSets).Count() },            
                            { "Quadriceps", QuadricepsExercises.SelectMany(s => s.CheckedSets).Count() },
                            { "Hamstrings", HamstringsExercises.SelectMany(s => s.CheckedSets).Count() },           
                            { "Glutes", GlutesExercises.SelectMany(s => s.CheckedSets).Count() },           
                            { "Calves", CalvesExercises.SelectMany(s => s.CheckedSets).Count() },           
                            { "Abs", AbsExercises.SelectMany(s => s.CheckedSets).Count() },         
                            { "Obliques", ObliquesExercises.SelectMany(s => s.CheckedSets).Count() }
                        };          
                                    
                        var max = muscleSets.Values.Max();
                        IMChart.Series = muscleSets.Where(kv => kv.Value > 0).Select(kv => new PieSeries<double>
                        {           
                            Name = kv.Key,
                            Values = new[]           { kv.Value },
                            Pushout = kv.Value == max ? 30 : 0
                        }).ToArray();           
                        break;          
                    }           
                                
                    case DisplayChoices.Reps:
                    {           
                        var muscleReps = new Dictionary<string, double>
                        {           
                            { "Biceps", BicepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Triceps", TricepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps)},
                            { "Forearms", ForearmsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps)},
                            { "Lats", LatsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },          
                            { "Traps", TrapsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps)},
                            { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },            
                            { "Side Delts", SideDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Front Delts", FrontDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps)},
                            { "Rear Delts", RearDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },           
                            { "Quadriceps", QuadricepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Hamstrings", HamstringsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },          
                            { "Glutes", GlutesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },          
                            { "Calves", CalvesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },          
                            { "Abs", AbsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },            
                            { "Obliques", ObliquesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) }
                        };          
                                    
                        var max = muscleReps.Values.Max();
                        IMChart.Series = muscleReps.Where(kv => kv.Value > 0).Select(kv => new PieSeries<double>
                        {           
                            Name = kv.Key,
                            Values = new[] { kv.Value },
                            Pushout = kv.Value == max ? 30 : 0
                        }).ToArray();           
                        break;          
                    }           
                                
                    case DisplayChoices.Volume:
                    {           
                        var muscleVolume = new Dictionary<string, double>
                        {           
                            { "Biceps", BicepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Triceps", TricepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight)},
                            { "Forearms", ForearmsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight)},
                            { "Lats", LatsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },           
                            { "Traps", TrapsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight)},
                            { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },         
                            { "Side Delts", SideDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Front Delts", FrontDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight)},
                            { "Rear Delts", RearDeltsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },            
                            { "Quadriceps", QuadricepsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Hamstrings", HamstringsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Glutes", GlutesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },           
                            { "Calves", CalvesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },           
                            { "Abs", AbsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },         
                            { "Obliques", ObliquesExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) }
                        };          
                                    
                        var max = muscleVolume.Values.Max();
                        IMChart.Series = muscleVolume.Where(kv => kv.Value > 0).Select (kv => new PieSeries<double>
                        {           
                            Name = kv.Key,
                            Values = new[] { kv.Value },
                            Pushout = kv.Value == max ? 30 : 0
                        }).ToArray();           
                        break;          
                    }           
                                
                    case DisplayChoices.Intensity:
                    {           
                                    
                        var muscleIntensity = new Dictionary<string, double>
                        {           
                            { "Biceps", GetIntensity(BicepsExercises)},         
                                    
                            { "Triceps", GetIntensity(TricepsExercises) },            
                            
                            { "Forearms", GetIntensity(ForearmsExercises) },           
                            
                            { "Lats", GetIntensity(LatsExercises) },           
                            
                            { "Traps", GetIntensity(TrapsExercises) },          
                            
                            { "Chest", GetIntensity(ChestExercises) },          
                            
                            { "Side Delts", GetIntensity(SideDeltsExercises) },          
                            
                            { "Front Delts", GetIntensity(FrontDeltsExercises)},         
                            
                            { "Rear Delts", GetIntensity(RearDeltsExercises) },          
                            
                            { "Quadriceps", GetIntensity(QuadricepsExercises)},         
                            
                            { "Hamstrings", GetIntensity(HamstringsExercises) },         
                            
                            { "Glutes", GetIntensity(GlutesExercises)},         
                            
                            { "Calves", GetIntensity(CalvesExercises) },         
                            
                            { "Abs", GetIntensity(AbsExercises) },            
                            
                            { "Obliques", GetIntensity(ObliquesExercises) }            
                        };          
                                    
                        var max = muscleIntensity.Values.Max();
                        IMChart.Series = muscleIntensity.Where(kv => kv.Value > 0).Select(kv => new PieSeries<double>
                        {           
                            Name = kv.Key,
                            Values = new[] { kv.Value },
                            Pushout = kv.Value == max ? 30 : 0
                        }).ToArray();
                        break;          
                    }           
            }           


        }

        
        public void UpdateMuscleFunctionChoices()
        {
            if (MC != MuscleChoices.LegsFunction && MC != MuscleChoices.CoreFunction && MC != MuscleChoices.Pull && MC != MuscleChoices.Push)
                return;

            if (MFChart == null)
                return;
            switch(MC)
            {
                case MuscleChoices.CoreFunction:
                    MF_InfoLabel.Text = "Core Current versus Last Month";
                    break;
                case MuscleChoices.LegsFunction:
                    MF_InfoLabel.Text = "Legs Current versus Last Month";
                    break;
                default:
                    MF_InfoLabel.Text = MC.ToString() + " Current versus Last Month";
                    break;
            }

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

            string muscleFunction = MC switch
            {
                MuscleChoices.Push => "Push",
                MuscleChoices.Pull => "Pull",
                MuscleChoices.CoreFunction => "Core",
                MuscleChoices.LegsFunction => "Legs",
                _ => null
            };

            if (muscleFunction == null) return;

            void UpdateLabelsForMuscleFunction(string mf)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.Function == mf)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.Function == mf)
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
                if (intensityDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MFIntensityTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MFIntensityTrend].Text = "\u2191 " + Math.Abs(intensityDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MFIntensityTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MFIntensityTrend].Text = "\u2193 " + Math.Abs(intensityDiff).ToString("F2");
                }

                if (volumeDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MFVolumeTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MFVolumeTrend].Text = "\u2191 " + Math.Abs(volumeDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MFVolumeTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MFVolumeTrend].Text = "\u2193 " + Math.Abs(volumeDiff).ToString("F2");
                }

                if (repsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MFRepsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MFRepsTrend].Text = "\u2191 " + Math.Abs(repsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MFRepsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MFRepsTrend].Text = "\u2193 " + Math.Abs(repsDiff).ToString();
                }

                if (setsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MFSetsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MFSetsTrend].Text = "\u2191 " + Math.Abs(setsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MFSetsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MFSetsTrend].Text = "\u2193 " + Math.Abs(setsDiff).ToString();
                }
            }

            UpdateLabelsForMuscleFunction(muscleFunction);
        }
        private void RecalculateMFChart()
        {
            if (AppState.Workouts == null)
                return;
            if (TC == TimeChoices.Empty || MC == MuscleChoices.Empty || DC == DisplayChoices.Empty)
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
            var PushExercises = allExercises.Where(e => e.Function == "Push").ToList();
            var PullExercises = allExercises.Where(e => e.Function == "Pull").ToList();
            var LegsExercises = allExercises.Where(e => e.Function == "Legs").ToList();
            var CoreExercises = allExercises.Where(e => e.Function == "Core").ToList();

            switch (DC)
            {
                case DisplayChoices.Sets:
                    {
                        var setsDict = new Dictionary<string, double>
                        {
                            { "Push", PushExercises.SelectMany(s => s.CheckedSets).Count() },
                            { "Pull", PullExercises.SelectMany(s => s.CheckedSets).Count() },
                            { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Count() },
                            { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Count() }
                        };

                        var max = setsDict.Values.Max();
                        MFChart.Series = setsDict
                            .Where(kv => kv.Value > 0)
                            .Select(kv => new PieSeries<double>
                            {
                                Name = kv.Key,
                                Values = new[] { kv.Value },
                                Pushout = kv.Value == max ? 30 : 0
                            })
                            .ToArray();
                        break;
                    }

                case DisplayChoices.Reps:
                    {
                        var repsDict = new Dictionary<string, double>
                        {
                            { "Push", PushExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Pull", PullExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                            { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) }
                        };

                        var max = repsDict.Values.Max();
                        MFChart.Series = repsDict
                            .Where(kv => kv.Value > 0)
                            .Select(kv => new PieSeries<double>
                            {
                                Name = kv.Key,
                                Values = new[] { kv.Value },
                                Pushout = kv.Value == max ? 30 : 0
                            })
                            .ToArray();
                        break;
                    }

                case DisplayChoices.Volume:
                    {
                        var volumeDict = new Dictionary<string, double>
                        {
                            { "Push", PushExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Pull", PullExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                            { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) }
                        };

                        var max = volumeDict.Values.Max();
                        MFChart.Series = volumeDict
                            .Where(kv => kv.Value > 0)
                            .Select(kv => new PieSeries<double>
                            {
                                Name = kv.Key,
                                Values = new[] { kv.Value },
                                Pushout = kv.Value == max ? 30 : 0
                            })
                            .ToArray();
                        break;
                    }

                case DisplayChoices.Intensity:
                    {

                        var intensityDict = new Dictionary<string, double>
                        {
                            { "Push", GetIntensity(PushExercises) },

                            { "Pull", GetIntensity(PullExercises) },

                            { "Legs", GetIntensity(LegsExercises) },

                            { "Core", GetIntensity(CoreExercises) }
                        };

                        var max = intensityDict.Values.Max();
                        MFChart.Series = intensityDict
                            .Where(kv => kv.Value > 0)
                            .Select(kv => new PieSeries<double>
                            {
                                Name = kv.Key,
                                Values = new[] { kv.Value },
                                Pushout = kv.Value == max ? 30 : 0
                            })
                            .ToArray();
                        break;
                    }
            }

        }

        public void UpdateMuscleGroupChoices()
        {
            if (MC != MuscleChoices.Arms && MC != MuscleChoices.LegsGroup && MC != MuscleChoices.ChestGroup
                && MC != MuscleChoices.Back && MC != MuscleChoices.Shoulders && MC != MuscleChoices.CoreGroup)
                return;

            if (MGChart == null)
                return;

            switch(MC)
            {
                case MuscleChoices.CoreGroup:
                    MG_InfoLabel.Text = "Core Current versus Last Month";
                    break;
                case MuscleChoices.LegsGroup:
                    MG_InfoLabel.Text = "Legs Current versus Last Month";
                    break;
                case MuscleChoices.ChestGroup:
                    MG_InfoLabel.Text = "Chest Current versus Last Month";
                    break;
                default:
                    MG_InfoLabel.Text = MC.ToString() + " Current versus Last Month";
                    break;
            }

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

            string muscleGroup = MC switch
            {
                MuscleChoices.Arms => "Arms",
                MuscleChoices.LegsGroup => "Legs",
                MuscleChoices.CoreGroup => "Core",
                MuscleChoices.ChestGroup => "Chest",
                MuscleChoices.Back => "Back",
                MuscleChoices.Shoulders => "Shoulders",
                _ => null
            };

            if (muscleGroup == null) return;

            void UpdateLabelsForMuscleGroup(string mg)
            {
                var exercisesNow = filteredWorkoutsNow.SelectMany(w => w.Exercises)
                                                      .Where(e => e.MuscleGroup == mg)
                                                      .ToList();
                var exercisesLastMonth = filteredWorkoutsLastMonth.SelectMany(w => w.Exercises)
                                                                  .Where(e => e.MuscleGroup == mg)
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
                if (intensityDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MGIntensityTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MGIntensityTrend].Text = "\u2191 " + Math.Abs(intensityDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MGIntensityTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MGIntensityTrend].Text = "\u2193 " + Math.Abs(intensityDiff).ToString("F2");
                }

                if (volumeDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MGVolumeTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MGVolumeTrend].Text = "\u2191 " + Math.Abs(volumeDiff).ToString("F2");
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MGVolumeTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MGVolumeTrend].Text = "\u2193 " + Math.Abs(volumeDiff).ToString("F2");
                }

                if (repsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MGRepsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MGRepsTrend].Text = "\u2191 " + Math.Abs(repsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MGRepsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MGRepsTrend].Text = "\u2193 " + Math.Abs(repsDiff).ToString();
                }

                if (setsDiff >= 0)
                {
                    StatisticsLabels[E_StatisticsLabels.MGSetsTrend].TextColor = Colors.Green;
                    StatisticsLabels[E_StatisticsLabels.MGSetsTrend].Text = "\u2191 " + Math.Abs(setsDiff).ToString();
                }
                else
                {
                    StatisticsLabels[E_StatisticsLabels.MGSetsTrend].TextColor = Colors.Red;
                    StatisticsLabels[E_StatisticsLabels.MGSetsTrend].Text = "\u2193 " + Math.Abs(setsDiff).ToString();
                }
            }

            UpdateLabelsForMuscleGroup(muscleGroup);
        }

        private void RecalculateMGChart()
        {
            if (AppState.Workouts == null)
                return;
            if (TC == TimeChoices.Empty || MC == MuscleChoices.Empty || DC == DisplayChoices.Empty)
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
            var ArmsExercises = allExercises.Where(e => e.MuscleGroup == "Arms").ToList();
            var LegsExercises = allExercises.Where(e => e.MuscleGroup == "Legs").ToList();
            var CoreExercises = allExercises.Where(e => e.MuscleGroup == "Core").ToList();
            var ChestExercises = allExercises.Where(e => e.MuscleGroup == "Chest").ToList();
            var BackExercises = allExercises.Where(e => e.MuscleGroup == "Back").ToList();
            var ShouldersExercises = allExercises.Where(e => e.MuscleGroup == "Shoulders").ToList();

            switch (DC)
            {
                case DisplayChoices.Sets:
                    var muscleSets = new Dictionary<string, int>
                    {
                        { "Arms", ArmsExercises.SelectMany(s => s.CheckedSets).Count() },
                        { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Count() },
                        { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Count() },
                        { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Count() },
                        { "Back", BackExercises.SelectMany(s => s.CheckedSets).Count() },
                        { "Shoulders", ShouldersExercises.SelectMany(s => s.CheckedSets).Count() }
                    };

                    var maxSets = muscleSets.Values.Max();
                    MGChart.Series = muscleSets
                        .Where(kv => kv.Value > 0)
                        .Select(kv => new PieSeries<double>
                        {
                            Name = kv.Key,
                            Values = new[] { (double)kv.Value },
                            Pushout = kv.Value == maxSets ? 30 : 0
                        })
                        .ToArray();
                    break;

                case DisplayChoices.Reps:
                    var muscleReps = new Dictionary<string, int>
                    {
                        { "Arms", ArmsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                        { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                        { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                        { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                        { "Back", BackExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) },
                        { "Shoulders", ShouldersExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps) }
                    };

                    var maxReps = muscleReps.Values.Max();
                    MGChart.Series = muscleReps
                        .Where(kv => kv.Value > 0)
                        .Select(kv => new PieSeries<double>
                        {
                            Name = kv.Key,
                            Values = new[] { (double)kv.Value },
                            Pushout = kv.Value == maxReps ? 30 : 0
                        })
                        .ToArray();
                    break;

                case DisplayChoices.Volume:
                    var muscleVolume = new Dictionary<string, double>
                    {
                        { "Arms", ArmsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                        { "Legs", LegsExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                        { "Core", CoreExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                        { "Chest", ChestExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                        { "Back", BackExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) },
                        { "Shoulders", ShouldersExercises.SelectMany(s => s.CheckedSets).Sum(s => s.Reps * s.Weight) }
                    };

                    var maxVolume = muscleVolume.Values.Max();
                    MGChart.Series = muscleVolume
                        .Where(kv => kv.Value > 0)
                        .Select(kv => new PieSeries<double>
                        {
                            Name = kv.Key,
                            Values = new[] { (double)kv.Value },
                            Pushout = kv.Value == maxVolume ? 30 : 0
                        })
                        .ToArray();
                    break;

                case DisplayChoices.Intensity:
                    var muscleIntensity = new Dictionary<string, double>
                    {
                        { "Arms", GetIntensity(ArmsExercises) },
                        { "Legs", GetIntensity(LegsExercises) },
                        { "Core", GetIntensity(CoreExercises) },
                        { "Chest", GetIntensity(ChestExercises) },
                        { "Back", GetIntensity(BackExercises) },
                        { "Shoulders", GetIntensity(ShouldersExercises) }
                    };

                    var maxIntensity = muscleIntensity.Values.Max();
                    MGChart.Series = muscleIntensity
                        .Where(kv => kv.Value > 0)
                        .Select(kv => new PieSeries<double>
                        {
                            Name = kv.Key,
                            Values = new[] { kv.Value },
                            Pushout = kv.Value == maxIntensity ? 30 : 0
                        })
                        .ToArray();
                    break;
            }


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
                    Volume = g.Sum(w => w.Volume),
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
                            AppState.Profile.ChestGroup += importedData.Profile.ChestGroup;
                            AppState.Profile.Back += importedData.Profile.Back;
                            AppState.Profile.LegsFunction += importedData.Profile.LegsFunction;
                            AppState.Profile.LegsGroup += importedData.Profile.LegsGroup;
                            AppState.Profile.Arms += importedData.Profile.Arms;
                            AppState.Profile.CoreFunction += importedData.Profile.CoreFunction;
                            AppState.Profile.CoreGroup += importedData.Profile.CoreGroup;
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
