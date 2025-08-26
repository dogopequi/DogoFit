using GymTracker.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GymTracker.Models
{
    internal class EditRoutineModel
    {

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

        public Routine routine { get; set; }
        public ICommand AddSetToExerciseCommand { get; }
        public ICommand SaveRoutineCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveSetFromExerciseCommand { get; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> AllExercises { get; set; }
        public ObservableCollection<Exercise> FilteredExercises { get; set; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand RemoveExerciseCommand { get; }
        public ICommand SaveEditedRoutineCommand { get; }
        public ICommand SaveExercisesToWorkoutCommand { get; }
        public ObservableCollection<String> TakenExercises { get; set; } = new ObservableCollection<String>();
        public ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();

        public ICommand FilterCommand { get; }
        public EditRoutineModel()
        {
            routine = AppState.EditedRoutine;
            TakenExercises = new ObservableCollection<string>(routine.Exercises.Select(e => e.Name));
            AllExercises = AppState.AllExercises;
            FilteredExercises = AppState.FilteredExercises;
            SelectedExercises = AppState.SelectedExercises;
            Categories = AppState.Categories;
            FilterByCategory("All", false);
            FilterCommand = new Command<Category>(SelectCategory);
            AddSetToExerciseCommand = new Command<Exercise>(OnAddSetToExercise);
            RemoveSetFromExerciseCommand = new Command<Exercise>(OnRemoveSetFromExercise);
            RemoveExerciseCommand = new Command<Exercise>(OnRemoveExercise);
            SelectExerciseCommand = new Command<Exercise>(SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSaveExercises);
            SaveEditedRoutineCommand = new Command(OnSaveRoutine);
            AddExerciseCommand = new Command(OnAddExercise);

            foreach (var exercise in routine.Exercises)
            {
                TakenExercises.Add(exercise.Name);
            }
        }


        private void OnAddSetToExercise(Exercise exercise)
        {
            if (exercise == null) return;
            Set set = new Set();
            set.ID = exercise.SetCount + 1;
            exercise.AddSet(set);
        }

        private void OnRemoveSetFromExercise(Exercise exercise)
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
        private async void OnAddExercise()
        {
            await Shell.Current.GoToAsync("editroutine/addexercise");
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

        private async void OnSaveRoutine()
        {
            if (Shell.Current.CurrentPage is ContentPage page)
                page.Focus();

            AppState.RemoveRoutine(AppState.CurrentRoutine);

            AppState.Routines.Add(routine);
            var template = DbHelper.ToDbRoutineTemplate(routine);
            App.Db.RoutineTemplates.Add(template);
            App.Db.SaveChanges();

            AppState.CurrentRoutineName = string.Empty;
            AppState.CurrentRoutine = null;

            FilterByCategory("All", false);

            await Shell.Current.Navigation.PopToRootAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
