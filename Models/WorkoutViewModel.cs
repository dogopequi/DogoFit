using GymTracker.Services;
using GymTracker.Views;
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
        public ICommand RemoveSetFromExercise { get; }
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

        public ICommand FilterCommand { get; }

        public ICommand SelectExerciseCommand { get; }

        public ICommand SaveExercisesToWorkoutCommand { get; }

        public ICommand SaveNewRoutineCommand { get; }

        public ICommand DeleteRoutineCommand { get; }

        public WorkoutViewModel()
        {
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
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            FilterByCategory("All");
            FilterCommand = new Command<Category>(SelectCategory);
            AddSetToExercise = new Command<Exercise>(AddSet);
            RemoveSetFromExercise = new Command<Exercise>(RemoveSet);
            SelectExerciseCommand = new Command<Exercise>(SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSetSelectionForRoutine);
            SaveNewRoutineCommand = new Command(OnSaveNewRoutine);
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

        public void RemoveSet(Exercise exercise)
        {
            if (exercise.Sets.Count > 0)
            {
                var setToRemove = exercise.Sets.LastOrDefault();
                if (setToRemove != null)
                {
                    exercise.RemoveSet(setToRemove.ID);
                }
            }
        }
        private void SelectCategory(Category category)
        {
            foreach (var c in Categories)
                c.IsSelected = false;

            category.IsSelected = true;

            FilterByCategory(category.Name);
        }

        public void SelectExercise(Exercise exercise)
        {
            exercise.IsSelected = !exercise.IsSelected;

            if (exercise.IsSelected)
            {
                if (!SelectedExercises.Contains(exercise))
                    SelectedExercises.Add(exercise);
            }
            else
            {
                SelectedExercises.Remove(exercise);
            }
        }


        private void FilterByCategory(string category)
        {
            FilteredExercises.Clear();

            var filtered = category == "All"
                ? AllExercises
                : AllExercises.Where(e => e.Categories.Contains(category, StringComparer.OrdinalIgnoreCase));

            foreach (var ex in filtered)
                FilteredExercises.Add(ex);
        }

        public async void OnSetSelectionForRoutine()
        {
            await Shell.Current.GoToAsync("createroutine/addexercise/setselection");
        }
        private async void OnAddRoutine()
        {
            AppState.FilteredExercises.Clear();
            await Shell.Current.GoToAsync("createroutine");
        }

        private async void OnStartWorkout(Routine routine)
        {
            AppState.CurrentRoutine = new Routine(routine);
            await Shell.Current.GoToAsync("startroutine");
        }

        private async void OnDeleteRoutine(Routine routine)
        {
            AppState.RemoveWorkout(routine);
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
                    await Shell.Current.DisplayAlert("Error", "Routine name already exists. Please choose a different name.", "OK");
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
                var newexercise = new Exercise(exercise);
                newexercise.IsSelected = false;
                routine.AddExercise(newexercise);
            }
            foreach (var exercise in AllExercises)
            {
                exercise.IsSelected = false;
                exercise.Description = string.Empty;
            }
            SelectedExercises.Clear();
            AppState.Routines.Add(routine);
            var template = DbHelper.ToDbRoutineTemplate(routine);
            App.Db.RoutineTemplates.Add(template);
            App.Db.SaveChanges();
            AppState.CurrentRoutineName = string.Empty;
            await Shell.Current.Navigation.PopToRootAsync();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
