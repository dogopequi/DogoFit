using GymTracker.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GymTracker.Models
{
    internal class StartRoutineViewModel
    {
        public ICommand AddSetToExerciseCommand { get; }

        public ICommand RemoveSetFromExerciseCommand { get; }
        public ICommand FinishRoutineCommand { get; }
        public ICommand AddExerciseCommand { get; }
        
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand SaveExercisesToWorkoutCommand { get; }
        public ObservableCollection<String> TakenExercises { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();
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
            routine = AppState.WorkoutInProgress;
            if(routine == null)
            {
                routine = new Routine();
                AppState.WorkoutInProgress = routine;
                routine.Start();
            }
            else
            {

                if(AppState.IsNewRoutine)
                {
                    routine.Start();
                }
                else
                {
                    routine.Resume();
                }
            }
            if (routine.Exercises != null)
            {
                foreach (var exercise in routine.Exercises)
                {
                    foreach (var set in exercise.Sets)
                    {
                        var lastSet = AppState.GetLastSetForExercise(exercise.Name, set.ID);
                        set.LastSet = lastSet != null
                            ? $"{lastSet.ID}x{lastSet.Reps}x{lastSet.Weight}"
                            : "-";
                    }
                }
            }
            TakenExercises = new ObservableCollection<string>(routine.Exercises.Select(e => e.Name));
            Categories = AppState.Categories;
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            SelectedExercises = AppState.SelectedExercises;
            FilterByCategory("All", false);
            FilterCommand = new Command<Category>(SelectCategory);
            AddSetToExerciseCommand = new Command<Exercise>(OnAddSetToExercise);
            RemoveSetFromExerciseCommand = new Command<Exercise>(OnRemoveSetFromExercise);
            FinishRoutineCommand = new Command(OnFinishRoutine);
            RemoveExerciseCommand = new Command<Exercise>(OnRemoveExercise);
            SelectExerciseCommand = new Command<Exercise>(SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSaveExercises);
            AddExerciseCommand = new Command(OnAddExercise);
            AppState.IsNewRoutine = false;
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

            exercise.RemoveSet(set.ID);

            for (int i = 0; i < exercise.Sets.Count; i++)
            {
                exercise.Sets[i].ID = i + 1;
            }
        }


        private void OnAddSetToExercise(Exercise exercise)
        {
            if (exercise == null) return;
            Set set = new Set();
            set.ID = exercise.SetCount + 1;
            exercise.AddSet(set);
            AppState.SaveWorkoutInProgress();
        }

        private void OnRemoveSetFromExercise(Exercise exercise)
        {
            if (exercise.Sets.Count > 0)
            {
                var setToRemove = exercise.Sets.LastOrDefault();
                if (setToRemove != null)
                {
                    exercise.RemoveSet(setToRemove.ID);
                    AppState.SaveWorkoutInProgress();
                }
            }
        }
        private async void OnFinishRoutine()
        {
            if (Shell.Current.CurrentPage is ContentPage page)
                page.Focus();

            if (routine == null) return;

            routine.Finish();
            AppState.AddRoutineToWorkouts(routine);
            AppState.IsWorkoutInProgress = false;
            AppState.WorkoutInProgress = null;
            AppState.SaveWorkoutInProgress();

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
                        .ToDictionary(s => s.ID);
                    foreach (var set in exercise.CheckedSets)
                    {
                        if (templateSets.TryGetValue(set.ID, out var s))
                        {
                            s.Reps = set.Reps;
                            s.Weight = set.Weight;
                        }
                    }
                }
            }


            await Shell.Current.Navigation.PopToRootAsync();
        }

        private async void OnAddExercise()
        {
            await Shell.Current.GoToAsync("startroutine/addexercise");
        }

        private async void OnRemoveExercise(Exercise exercise)
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
            for (int i = SelectedExercises.Count - 1; i >= 0; i--)
            {
                if (TakenExercises.Contains(SelectedExercises[i].Name))
                {
                    SelectedExercises[i].IsSelected = false;
                    SelectedExercises.RemoveAt(i);
                }
            }
            foreach (var exercise in SelectedExercises)
            {
                exercise.IsSelected = false;
                var newexercise = new Exercise(exercise);
                newexercise.IsSelected = false;
                routine.AddExercise(newexercise);
                TakenExercises.Add(newexercise.Name);
            }

            foreach (var exercise in AllExercises)
            {
                exercise.IsSelected = false;
                exercise.Description = string.Empty;
            }
            foreach (var exercise in FilteredExercises)
            {
                exercise.IsSelected = false;
                exercise.Description = string.Empty;
            }
            FilteredExercises.Clear();
            SelectedExercises.Clear();
            AppState.SelectedExerciseIds.Clear();

            await Shell.Current.Navigation.PopAsync();
        }
        public void SelectExercise(Exercise exercise)
        {
            exercise.IsSelected = !exercise.IsSelected;

            if (exercise.IsSelected)
            {
                if (!SelectedExercises.Any(e => e.Name == exercise.Name))
                {
                    SelectedExercises.Add(new Exercise(exercise));
                    AppState.SelectedExerciseIds.Add(exercise.Name);
                }
            }
            else
            {
                var toRemove = SelectedExercises.FirstOrDefault(e => e.Name == exercise.Name);
                if (toRemove != null)
                    SelectedExercises.Remove(toRemove);

                AppState.SelectedExerciseIds.Remove(exercise.Name);
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
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        
    }
}
