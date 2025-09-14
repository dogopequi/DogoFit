using GymTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace GymTracker.Models
{
    internal class StartRoutineViewModel
    {
        public ICommand RemoveSetFromExerciseCommand { get; }
        public ICommand FinishRoutineCommand { get; }
        public ICommand AddExerciseCommand { get; }
        
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> DisplayedExercises { get; set; }
        public ICommand LoadMoreExercisesCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand SaveExercisesToWorkoutCommand { get; }
        public ObservableCollection<String> TakenExercises { get; set; } = new ObservableCollection<String>();

        public ICommand FilterCommand { get; }
        private Routine? _routine;
        public Routine? routine
        {
            get => _routine;
            set
            {
                if (_routine != value)
                {
                    _routine = value;
                    OnPropertyChanged();
                    AppState.SaveWorkoutInProgress();
                }
            }
        }

        private string? _enteredexercisename;
        public string? EnteredExerciseName
        {
            get => _enteredexercisename;
            set
            {
                if (_enteredexercisename != value)
                {
                    _enteredexercisename = value;
                    OnPropertyChanged();
                }
            }
        }

        public StartRoutineViewModel()
        {
            DisplayedExercises = new ObservableCollection<Exercise>();
            AppState.MaxExercises = 20;
            AppState.FillDisplayedExercises(DisplayedExercises);
            LoadMoreExercisesCommand = new Command(() => { 
                AppState.MaxExercises += 20;
                AppState.FillDisplayedExercises(DisplayedExercises);
            });
            routine = AppState.WorkoutInProgress;
            if (routine != null)
            {
                if (AppState.WorkoutState == WorkoutStates.WorkoutInProgress)
                {
                    routine.Resume();
                }
                else
                {
                    routine.Start();
                }
            }
            else
            {
                routine = new Routine();
                routine.Start();
            }
            FillLastSets();
            TakenExercises = new ObservableCollection<string>(routine.Exercises.Select(e => e.Name));
            Categories = AppState.Categories;
            AppState.FilterByCategory("All", false);
            FilterCommand = new Command<Category>((Category cat) => { AppState.SelectCategory(cat); AppState.FillDisplayedExercises(DisplayedExercises); });
            RemoveSetFromExerciseCommand = new Command<Exercise>(OnRemoveSetFromExercise);
            FinishRoutineCommand = new Command(OnFinishRoutine);
            RemoveExerciseCommand = new Command<Exercise>(OnRemoveExercise);
            SelectExerciseCommand = new Command<Exercise>(AppState.SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSaveExercises);
            AddExerciseCommand = new Command(OnAddExercise);
            AppState.WorkoutState = WorkoutStates.WorkoutInProgress;
        }
        public void OnEditSetWarmup(Set set)
        {
            set.Type = SetType.Warmup;
        }
        public void OnEditSetDrop(Set set)
        {
            set.Type = SetType.Drop;
        }
        public void OnEditSetNormal(Set set)
        {
            set.Type = SetType.Normal;
        }
        public void OnEditSetFailure(Set set)
        {
            set.Type = SetType.Failure;
        }

        public void OnRemoveSet(Exercise exercise, Set set)
        {
            if (exercise?.Sets == null) return;

            exercise.RemoveSet(set.ID, set.Side);

            for (int i = 0; i < exercise.Sets.Count; i++)
            {
                exercise.Sets[i].ID = i + 1;
            }
        }

        private void OnRemoveSetFromExercise(Exercise exercise)
        {
            if (exercise.Sets.Count > 0)
            {
                var setToRemove = exercise.Sets.LastOrDefault();
                if (setToRemove != null)
                {
                    exercise.RemoveSet(setToRemove.ID, setToRemove.Side);
                    AppState.SaveWorkoutInProgress();
                }
            }
        }
        private async void OnFinishRoutine()
        {
            if (Shell.Current.CurrentPage is ContentPage page)
                page.Focus();

            if (routine == null) return;
            foreach (Exercise exercise in routine.Exercises)
            {
                if (exercise.Description != null)
                {
                    if (exercise.Description.Length > 200)
                    {
                        await Shell.Current.DisplayAlert("Error", "Exercise: " + exercise.Name + " has a description over 200 characters.", "OK");
                        return;
                    }
                }
            }

            routine.Finish();
            AppState.AddRoutineToWorkouts(routine);
            AppState.WorkoutState = WorkoutStates.None;
            AppState.WorkoutInProgress = null;

            var template = AppState.Routines
                .FirstOrDefault(r => r.RoutineID == routine.RoutineID);
            if (template != null)
            {
                foreach (var exercise in routine.Exercises)
                {
                    var templateExercise = template.Exercises
                        .FirstOrDefault(e => e.Name == exercise.Name);
                    if (templateExercise == null || templateExercise.Sets == null)
                        continue;
                    var templateSets = templateExercise.Sets
                        .ToDictionary(s => (s.ID, s.Side));
                    foreach (var set in exercise.CheckedSets)
                    {
                        if (templateSets.TryGetValue((set.ID, set.Side), out var s))
                        {
                            s.Reps = set.Reps;
                            s.Weight = set.Weight;
                        }
                    }
                }
            }


            await Shell.Current.Navigation.PopToRootAsync();
        }

        public async void OnAddExercise()
        {
            await Shell.Current.GoToAsync("startroutine/addexercise");
        }

        public async void OnRemoveExercise(Exercise exercise)
        {
            if (exercise == null) return;
            var result = await Shell.Current.DisplayAlert("Remove Exercise", $"Are you sure you want to delete this exercise?", "Yes", "No");
            if (result)
            {
                routine.Exercises.Remove(exercise);
                TakenExercises.Remove(exercise.Name);
            }
        }
        public async void OnSaveExercises()
        {
            for (int i = AppState.SelectedExercises.Count - 1; i >= 0; i--)
            {
                if (TakenExercises.Contains(AppState.SelectedExercises[i].Name))
                {
                    AppState.SelectedExercises[i].IsSelected = false;
                    AppState.SelectedExercises.RemoveAt(i);
                }
            }
            foreach (var exercise in AppState.SelectedExercises)
            {
                exercise.IsSelected = false;
                var newexercise = new Exercise(exercise);
                newexercise.IsSelected = false;
                routine.AddExercise(newexercise);
                TakenExercises.Add(newexercise.Name);
            }

            AppState.ClearExerciseFilters();
            FillLastSets();

            await Shell.Current.Navigation.PopAsync();
        }

        private void FillLastSets()
        {
            if (routine.Exercises != null)
            {
                foreach (var exercise in routine.Exercises)
                {
                    foreach (var set in exercise.Sets)
                    {
                        var lastSet = AppState.GetLastSetForExercise(exercise.Name, set.ID, set.Side);
                        set.LastSet = lastSet != null
                            ? $"{lastSet.Reps}x{lastSet.Weight:0}"
                            : "-";
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
