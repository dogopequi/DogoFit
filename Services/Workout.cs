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


namespace GymTracker.Services
{
    public class Routine : INotifyPropertyChanged
    {
        private int _volume;
        private int _setCount;
        private int _repCount;
        private TimeSpan _duration;
        private System.Timers.Timer _timer;

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
        [JsonInclude]
        public TimeSpan Duration
        {
            get => _duration;
            private set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(DurationString));
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
        public string DurationString => Duration.ToString(@"hh\:mm\:ss");
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
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
            Exercises = new ObservableCollection<Exercise>(other.Exercises.Select(ex => new Exercise
            {
                Name = ex.Name,
                Description = ex.Description,
                Sets = new ObservableCollection<Set>(ex.Sets.Select(s => new Set
                {
                    ID = s.ID,
                    Reps = s.Reps,
                    Weight = s.Weight
                }))
            }));
            SubscribeExerciseEvents(Exercises);
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
            SetCount = Exercises.Sum(ex => ex.Sets.Count);
            Volume = Exercises.Sum(ex => ex.Sets.Sum(s => s.Reps * s.Weight));
            RepCount = Exercises.Sum(ex => ex.Sets.Sum(s => s.Reps));
        }
        public void AddExercise(Exercise exercise)
        {
            var newexercise = new Exercise(exercise);
            Exercises.Add(newexercise);
        }

        public void RemoveExercise(Exercise exercise)
        {
            Exercises.Remove(exercise);
        }

        public void Start()
        {
            StartTime = DateTime.Now;
            Duration = TimeSpan.Zero;

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) =>
            {
                Duration = DateTime.Now - StartTime;
            };
            _timer.Start();
        }

        public void Finish()
        {
            EndTime = DateTime.Now;
            Duration = EndTime - StartTime;
            _timer?.Stop();
            _timer?.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
