using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GymTracker.Services
{
    public class Routine : INotifyPropertyChanged
    {
        private IDispatcherTimer? _timer;

        [JsonInclude]
        public long RoutineID { get; }

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


        [JsonInclude]
        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get => IsRunning ? DateTime.Now - StartTime : _duration;
            set => _duration = value;
        }

        [JsonIgnore]
        public int ID { get; set; }
      
        [JsonIgnore]
        public string DurationString
        {
            get
            {
                var ts = IsRunning ? DateTime.Now - StartTime : Duration;

                if (ts.TotalHours >= 24)
                {
                    return $"{ts.Days}d {ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
                }
                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            }
        }



        [JsonIgnore]
        public bool IsRunning => EndTime == null;
        [JsonInclude]
        public DateTime StartTime { get; set; }

        [JsonInclude]
        public DateTime? EndTime { get; set; }
        [JsonInclude]
        public string Name { get; set; }
        [JsonInclude]

        private ObservableCollection<Exercise> _exercises = new ObservableCollection<Exercise>();
        [JsonInclude]
        public ObservableCollection<Exercise> Exercises
        {
            get => _exercises;
            set
            {
                _exercises = value;

            }
        }


        public Routine()
        {
            RoutineID = Random.Shared.NextInt64(long.MinValue, long.MaxValue);
        }

        public Routine(Routine other, bool copyID)
        {
            Name = other.Name;
            StartTime = other.StartTime;
            EndTime = other.EndTime;
            Duration = other.Duration;

            if (copyID)
                RoutineID = other.RoutineID;
            else
                RoutineID = Random.Shared.NextInt64(long.MinValue, long.MaxValue);

            Exercises = new ObservableCollection<Exercise>(
                other.Exercises.Select(ex => new Exercise(ex))
            );
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

        public int Sets => Exercises.SelectMany(e => e.CheckedSets).Count();
        public int Reps => Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Reps);
        public double Volume => Exercises.SelectMany(e => e.CheckedSets).Sum(s => s.Weight * s.Reps);
        public string VolumeDisplay => $"{Volume:N2} {AppState.Profile.WeightUnit}";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}