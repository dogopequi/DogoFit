using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;

namespace GymTracker.Services
{
    public class Routine : INotifyPropertyChanged
    {
        private IDispatcherTimer? _timer;
        private int _volume;
        private int _setCount;
        private int _repCount;

        // muscle split
        private int _arms;
        private int _shoulders;
        private int _chest;
        private int _back;
        private int _legs;
        private int _core;

        private bool _isSplitsVisible;
        public bool IsSplitsVisible
        {
            get => _isSplitsVisible;
            set { _isSplitsVisible = value; OnPropertyChanged(nameof(IsSplitsVisible)); }
        }

        private string _splitsButtonText = "Show Splits";
        public string SplitsButtonText
        {
            get => _splitsButtonText;
            set { _splitsButtonText = value; OnPropertyChanged(nameof(SplitsButtonText)); }
        }
        private bool _isExercisesVisible;
        public bool IsExercisesVisible
        {
            get => _isExercisesVisible;
            set { _isExercisesVisible = value; OnPropertyChanged(nameof(IsExercisesVisible)); }
        }

        private string _exercisesButtonText = "Show Exercises";
        public string ExercisesButtonText
        {
            get => _exercisesButtonText;
            set { _exercisesButtonText = value; OnPropertyChanged(nameof(ExercisesButtonText)); }
        }

        [JsonIgnore]
        public ICommand ToggleSplitsCommand { get; }
        [JsonIgnore]
        public ICommand ToggleExercisesCommand { get; }
        [JsonInclude]
        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => IsRunning ? DateTime.Now - StartTime : _duration;
            set => _duration = value;
        }

        [JsonIgnore]
        public int? ID { get; set; }

        public int Arms
        {
            get => _arms;
            private set
            {
                if (_arms != value)
                {
                    _arms = value;
                    OnPropertyChanged(nameof(Arms));
                    OnPropertyChanged(nameof(ArmsPercentage));
                }
            }
        }

        public int Shoulders
        {
            get => _shoulders;
            private set
            {
                if (_shoulders != value)
                {
                    _shoulders = value;
                    OnPropertyChanged(nameof(Shoulders));
                    OnPropertyChanged(nameof(ShouldersPercentage));
                }
            }
        }

        public int Chest
        {
            get => _chest;
            private set
            {
                if (_chest != value)
                {
                    _chest = value;
                    OnPropertyChanged(nameof(Chest));
                    OnPropertyChanged(nameof(ChestPercentage));
                }
            }
        }

        public int Back
        {
            get => _back;
            private set
            {
                if (_back != value)
                {
                    _back = value;
                    OnPropertyChanged(nameof(Back));
                    OnPropertyChanged(nameof(BackPercentage));
                }
            }
        }

        public int Legs
        {
            get => _legs;
            private set
            {
                if (_legs != value)
                {
                    _legs = value;
                    OnPropertyChanged(nameof(Legs));
                    OnPropertyChanged(nameof(LegsPercentage));
                }
            }
        }

        public int Core
        {
            get => _core;
            private set
            {
                if (_core != value)
                {
                    _core = value;
                    OnPropertyChanged(nameof(Core));
                    OnPropertyChanged(nameof(CorePercentage));
                }
            }
        }

        public int Volume
        {
            get => _volume;
            private set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public int SetCount
        {
            get => _setCount;
            private set
            {
                if (_setCount != value)
                {
                    _setCount = value;
                    OnPropertyChanged(nameof(SetCount));
                    OnPropertyChanged(nameof(LatsPercentage));
                    OnPropertyChanged(nameof(TricepsPercentage));
                    OnPropertyChanged(nameof(BicepsPercentage));
                    OnPropertyChanged(nameof(QuadsPercentage));
                    OnPropertyChanged(nameof(HamsPercentage));
                    OnPropertyChanged(nameof(GlutesPercentage));
                    OnPropertyChanged(nameof(CalvesPercentage));
                    OnPropertyChanged(nameof(AbsPercentage));
                    OnPropertyChanged(nameof(ObliquesPercentage));
                    OnPropertyChanged(nameof(FDPercentage));
                    OnPropertyChanged(nameof(RDPercentage));
                    OnPropertyChanged(nameof(LDPercentage));
                    OnPropertyChanged(nameof(TrapsPercentage));
                    OnPropertyChanged(nameof(ForearmsPercentage));
                    OnPropertyChanged(nameof(PushPercentage));
                    OnPropertyChanged(nameof(ChestPercentage));
                    OnPropertyChanged(nameof(BackPercentage));
                    OnPropertyChanged(nameof(LegsPercentage));
                    OnPropertyChanged(nameof(ArmsPercentage));
                    OnPropertyChanged(nameof(CorePercentage));
                    OnPropertyChanged(nameof(ShouldersPercentage));
                    OnPropertyChanged(nameof(PullPercentage));
                }
            }
        }

        public int RepCount
        {
            get => _repCount;
            private set
            {
                if (_repCount != value)
                {
                    _repCount = value;
                    OnPropertyChanged(nameof(RepCount));
                }
            }
        }

        private int _pull;
        private int _push;

        public int Pull
        {
            get => _pull;
            private set
            {
                if (_pull != value)
                {
                    _pull = value;
                    OnPropertyChanged(nameof(Pull));
                    OnPropertyChanged(nameof(PullPercentage));
                }
            }
        }

        public int Push
        {
            get => _push;
            private set
            {
                if (_push != value)
                {
                    _push = value;
                    OnPropertyChanged(nameof(Push));
                    OnPropertyChanged(nameof(PushPercentage));
                }
            }
        }

        private int _triceps;
        public int Triceps
        {
            get => _triceps;
            private set
            {
                if (_triceps != value)
                {
                    _triceps = value;
                    OnPropertyChanged(nameof(Triceps));
                    OnPropertyChanged(nameof(TricepsPercentage));
                }
            }
        }

        private int _biceps;
        public int Biceps
        {
            get => _biceps;
            private set
            {
                if (_biceps != value)
                {
                    _biceps = value;
                    OnPropertyChanged(nameof(Biceps));
                    OnPropertyChanged(nameof(BicepsPercentage));
                }
            }
        }

        private int _quadriceps;
        public int Quadriceps
        {
            get => _quadriceps;
            private set
            {
                if (_quadriceps != value)
                {
                    _quadriceps = value;
                    OnPropertyChanged(nameof(Quadriceps));
                    OnPropertyChanged(nameof(QuadsPercentage));
                }
            }
        }

        private int _hamstrings;
        public int Hamstrings
        {
            get => _hamstrings;
            private set
            {
                if (_hamstrings != value)
                {
                    _hamstrings = value;
                    OnPropertyChanged(nameof(Hamstrings));
                    OnPropertyChanged(nameof(HamsPercentage));
                }
            }
        }

        private int _glutes;
        public int Glutes
        {
            get => _glutes;
            private set
            {
                if (_glutes != value)
                {
                    _glutes = value;
                    OnPropertyChanged(nameof(Glutes));
                    OnPropertyChanged(nameof(GlutesPercentage));
                }
            }
        }

        private int _calves;
        public int Calves
        {
            get => _calves;
            private set
            {
                if (_calves != value)
                {
                    _calves = value;
                    OnPropertyChanged(nameof(Calves));
                    OnPropertyChanged(nameof(CalvesPercentage));
                }
            }
        }

        private int _abdominals;
        public int Abdominals
        {
            get => _abdominals;
            private set
            {
                if (_abdominals != value)
                {
                    _abdominals = value;
                    OnPropertyChanged(nameof(Abdominals));
                    OnPropertyChanged(nameof(AbsPercentage));
                }
            }
        }

        private int _obliques;
        public int Obliques
        {
            get => _obliques;
            private set
            {
                if (_obliques != value)
                {
                    _obliques = value;
                    OnPropertyChanged(nameof(Obliques));
                    OnPropertyChanged(nameof(ObliquesPercentage));
                }
            }
        }

        private int _traps;
        public int Traps
        {
            get => _traps;
            private set
            {
                if (_traps != value)
                {
                    _traps = value;
                    OnPropertyChanged(nameof(Traps));
                    OnPropertyChanged(nameof(TrapsPercentage));
                }
            }
        }

        private int _lateralDelts;
        public int LateralDelts
        {
            get => _lateralDelts;
            private set
            {
                if (_lateralDelts != value)
                {
                    _lateralDelts = value;
                    OnPropertyChanged(nameof(LateralDelts));
                    OnPropertyChanged(nameof(LDPercentage));
                }
            }
        }

        private int _frontDelts;
        public int FrontDelts
        {
            get => _frontDelts;
            private set
            {
                if (_frontDelts != value)
                {
                    _frontDelts = value;
                    OnPropertyChanged(nameof(FrontDelts));
                    OnPropertyChanged(nameof(FDPercentage));
                }
            }
        }

        private int _rearDelts;
        public int RearDelts
        {
            get => _rearDelts;
            private set
            {
                if (_rearDelts != value)
                {
                    _rearDelts = value;
                    OnPropertyChanged(nameof(RearDelts));
                    OnPropertyChanged(nameof(RDPercentage));
                }
            }
        }
        private int _formarms;
        public int Forearms
        {
            get => _formarms;
            private set
            {
                if (_formarms != value)
                {
                    _formarms = value;
                    OnPropertyChanged(nameof(Forearms));
                    OnPropertyChanged(nameof(ForearmsPercentage));
                }
            }
        }

        private int _lats;
        public int Lats
        {
            get => _lats;
            private set
            {
                if (_lats != value)
                {
                    _lats = value;
                    OnPropertyChanged(nameof(Lats));
                    OnPropertyChanged(nameof(LatsPercentage));
                }
            }
        }
        public void CalculateSetDistribution()
        {
            foreach (var exercise in Exercises)
            {
                if (!string.IsNullOrEmpty(exercise.Function))
                {
                    switch (exercise.Function)
                    {
                        case "Pull":
                            Pull += exercise.SetCountChecked;
                            break;
                        case "Push":
                            Push += exercise.SetCountChecked;
                            break;
                        case "Legs":
                            Legs += exercise.SetCountChecked;
                            break;
                        case "Core":
                            Core += exercise.SetCountChecked;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(exercise.MuscleGroup))
                {
                    switch (exercise.MuscleGroup)
                    {
                        case "Arms":
                            Arms += exercise.SetCountChecked;
                            break;
                        case "Shoulders":
                            Shoulders += exercise.SetCountChecked;
                            break;
                        case "Back":
                            Back += exercise.SetCountChecked;
                            break;
                        case "Chest":
                            Chest += exercise.SetCountChecked;
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(exercise.TargetMuscle))
                {
                    switch (exercise.TargetMuscle)
                    {
                        case "Biceps":
                            Biceps += exercise.SetCountChecked;
                            break;
                        case "Triceps":
                            Triceps += exercise.SetCountChecked;
                            break;
                        case "Quadriceps":
                            Quadriceps += exercise.SetCountChecked;
                            break;
                        case "Hamstrings":
                            Hamstrings += exercise.SetCountChecked;
                            break;
                        case "Glutes":
                            Glutes += exercise.SetCountChecked;
                            break;
                        case "Calves":
                            Calves += exercise.SetCountChecked;
                            break;
                        case "Abdominals":
                            Abdominals += exercise.SetCountChecked;
                            break;
                        case "Obliques":
                            Obliques += exercise.SetCountChecked;
                            break;
                        case "Traps":
                            Traps += exercise.SetCountChecked;
                            break;
                        case "Lateral Delts":
                            LateralDelts += exercise.SetCountChecked;
                            break;
                        case "Rear Delts":
                            RearDelts += exercise.SetCountChecked;
                            break;
                        case "Front Delts":
                            FrontDelts += exercise.SetCountChecked;
                            break;
                        case "Forearm":
                            Forearms += exercise.SetCountChecked;
                            break;
                        case "Lats":
                            Forearms += exercise.SetCountChecked;
                            break;
                    }
                }
            }
        }


        [JsonIgnore]
        public string DurationString
        {
            get
            {
                if (IsRunning)
                    return (DateTime.Now - StartTime).ToString(@"hh\:mm\:ss");
                else
                    return Duration.ToString(@"hh\:mm\:ss");
            }
        }



        [JsonIgnore]
        public bool IsRunning => EndTime == null;
        [JsonInclude]
        public DateTime StartTime { get; set; }

        [JsonInclude]
        public DateTime? EndTime { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        private ObservableCollection<Exercise> _exercises = new ObservableCollection<Exercise>();

        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                if (_exercises != null)
                    UnsubscribeExerciseEvents(_exercises);

                _exercises = value;
                SubscribeExerciseEvents(_exercises);
                RecalculateSetCountAndVolume();
            }
        }


        public Routine()
        {
            SubscribeExerciseEvents(_exercises);
            ToggleSplitsCommand = new Command(() =>
            {
                IsSplitsVisible = !IsSplitsVisible;
                SplitsButtonText = IsSplitsVisible ? "Hide Splits" : "Show Splits";
            });
            ToggleExercisesCommand = new Command(() =>
            {
                IsExercisesVisible = !IsExercisesVisible;
                ExercisesButtonText = IsExercisesVisible ? "Hide Exercises" : "Show Exercises";
            });
        }

        public Routine(Routine other)
        {
            Name = other.Name;
            Description = other.Description;
            StartTime = other.StartTime;
            EndTime = other.EndTime;
            Duration = other.Duration;
            Volume = other.Volume;
            SetCount = other.SetCount;
            RepCount = other.RepCount;
            Duration = other.Duration;
            Volume = other.Volume;
            Lats = other.Lats;
            Triceps = other.Triceps;
            Biceps = other.Biceps;
            Quadriceps = other.Quadriceps;
            Hamstrings = other.Hamstrings;
            Glutes = other.Glutes;
            Calves = other.Calves;
            Abdominals = other.Abdominals;
            Obliques = other.Obliques;
            Traps = other.Traps;
            LateralDelts = other.LateralDelts;
            FrontDelts = other.FrontDelts;
            RearDelts = other.RearDelts;
            Forearms = other.Forearms;
            Push = other.Push;
            Chest = other.Chest;
            Back = other.Back;
            Legs = other.Legs;
            Arms = other.Arms;
            Core = other.Core;
            Shoulders = other.Shoulders;
            Pull = other.Pull;
            

            Exercises = new ObservableCollection<Exercise>(
                other.Exercises.Select(ex => new Exercise(ex))
            );

            SubscribeExerciseEvents(Exercises);
            RecalculateSetCountAndVolume();
            ToggleSplitsCommand = new Command(() =>
            {
                IsSplitsVisible = !IsSplitsVisible;
                SplitsButtonText = IsSplitsVisible ? "Hide Splits" : "Show Splits";
            });
            ToggleExercisesCommand = new Command(() =>
            {
                IsExercisesVisible = !IsExercisesVisible;
                ExercisesButtonText = IsExercisesVisible ? "Hide Exercises" : "Show Exercises";
            });
        }

        private void SubscribeExerciseEvents(IEnumerable<Exercise> exercises)
        {
            foreach (var ex in exercises)
            {
                ex.Sets.CollectionChanged += OnSetsChanged;
                foreach (var set in ex.Sets)
                {
                    set.PropertyChanged += OnSetPropertyChanged;
                }
            }

            if (exercises is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += OnExercisesCollectionChanged;
            }
        }

        private void UnsubscribeExerciseEvents(IEnumerable<Exercise> exercises)
        {
            foreach (var ex in exercises)
            {
                ex.Sets.CollectionChanged -= OnSetsChanged;
                foreach (var set in ex.Sets)
                {
                    set.PropertyChanged -= OnSetPropertyChanged;
                }
            }

            if (exercises is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged -= OnExercisesCollectionChanged;
            }
        }

        private void OnExercisesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Exercise ex in e.NewItems)
                    SubscribeExerciseEvents(new[] { ex });

            if (e.OldItems != null)
                foreach (Exercise ex in e.OldItems)
                    UnsubscribeExerciseEvents(new[] { ex });

            RecalculateSetCountAndVolume();
        }

        private void OnSetsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Set s in e.NewItems)
                    s.PropertyChanged += OnSetPropertyChanged;

            if (e.OldItems != null)
                foreach (Set s in e.OldItems)
                    s.PropertyChanged -= OnSetPropertyChanged;

            RecalculateSetCountAndVolume();
        }

        private void OnSetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RecalculateSetCountAndVolume();
        }

        private void RecalculateSetCountAndVolume()
        {
            SetCount = Exercises.Sum(ex => ex.Sets.Count(s => s.IsChecked));
            Volume = Exercises.Sum(ex => ex.Sets.Where(s => s.IsChecked).Sum(s => s.Reps * s.Weight));
            RepCount = Exercises.Sum(ex => ex.Sets.Where(s => s.IsChecked).Sum(s => s.Reps));
        }

        public void AddExercise(Exercise exercise)
        {
            Exercises.Add(exercise);
        }

        public void RemoveExercise(Exercise exercise)
        {
            Exercises.Remove(exercise);
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            Duration = TimeSpan.Zero;
            EndTime = null;

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                if (EndTime == null)
                {
                    Duration = DateTime.Now - StartTime;
                    OnPropertyChanged(nameof(DurationString));
                }
            };
            _timer.Start();
        }
        private int _tickCounter = 0;
        public void Resume()
        {
            if (StartTime == default || EndTime != null)
                return;

            _timer = Application.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                if (EndTime == null)
                {
                    Duration = DateTime.Now - StartTime;
                    OnPropertyChanged(nameof(DurationString));

                    _tickCounter++;
                    if (_tickCounter >= 30)
                    {
                        _tickCounter = 0;
                        AppState.SaveWorkoutInProgress();
                    }
                }
            };
            _timer.Start();
        }
        public void Finish()
        {
            EndTime = DateTime.Now;
            Duration = EndTime.Value - StartTime;
            OnPropertyChanged(nameof(DurationString));
            _timer?.Stop();
        }

        public void UpdateDuration()
        {
            if (IsRunning)
            {
                OnPropertyChanged(nameof(DurationString));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));



        public double LatsPercentage => SetCount == 0 ? 0 : (double)Lats / SetCount;
        public double TricepsPercentage => SetCount == 0 ? 0 : (double)Triceps / SetCount;
        public double BicepsPercentage => SetCount == 0 ? 0 : (double)Biceps / SetCount;
        public double QuadsPercentage => SetCount == 0 ? 0 : (double)Quadriceps / SetCount;
        public double HamsPercentage => SetCount == 0 ? 0 : (double)Hamstrings / SetCount;
        public double GlutesPercentage => SetCount == 0 ? 0 : (double)Glutes / SetCount;
        public double CalvesPercentage => SetCount == 0 ? 0 : (double)Calves / SetCount;
        public double AbsPercentage => SetCount == 0 ? 0 : (double)Abdominals / SetCount;
        public double ObliquesPercentage => SetCount == 0 ? 0 : (double)Obliques / SetCount;
        public double FDPercentage => SetCount == 0 ? 0 : (double)FrontDelts / SetCount;
        public double RDPercentage => SetCount == 0 ? 0 : (double)RearDelts / SetCount;
        public double LDPercentage => SetCount == 0 ? 0 : (double)LateralDelts / SetCount;
        public double TrapsPercentage => SetCount == 0 ? 0 : (double)Traps / SetCount;
        public double ForearmsPercentage => SetCount == 0 ? 0 : (double)Forearms / SetCount;
        public double PushPercentage => SetCount == 0 ? 0 : (double)Push / SetCount;
        public double ChestPercentage => SetCount == 0 ? 0 : (double)Chest / SetCount;
        public double BackPercentage => SetCount == 0 ? 0 : (double)Back / SetCount;
        public double LegsPercentage => SetCount == 0 ? 0 : (double)Legs / SetCount;
        public double ArmsPercentage => SetCount == 0 ? 0 : (double)Arms / SetCount;
        public double CorePercentage => SetCount == 0 ? 0 : (double)Core / SetCount;
        public double ShouldersPercentage => SetCount == 0 ? 0 : (double)Shoulders / SetCount;
        public double PullPercentage => SetCount == 0 ? 0 : (double)Pull / SetCount;

    }
}
