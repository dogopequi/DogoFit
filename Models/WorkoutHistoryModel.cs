using GymTracker.Services;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace GymTracker.Models
{
    internal class WorkoutHistoryModel : INotifyPropertyChanged
    {
        private int pageSize = 20;
        private int currentPage = 1;
        public bool HasRoutinesChanged = false;
        public ICommand LoadMoreCommand { get; }

        private ObservableCollection<Routine> _routines;
        public ObservableCollection<Routine> Routines
        {
            get => _routines;
            set
            {
                _routines = value;
                OnPropertyChanged(nameof(Routines));
            }
        }
        public ICommand RemoveWorkoutCommand { get; }
        public Services.Profile Profile => AppState.Profile;
        public bool ApplyFilter = false;
        private DateTime Start;
        private DateTime End;
        public WorkoutHistoryModel()
        {
            var sorted = AppState.Workouts.OrderByDescending(w => w.StartTime).Take(pageSize);
            Routines = new ObservableCollection<Routine>(sorted);
            AppState.Workouts.CollectionChanged += Workouts_CollectionChanged;
            AppState.Profile.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(Profile.UseMetric))
                {
                    RefreshRoutines(null);
                }
            };
            RemoveWorkoutCommand = new Command<Routine>(OnDeleteWorkout);
            LoadMoreCommand = new Command(() =>
            {
                currentPage++;
                IEnumerable<Routine> items = null;
                if(ApplyFilter == true)
                {
                    items = AppState.Workouts.Where(r => r.StartTime >= Start && r.StartTime <= End)
                        .OrderByDescending(w => w.StartTime)
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize);
                }
                else
                {
                    items = AppState.Workouts
                        .OrderByDescending(w => w.StartTime)
                        .Skip((currentPage - 1) * pageSize)
                        .Take(pageSize);
                }
                if(items != null)
                    foreach (var item in items)
                        if (!Routines.Contains(item))
                            Routines.Add(item);
            });
        }
        public bool MoreButtonVisibility()
        {
            int totalCount = ApplyFilter 
                     ? AppState.Workouts.Count(r => r.StartTime >= Start && r.StartTime <= End)
                     : AppState.Workouts.Count();

            return totalCount > Routines.Count() && totalCount > pageSize;
        }
        public void FilterWorkouts(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            var filtered = AppState.Workouts
                                 .Where(r => r.StartTime >= start && r.StartTime <= end)
                                 .OrderByDescending(w => w.StartTime).Take(pageSize);
            RefreshRoutines(filtered);
        }
        public void RefreshRoutines(IEnumerable<Routine> filtered)
        {
            if(filtered == null)
            {
                filtered = AppState.Workouts.OrderByDescending(w => w.StartTime).Take(pageSize);
            }
            Routines = new ObservableCollection<Routine>(filtered);
            HasRoutinesChanged = true;
        }

        private void Workouts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Routine r in e.NewItems)
                {
                    int index = Routines.TakeWhile(x => x.StartTime > r.StartTime).Count();
                    Routines.Insert(index, r);
                }
            }
            if (e.OldItems != null)
            {
                foreach (Routine r in e.OldItems)
                {
                    if (Routines.Contains(r))
                        Routines.Remove(r);
                }
            }
        }

        public async void OnDeleteWorkout(Routine routine)
        {
            var result = await Shell.Current.DisplayAlert(
                "Delete Workout",
                $"Are you sure you want to delete the workout '{routine.Name}'?",
                "Yes",
                "No"
            );

            if (result)
            {
                AppState.RemoveWorkout(routine);
                Routines.Remove(routine);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
