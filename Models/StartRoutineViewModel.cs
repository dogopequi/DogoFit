using GymTracker.Services;
using System;
using System.Collections.Generic;
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

        public StartRoutineViewModel()
        {
            routine = AppState.CurrentRoutine;
            AddSetToExerciseCommand = new Command<Exercise>(OnAddSetToExercise);
            RemoveSetFromExerciseCommand = new Command<Exercise>(OnRemoveSetFromExercise);
            FinishRoutineCommand = new Command(OnFinishRoutine);
            if (!AppState.WorkoutStarted == true)
            {
                routine.Start();
                AppState.WorkoutStarted = true;
            }
        }
        public Routine routine { get; set; }


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
        private async void OnFinishRoutine()
        {
            if (Shell.Current.CurrentPage is ContentPage page)
                page.Focus();

            if (routine == null) return;

            routine.Finish();
            AppState.AddRoutineToWorkouts(routine);
            AppState.WorkoutStarted = false;



            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}
