using GymTracker.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymTracker.Models
{
    internal class WorkoutHistoryModel
    {
        public ObservableCollection<Routine> routines { get; set; }

        public ICommand RemoveWorkoutCommand { get; }
        public Services.Profile Profile => AppState.Profile;
        public WorkoutHistoryModel() 
        {
            routines = AppState.Workouts;
            RemoveWorkoutCommand = new Command<Routine>(OnDeleteWorkout);
        }
        private async void OnDeleteWorkout(Routine routine)
        {
            var result = await Shell.Current.DisplayAlert("Delete Workout", $"Are you sure you want to delete the workout '{routine.Name}'?", "Yes", "No");
            if (result)
            {
                AppState.RemoveWorkout(routine);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
