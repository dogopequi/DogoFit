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
        public ICommand StartEmptyCommand { get; }
        public ICommand LoadMoreExercisesCommand { get; }
        public ObservableCollection<GymTracker.Services.Routine> Routines { get; }
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }
        public ObservableCollection<Exercise> DisplayedExercises { get; set; }
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

            exercise.RemoveSet(set.ID, set.Side);

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

        public ObservableCollection<Category> Categories { get; set; }
        public ICommand FilterCommand { get; }

        public ICommand SelectExerciseCommand { get; }

        public ICommand SaveExercisesToWorkoutCommand { get; }

        public ICommand SaveNewRoutineCommand { get; }


        public ICommand EditRoutineCommand { get; }

        public ICommand ResumeWorkoutInProgressCommand { get; }
        public ICommand DiscardWorkoutInProgressCommand { get; }

        public WorkoutViewModel()
        {
            DisplayedExercises = new ObservableCollection<Exercise>();
            AppState.MaxExercises = 20;
            WorkoutInProgress = AppState.WorkoutInProgress;
            if (AppState.WorkoutInProgress == null)
                OnDiscard();
            LoadMoreExercisesCommand = new Command(() => { 
                AppState.MaxExercises += 20;
                DisplayedExercises.Clear();
                int i = 0;
                foreach(Exercise e in FilteredExercises)
                {
                    if (i >= AppState.MaxExercises)
                        break;
                    DisplayedExercises.Add(e);
                    i++;
                }
            });
            Categories = AppState.Categories;
            Routines = AppState.Routines;
            CurrentRoutineName = AppState.CurrentRoutineName;
            SelectedExercises = AppState.SelectedExercises;
            AddRoutineCommand = new Command(OnAddRoutine);
            StartWorkoutCommand = new Command<Routine>(OnStartWorkout);
            AddExerciseCommand = new Command(OnAddExercise);
            EditRoutineCommand = new Command<Routine>(OnEditRoutine);
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            AppState.FilterByCategory("All", false);
            FilterCommand = new Command<Category>(AppState.SelectCategory);
            SelectExerciseCommand = new Command<Exercise>(SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSetSelectionForRoutine);
            SaveNewRoutineCommand = new Command(OnSaveNewRoutine);
            ResumeWorkoutInProgressCommand = new Command(OnResume);
            DiscardWorkoutInProgressCommand = new Command(OnDiscard);
            StartEmptyCommand = new Command(OnStartEmpty);
        }
        public async void OnStartEmpty()
        {
            AppState.WorkoutState = WorkoutStates.EmptyWorkout;
            WorkoutInProgress = AppState.WorkoutInProgress;
            AppState.FilteredExercises.Clear();
            AppState.SelectedExercises.Clear();
            await Shell.Current.GoToAsync("createroutine");
        }
        public async Task OnRemoveExercise(Exercise exercise)
        {
            if (exercise == null) return;
            var result = await Shell.Current.DisplayAlert("Remove Exercise", $"Are you sure you want to delete this exercise?", "Yes", "No");
            if (result)
            {
                SelectedExercises.Remove(exercise);
            }
        }
        public void OnResume()
        {
            if (WorkoutInProgress != null)
            {
                AppState.WorkoutState = WorkoutStates.WorkoutInProgress;
                Shell.Current.GoToAsync("startroutine");
            }
        }

        public void OnDiscard()
        {
            if (WorkoutInProgress != null)
            {
                AppState.WorkoutInProgress = null;
                AppState.WorkoutState = WorkoutStates.None;
                WorkoutInProgress = null;
                DbHelper.DeleteWorkoutInProgress(App.Db);
            }
        }

        public void AddSet(Exercise exercise, SideType type)
        {
            if (exercise == null) return;
            Set set = new Set();
            set.Side = type;
            exercise.AddSet(set);
            exercise.RecalculateSetIndexes();
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


        public async void OnSetSelectionForRoutine()
        {
            switch(AppState.WorkoutState)
            {
                case WorkoutStates.EditingRoutine:
                    await Shell.Current.Navigation.PopAsync();
                    return;
                case WorkoutStates.WorkoutInProgress:
                    await Shell.Current.Navigation.PopAsync();
                    return;
                default:
                    await Shell.Current.GoToAsync("createroutine/addexercise/setselection");
                    return;
            }
        }
        private async void OnAddRoutine()
        {
            AppState.FilteredExercises.Clear();
            AppState.SelectedExercises.Clear();
            AppState.CurrentRoutineName = "";
            AppState.WorkoutState = WorkoutStates.NewRoutine;
            await Shell.Current.GoToAsync("createroutine");
        }

        private async void OnStartWorkout(Routine routine)
        {
            AppState.WorkoutInProgress = new Routine(routine, true);;
            AppState.WorkoutState = WorkoutStates.StartRoutine;
            WorkoutInProgress = AppState.WorkoutInProgress;
            await Shell.Current.GoToAsync("startroutine");
        }

        public async Task OnDeleteRoutine(Routine routine)
        {
            var result = await Shell.Current.DisplayAlert(
                "Delete Routine",
                $"Are you sure you want to delete the routine '{routine.Name}'?",
                "Yes", "No");
        
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

                if (AppState.WorkoutState == WorkoutStates.EmptyWorkout)
                {
                    Routine routine = new Routine();
                    routine.Name = CurrentRoutineName;
                    AppState.WorkoutInProgress = routine;
                    AppState.WorkoutInProgress.Name = CurrentRoutineName;
                    AppState.CurrentRoutineName = string.Empty;
                    AppState.FilterByCategory("All", false);

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
                if(exercise.Description != null)
                {
                    if(exercise.Description.Length > 200)
                    {
                        await Shell.Current.DisplayAlert("Error", "Exercise: " + exercise.Name + " has a description over 200 characters.", "OK");
                        return;
                    }
                }
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

            if (AppState.WorkoutState == WorkoutStates.WorkoutInProgress)
            {
                AppState.WorkoutInProgress = routine;
                AppState.WorkoutInProgress.Name = CurrentRoutineName;
                AppState.CurrentRoutineName = string.Empty;
                AppState.FilterByCategory("All", false);

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
                 AppState.FilterByCategory("All", false);

                await Shell.Current.Navigation.PopToRootAsync();
            }
        }


        public async void OnEditRoutine(Routine routine)
        {
            AppState.CurrentRoutine = routine;
            AppState.EditedRoutine = new Routine(routine, true);
            AppState.WorkoutState = WorkoutStates.EditingRoutine;
            await Shell.Current.GoToAsync("editroutine");
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
