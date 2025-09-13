using GymTracker.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public ICommand SaveRoutineCommand { get; }
        public ICommand AddExerciseCommand { get; }
        public ICommand RemoveSetFromExerciseCommand { get; }
        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<Exercise> DisplayedExercises { get; set; }
        public ICommand LoadMoreExercisesCommand { get; }
        public ICommand SelectExerciseCommand { get; }
        public ICommand SaveEditedRoutineCommand { get; }
        public ICommand SaveExercisesToWorkoutCommand { get; }
        public ObservableCollection<String> TakenExercises { get; set; } = new ObservableCollection<String>();

        public ICommand FilterCommand { get; }
        public EditRoutineModel()
        {
            DisplayedExercises = new ObservableCollection<Exercise>();
            AppState.MaxExercises = 20;
            AppState.FillDisplayedExercises(DisplayedExercises);
            LoadMoreExercisesCommand = new Command(() => { 
                AppState.MaxExercises += 20;
                AppState.FillDisplayedExercises(DisplayedExercises);
            });
            routine = AppState.EditedRoutine;
            TakenExercises = new ObservableCollection<string>(routine.Exercises.Select(e => e.Name));
            Categories = AppState.Categories;
            AppState.FilterByCategory("All", false);
            FilterCommand = new Command<Category>((Category cat) => { AppState.SelectCategory(cat); AppState.FillDisplayedExercises(DisplayedExercises); });
            RemoveSetFromExerciseCommand = new Command<Exercise>(OnRemoveSetFromExercise);
            SelectExerciseCommand = new Command<Exercise>(AppState.SelectExercise);
            SaveExercisesToWorkoutCommand = new Command(OnSaveExercises);
            SaveEditedRoutineCommand = new Command(OnSaveRoutine);
            AddExerciseCommand = new Command(OnAddExercise);

            foreach (var exercise in routine.Exercises)
            {
                TakenExercises.Add(exercise.Name);
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

        private void OnRemoveSetFromExercise(Exercise exercise)
        {
            if (exercise.Sets.Count > 0)
            {
                var setToRemove = exercise.Sets.LastOrDefault();
                if (setToRemove != null)
                {
                    exercise.RemoveSet(setToRemove.ID, setToRemove.Side);
                }
            }
        }
        private async void OnAddExercise()
        {
            await Shell.Current.GoToAsync("editroutine/addexercise");
        }

        public async Task OnRemoveExercise(Exercise exercise)
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

            await Shell.Current.Navigation.PopAsync();
        }

        private async void OnSaveRoutine()
        {
            if (Shell.Current.CurrentPage is ContentPage page)
                page.Focus();

            foreach (Exercise exercise in routine.Exercises)
            {
                if(exercise.Description != null)
                {
                    if (exercise.Description.Length > 200)
                    {
                        await Shell.Current.DisplayAlert("Error", "Exercise: " + exercise.Name + " has a description over 200 characters.", "OK");
                        return;
                    }
                }
            }

            AppState.RemoveRoutine(AppState.CurrentRoutine);

            AppState.Routines.Add(routine);
            var template = DbHelper.ToDbRoutineTemplate(routine);
            App.Db.RoutineTemplates.Add(template);
            App.Db.SaveChanges();

            AppState.CurrentRoutineName = string.Empty;
            AppState.CurrentRoutine = null;

            AppState.FilterByCategory("All", false);

            await Shell.Current.Navigation.PopToRootAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
