using CommunityToolkit.Maui;
using GymTracker.Services;
using GymTracker.Views;
using Microsoft.Maui.Controls;
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
    class WorkoutViewModel : INotifyPropertyChanged
    {
        public string CurrentRoutineName { get; set; }
        public ICommand AddRoutineCommand { get; }
        public ICommand StartWorkoutCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand AddSetToExercise { get; }
        public ICommand StartEmptyCommand { get; }
        public ObservableCollection<GymTracker.Services.Routine> Routines { get; }
        private int _routinesCount;
        public int RoutinesCount
        {
            get => _routinesCount;
            set
            {
                if (_routinesCount != value)
                {
                    _routinesCount = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }

        public ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();

        private Routine? _workoutInProgress;
        public Routine? WorkoutInProgress
        {
            get => _workoutInProgress;
            set
            {
                if (_workoutInProgress != value)
                {
                    _workoutInProgress = value;
                    OnPropertyChanged();
                }
            }
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


        public ICommand FilterCommand { get; }

        public ICommand SelectExerciseCommand { get; }

        public ICommand SaveExercisesToWorkoutCommand { get; }

        public ICommand SaveNewRoutineCommand { get; }

        public ICommand DeleteRoutineCommand { get; }

        public ICommand EditRoutineCommand { get; }

        public ICommand ResumeWorkoutInProgressCommand { get; }
        public ICommand DiscardWorkoutInProgressCommand { get; }

        public WorkoutViewModel()
        {
            WorkoutInProgress = AppState.WorkoutInProgress;
            if (AppState.IsWorkoutInProgress == false)
                OnDiscard();
            Categories = AppState.Categories;
            Routines = AppState.Routines;
            Routines.CollectionChanged += (s, e) => RoutinesCount = Routines.Count;
            RoutinesCount = AppState.RoutinesCount;
            CurrentRoutineName = AppState.CurrentRoutineName;
            SelectedExercises = AppState.SelectedExercises;
            AddRoutineCommand = new Command(OnAddRoutine);
            StartWorkoutCommand = new Command<Routine>(OnStartWorkout);
            AddExerciseCommand = new Command(OnAddExercise);
            DeleteRoutineCommand = new Command<Routine>(OnDeleteRoutine);
            EditRoutineCommand = new Command<Routine>(OnEditRoutine);
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            FilterByCategory("All", false);
            FilterCommand = new Command<Category>(SelectCategory);
            AddSetToExercise = new Command<Exercise>(AddSet);
            SelectExerciseCommand = new Command<Exercise>(SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSetSelectionForRoutine);
            SaveNewRoutineCommand = new Command(OnSaveNewRoutine);
            ResumeWorkoutInProgressCommand = new Command(OnResume);
            DiscardWorkoutInProgressCommand = new Command(OnDiscard);
            StartEmptyCommand = new Command(OnStartEmpty);
        }
        public async void OnStartEmpty()
        {
            AppState.WorkoutInProgress = new Routine();
            AppState.IsNewRoutine = true;
            AppState.IsEmptyWorkout = true;
            WorkoutInProgress = AppState.WorkoutInProgress;
            AppState.IsWorkoutInProgress = true;
            AppState.FilteredExercises.Clear();
            AppState.SelectedExercises.Clear();
            await Shell.Current.GoToAsync("createroutine");
        }
        public void OnResume()
        {
            if (WorkoutInProgress != null)
            {
                AppState.IsNewRoutine = false;
                Shell.Current.GoToAsync("startroutine");
            }
        }

        public void OnDiscard()
        {
            if (WorkoutInProgress != null)
            {
                AppState.WorkoutInProgress = null;
                AppState.IsWorkoutInProgress = false;
                WorkoutInProgress = null;
                DbHelper.DeleteWorkoutInProgress(App.Db);
            }
        }

        public void AddSet(Exercise exercise)
        {
            exercise.AddSet(new Set
            {
                ID = exercise.Sets.Count + 1,
                Reps = 0,
                Weight = 0
            });

        }

        private void SelectCategory(Category category)
        {
            foreach (var c in Categories)
                c.IsSelected = false;

            category.IsSelected = true;

            FilterByCategory(category.Name, false);
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


        public async void OnSetSelectionForRoutine()
        {
            if (AppState.IsEditingRoutine || (!AppState.IsEmptyWorkout && AppState.IsWorkoutInProgress))
            {
                await Shell.Current.Navigation.PopAsync();
                return;
            }
            await Shell.Current.GoToAsync("createroutine/addexercise/setselection");
        }
        private async void OnAddRoutine()
        {
            AppState.FilteredExercises.Clear();
            AppState.SelectedExercises.Clear();
            await Shell.Current.GoToAsync("createroutine");
        }

        private async void OnStartWorkout(Routine routine)
        {
            AppState.WorkoutInProgress = new Routine(routine, true);
            AppState.IsNewRoutine = true;
            WorkoutInProgress = AppState.WorkoutInProgress;
            AppState.IsWorkoutInProgress = true;
            await Shell.Current.GoToAsync("startroutine");
        }

        public async void OnDeleteRoutine(Routine routine)
        {
            var result = await Shell.Current.DisplayAlert("Delete Routine", $"Are you sure you want to delete the routine '{routine.Name}'?", "Yes", "No");
            if (result)
            {
                AppState.RemoveRoutine(routine);
            }
        }

        private async void OnAddExercise()
        {
            if (string.IsNullOrWhiteSpace(CurrentRoutineName))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a routine name.", "OK");
                return;
            }
            else
            {
                if (!AppState.ValidateRoutineName(CurrentRoutineName))
                {                     
                    await Shell.Current.DisplayAlert("Error", "Routine name is over 20 characters long.", "OK");
                    return;
                }
                if (AppState.IsEmptyWorkout)
                {
                    Routine routine = new Routine();
                    routine.Name = CurrentRoutineName;
                    AppState.WorkoutInProgress = routine;
                    AppState.WorkoutInProgress.Name = CurrentRoutineName;
                    AppState.CurrentRoutineName = string.Empty;
                    AppState.IsEmptyWorkout = false;
                    FilterByCategory("All", false);

                    await Shell.Current.Navigation.PopToRootAsync();
                    await Shell.Current.GoToAsync("startroutine");
                    return;
                }
                AppState.CurrentRoutineName = CurrentRoutineName;
                await Shell.Current.GoToAsync("createroutine/addexercise");
            }
        }


        private async void OnSaveNewRoutine()
        {
            Routine routine = new Routine();
            routine.Name = CurrentRoutineName;

            foreach (var exercise in SelectedExercises)
            {
                exercise.IsSelected = false;
                var newexercise = new Exercise(exercise);
                newexercise.IsSelected = false;
                routine.AddExercise(newexercise);
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

            if (AppState.IsWorkoutInProgress)
            {
                AppState.WorkoutInProgress = routine;
                AppState.WorkoutInProgress.Name = CurrentRoutineName;
                AppState.CurrentRoutineName = string.Empty;
                AppState.IsEmptyWorkout = false;
                FilterByCategory("All", false);

                await Shell.Current.Navigation.PopToRootAsync();
                await Shell.Current.GoToAsync("startroutine");
            }
            else
            {
                AppState.Routines.Add(routine);
                var template = DbHelper.ToDbRoutineTemplate(routine);
                App.Db.RoutineTemplates.Add(template);
                App.Db.SaveChanges();

                AppState.CurrentRoutineName = string.Empty;
                FilterByCategory("All", false);

                await Shell.Current.Navigation.PopToRootAsync();
            }
        }


        public async void OnEditRoutine(Routine routine)
        {
            AppState.CurrentRoutine = routine;
            AppState.EditedRoutine = new Routine(routine, true);
            AppState.IsEditingRoutine = true;
            await Shell.Current.GoToAsync("editroutine");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
