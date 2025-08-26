using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public static class AppState
    {
        public static string CurrentRoutineName { get; set; } = "";
        public static ObservableCollection<Routine> Workouts { get; set; } = new ObservableCollection<Routine>();
        public static ObservableCollection<Routine> Routines { get; set; } = new ObservableCollection<Routine>();
        public static HashSet<string> SelectedExerciseIds { get; set; } = new();
        public static Routine? EditedRoutine { get; set; } = new Routine();
        public static bool IsEditingRoutine { get; set; }
        public static Routine? CurrentRoutine { get; set; }
        public static bool IsWorkoutInProgress { get; set; } = false;
        public static Routine? WorkoutInProgress { get; set; } = null;
        public static ObservableCollection<Category> Categories { get; set; }
        public static ObservableCollection<Exercise> AllExercises { get; set; }
        public static ObservableCollection<Exercise> FilteredExercises { get; set; }

        public static ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();
        public static Profile Profile { get; set; }
        public static bool IsNewRoutine { get; set; }
        public static int RoutinesCount{ get; set; }


        public static void AddRoutineToWorkouts(Routine routine)
        {
            if (routine == null) return;

            var newRoutine = new Routine(routine);
            Workouts.Insert(0, newRoutine);
            RoutinesCount += 1;

            newRoutine.CalculateSetDistribution();
            Profile.Lats += newRoutine.Lats;
            Profile.Triceps += newRoutine.Triceps;
            Profile.Biceps += newRoutine.Biceps;
            Profile.Quadriceps += newRoutine.Quadriceps;
            Profile.Hamstrings += newRoutine.Hamstrings;
            Profile.Glutes += newRoutine.Glutes;
            Profile.Calves += newRoutine.Calves;
            Profile.Abdominals += newRoutine.Abdominals;
            Profile.Obliques += newRoutine.Obliques;
            Profile.Traps += newRoutine.Traps;
            Profile.LateralDelts += newRoutine.LateralDelts;
            Profile.FrontDelts += newRoutine.FrontDelts;
            Profile.RearDelts += newRoutine.RearDelts;
            Profile.Forearms += newRoutine.Forearms;
            Profile.Push += newRoutine.Push;
            Profile.Chest += newRoutine.Chest;
            Profile.Back += newRoutine.Back;
            Profile.Legs += newRoutine.Legs;
            Profile.Arms += newRoutine.Arms;
            Profile.Core += newRoutine.Core;
            Profile.Shoulders += newRoutine.Shoulders;
            Profile.Pull += newRoutine.Pull;
            Profile.Reps += newRoutine.RepCount;
            Profile.Volume += newRoutine.Volume;
            Profile.Sets += newRoutine.SetCount;
            Profile.Duration += newRoutine.Duration.TotalSeconds;
            Profile.Workouts += 1;

            var workout = DbHelper.ToDbWorkout(newRoutine);
            App.Db.Workouts.Add(workout);
            App.Db.SaveChanges();

            newRoutine.ID = workout.Id;

            DbHelper.SaveProfile(App.Db, Profile);
        }


        public static void RemoveRoutine(Routine routine)
        {
            if (routine == null) return;

            var dbTemplate = App.Db.RoutineTemplates
                .FirstOrDefault(t => t.Name.ToLower() == routine.Name.ToLower());

            if (dbTemplate != null)
            {
                App.Db.RoutineTemplates.Remove(dbTemplate);
                App.Db.SaveChanges();
            }

            var existingRoutine = AppState.Routines
                .FirstOrDefault(r => r.Name.Equals(routine.Name, StringComparison.OrdinalIgnoreCase));
            if (existingRoutine != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    AppState.Routines.Remove(existingRoutine);
                });
            }
        }

        public static void RemoveWorkout(Routine workout)
        {
            if (workout == null) return;

            var dbWorkout = App.Db.Workouts.FirstOrDefault(w => w.Id == workout.ID);
            if (dbWorkout != null)
            {
                App.Db.Workouts.Remove(dbWorkout);
                App.Db.SaveChanges();
            }

            var existingRoutine = AppState.Workouts.FirstOrDefault(r => r.ID == workout.ID);
            if (existingRoutine != null)
            {
                Profile.Lats -= existingRoutine.Lats;
                Profile.Triceps -= existingRoutine.Triceps;
                Profile.Biceps -= existingRoutine.Biceps;
                Profile.Quadriceps -= existingRoutine.Quadriceps;
                Profile.Hamstrings -= existingRoutine.Hamstrings;
                Profile.Glutes -= existingRoutine.Glutes;
                Profile.Calves -= existingRoutine.Calves;
                Profile.Abdominals -= existingRoutine.Abdominals;
                Profile.Obliques -= existingRoutine.Obliques;
                Profile.Traps -= existingRoutine.Traps;
                Profile.LateralDelts -= existingRoutine.LateralDelts;
                Profile.FrontDelts -= existingRoutine.FrontDelts;
                Profile.RearDelts -= existingRoutine.RearDelts;
                Profile.Forearms -= existingRoutine.Forearms;
                Profile.Push -= existingRoutine.Push;
                Profile.Chest -= existingRoutine.Chest;
                Profile.Back -= existingRoutine.Back;
                Profile.Legs -= existingRoutine.Legs;
                Profile.Arms -= existingRoutine.Arms;
                Profile.Core -= existingRoutine.Core;
                Profile.Shoulders -= existingRoutine.Shoulders;
                Profile.Pull -= existingRoutine.Pull;
                Profile.Reps -= existingRoutine.RepCount;
                Profile.Volume -= existingRoutine.Volume;
                Profile.Sets -= existingRoutine.SetCount;
                Profile.Duration -= existingRoutine.Duration.TotalSeconds;
                Profile.Workouts -= 1;
                AppState.Workouts.Remove(existingRoutine);
            }
            DbHelper.SaveProfile(App.Db, Profile);
        }



        public static bool ValidateRoutineName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            else if (name.Length > 20)
                return false;
            return !Routines.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void SaveWorkoutInProgress()
        {
            DbHelper.SaveWorkoutInProgress(App.Db, WorkoutInProgress);
        }

        public static void Init()
        {
            WorkoutInProgress = DbHelper.LoadWorkoutInProgress(App.Db);
            if(WorkoutInProgress != null)
            {
                IsWorkoutInProgress = true;
            }
            else
            {
                IsWorkoutInProgress = false;
            }
            EditedRoutine = null;
            Workouts = new ObservableCollection<Routine>(
                App.Db.Workouts
                    .OrderByDescending(w => w.Date)
                    .AsEnumerable()
                    .Select(w => DbHelper.FromDbWorkout(w))
                    .Where(r => r != null)
            );

            Routines = new ObservableCollection<Routine>(
                App.Db.RoutineTemplates
                    .AsEnumerable()
                    .Select(t => DbHelper.FromDbRoutineTemplate(t))
                    .Where(r => r != null)
            );
            RoutinesCount = 0;
            foreach (var r in Routines) {
                RoutinesCount += 1;
            }
            Profile = DbHelper.LoadProfile(App.Db);

            AllExercises = new ObservableCollection<Exercise>
            {

            new Exercise{ Name = "Bench Press (Barbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Bench Press (Dumbbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Incline Bench Press (Barbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Incline Bench Press (Dumbbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Decline Bench Press (Barbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Decline Bench Press (Dumbbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Chest Press (Machine)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Chest Press (Cable)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Fly (Cable)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Fly (Dumbbell)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Fly (Machine)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Chest Dip (Machine)", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Chest Dip", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },
            new Exercise{ Name = "Push-Up", Function = "Push", MuscleGroup = "Chest", TargetMuscle = "Chest", SecondaryMuscles = new List<string>{"Triceps", "Front Delts" } },

            new Exercise {Name = "Pull-Up", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Chin-Up", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Seated Row (Machine)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Seated Row (Cable)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "T Bar Row", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Bent Over Row", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Dumbbell Roww", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Lat Pulldown (Machine)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Lat Pulldown (Cable)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Close Grip Lat Pulldown (Machine)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Close Grip Lat Pulldown (Cable)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Behind The Neck Lat Pulldown", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Reverse Grip Lat Pulldown", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Lats", SecondaryMuscles = new List<string>{ "Biceps", "Rear Delts" } },
            new Exercise {Name = "Shrugs (Dumbbell)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise {Name = "Shrugs (Barbell)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise {Name = "Shrugs (Machine)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise {Name = "Shrugs (Cable)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise {Name = "Upright Row (Dumbbell)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Biceps", "Front Delts" } },
            new Exercise {Name = "Upright Row (Barbell)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Biceps", "Front Delts" } },
            new Exercise {Name = "Upright Row (Machine)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Biceps", "Front Delts" } },
            new Exercise {Name = "Upright Row (Cable)", Function = "Pull", MuscleGroup = "Back", TargetMuscle = "Traps", SecondaryMuscles = new List<string>{ "Biceps", "Front Delts" } },

            new Exercise {Name = "Back Extension", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Lower Back", SecondaryMuscles = new List<string>{ "Lower Back" } },
            new Exercise {Name = "Superman", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Lower Back", SecondaryMuscles = new List<string>{ "Lower Back" } },

            new Exercise { Name = "Bicep Curl (Barbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Bicep Curl (Dumbbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Bicep Curl (Machine)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Bicep Curl (Cable)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Reverse Bicep Curl (Barbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Reverse Bicep Curl (Dumbbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Reverse Bicep Curl (Machine)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Reverse Bicep Curl (Cable)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Hammer Curl (Barbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Hammer Curl (Dumbbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Hammer Curl (Machine)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Hammer Curl (Cable)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "EZ Bar Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Reverse EZ Bar Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Preacher Curl (Barbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Preacher Curl (Dumbbell)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Preacher Curl (Machine)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Preacher Curl (Cable)", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Overhead Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            new Exercise { Name = "Incline Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Biceps", SecondaryMuscles = new List<string>{ "Forearms" } },
            
            new Exercise { Name = "Behind the Back Wrist Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Forearms", SecondaryMuscles = new List<string>{ "Biceps" } },
            new Exercise { Name = "Seated Wrist Extension", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Forearms", SecondaryMuscles = new List<string>{ "Biceps" } },
            new Exercise { Name = "Seated Palms Up Wrist Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Forearms", SecondaryMuscles = new List<string>{ "Biceps" } },
            new Exercise { Name = "Wrist Roller", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Forearms", SecondaryMuscles = new List<string>{ "Biceps" } },
            new Exercise { Name = "Machine Wrist Curl", Function = "Pull", MuscleGroup = "Arms", TargetMuscle = "Forearms", SecondaryMuscles = new List<string>{ "Biceps" } },
            
            new Exercise { Name = "Arnold Press (Dumbbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },
            new Exercise { Name = "Arnold Press (Barbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },
            new Exercise { Name = "Reverse Fly (Machine)", Function = "Pull", MuscleGroup = "Shoulders", TargetMuscle = "Rear Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Reverse Fly (Cable)", Function = "Pull", MuscleGroup = "Shoulders", TargetMuscle = "Rear Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Reverse Fly (Dumbbell)", Function = "Pull", MuscleGroup = "Shoulders", TargetMuscle = "Rear Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Face Pull", Function = "Pull", MuscleGroup = "Shoulders", TargetMuscle = "Rear Delts", SecondaryMuscles = new List<string>{ "Traps", "Biceps" } },
            new Exercise { Name = "Lateral Raise (Machine)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Lateral Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Lateral Raise (Dumbbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Lateral Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Lateral Raise (Cable)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Lateral Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Front Raise (Machine)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Front Raise (Dumbbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Front Raise (Cable)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Traps" } },
            new Exercise { Name = "Shoulder Press (Dumbbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },
            new Exercise { Name = "Shoulder Press (Cable)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },
            new Exercise { Name = "Shoulder Press (Machine)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },
            new Exercise { Name = "Shoulder Press (Barbell)", Function = "Push", MuscleGroup = "Shoulders", TargetMuscle = "Front Delts", SecondaryMuscles = new List<string>{ "Triceps", "Lateral Delts", "Rear Delts" } },

            new Exercise { Name = "Plank", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques", "Lower Back" } },
            new Exercise { Name = "Crunch", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Ab Wheel", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques", "Lower Back" } },
            new Exercise { Name = "Bicycle Crunch", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Bicycle Crunch (Raised Legs)", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Crunch (Cable)", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Crunch (Machine)", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Decline Crunch", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Elbow to Knee", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Obliques" } },
            new Exercise { Name = "Hanging Knee Raise", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Hip Flexors" } },
            new Exercise { Name = "Hanging Leg Raise", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Hip Flexors" } },
            new Exercise { Name = "Leg Raise", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Hip Flexors" } },
            new Exercise { Name = "L-Sit", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Abdominals", SecondaryMuscles = new List<string>{ "Hip Flexors", "Shoulders" } },
            new Exercise { Name = "Russian Twist", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Obliques", SecondaryMuscles = new List<string>{ "Abdominals" } },
            new Exercise { Name = "Side Plank", Function = "Core", MuscleGroup = "Core", TargetMuscle = "Obliques", SecondaryMuscles = new List<string>{ "Abdominals", "Lower Back" } },
            
            new Exercise { Name = "Barbell Squat", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Quadriceps", SecondaryMuscles = new List<string>{ "Glutes", "Hamstrings", "Core" } },
            new Exercise { Name = "Goblet Squat", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Quadriceps", SecondaryMuscles = new List<string>{ "Glutes", "Hamstrings", "Core" } },
            new Exercise { Name = "Smith Machine Squat", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Quadriceps", SecondaryMuscles = new List<string>{ "Glutes", "Hamstrings" } },
            new Exercise { Name = "Leg Press", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Quadriceps", SecondaryMuscles = new List<string>{ "Glutes", "Hamstrings" } },
            new Exercise { Name = "Leg Extension", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Quadriceps", SecondaryMuscles = new List<string>() },
            new Exercise { Name = "Lying Leg Curl", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Calves" } },
            new Exercise { Name = "Seated Leg Curl", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Calves" } },
            new Exercise { Name = "Calf Press", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string>{ "Hamstrings" } },
            new Exercise { Name = "Deadlift", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Glutes", "Lower Back", "Core" } },
            new Exercise { Name = "Romanian Deadlift (Barbell)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Glutes", "Lower Back" } },
            new Exercise { Name = "Stiff-Legged Deadlift (Barbell)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Glutes", "Lower Back" } },
            new Exercise { Name = "Stiff-Legged Deadlift (Dumbbell)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Glutes", "Lower Back" } },
            new Exercise { Name = "Hip Thrust (Barbell)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Glutes", SecondaryMuscles = new List<string>{ "Hamstrings", "Core" } },
            new Exercise { Name = "Hip Thrust (Machine)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Glutes", SecondaryMuscles = new List<string>{ "Hamstrings", "Core" } },
            new Exercise { Name = "Romanian Deadlift (Dumbbell)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Hamstrings", SecondaryMuscles = new List<string>{ "Glutes", "Lower Back" } },
            
            new Exercise { Name = "Standing Calf Raise", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },
            new Exercise { Name = "Standing Calf Raise (Machine)", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },
            new Exercise { Name = "Seated Calf Raise", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },
            new Exercise { Name = "Donkey Calf Raise", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },
            new Exercise { Name = "Smith Machine Calf Raise", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },
            new Exercise { Name = "Leg Press Calf Raise", Function = "Legs", MuscleGroup = "Legs", TargetMuscle = "Calves", SecondaryMuscles = new List<string> { "" } },


            new Exercise { Name = "Tricep Pushdown (Cable)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Tricep Pushdown (Machine)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Bench Dip", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Chest", "Front Delts" } },
            new Exercise { Name = "Skullcrusher (Barbell)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Skullcrusher (Dumbbell)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Overhead Extension (Barbell)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Overhead Extension (Dumbbell)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Overhead Extension (Machine)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Overhead Extension (Cable)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Kickback (Dumbbell)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Kickback (Machine)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Kickback (Cable)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Front Delts" } },
            new Exercise { Name = "Close Grip Bench Press", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Chest", "Front Delts" } },
            new Exercise { Name = "Triceps Dip (Machine)", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Chest", "Front Delts" } },
            new Exercise { Name = "Triceps Dip", Function = "Push", MuscleGroup = "Arms", TargetMuscle = "Triceps", SecondaryMuscles = new List<string>{ "Chest", "Front Delts" } },
            };

            Categories = new ObservableCollection<Category>
            {
                new Category { Name = "All", IsSelected = true },
                new Category { Name = "Push" },
                new Category { Name = "Pull" },
                new Category { Name = "Biceps" },
                new Category { Name = "Triceps" },
                new Category { Name = "Chest" },
                new Category { Name = "Back" },
                new Category { Name = "Shoulders" },
                new Category { Name = "Legs" },
                new Category { Name = "Core" },
                new Category { Name = "Arms" },
                new Category { Name = "Obliques" },
                new Category { Name = "Front Delts" },
                new Category { Name = "Rear Delts" },
                new Category { Name = "Lateral Delts" },
                new Category { Name = "Quadriceps" },
                new Category { Name = "Hamstrings" },
                new Category { Name = "Glutes" },
                new Category { Name = "Traps" },
                new Category { Name = "Abdominals" },
                new Category { Name = "Calves" },
                new Category { Name = "Lats" },
                new Category { Name = "Forearms" }
            };

            FilteredExercises = new ObservableCollection<Exercise>(AllExercises.Select(e => new Exercise(e)));

        }
    }
}
