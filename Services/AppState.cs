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
        public static Routine EditedRoutine { get; set; }
        public static Routine CurrentRoutine { get; set; } = new Routine();
        public static bool WorkoutStarted { get; set; } = false;
        public static ObservableCollection<Category> Categories { get; set; }
        public static ObservableCollection<Exercise> AllExercises { get; set; }
        public static ObservableCollection<Exercise> FilteredExercises { get; set; }

        public static ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();
        public static Profile Profile { get; set; }

        public static int RoutinesCount{ get; set; }

        public static void AddRoutineToWorkouts(Routine routine)
        {
            if (routine == null) return;
            var newRoutine = new Routine(routine);
            Workouts.Insert(0, newRoutine);
            RoutinesCount += 1;

            newRoutine.CalculateSetDistribuition();

            Profile.Push += newRoutine.Push;
            Profile.Chest += newRoutine.Chest;
            Profile.Back += newRoutine.Back;
            Profile.Legs += newRoutine.Legs;
            Profile.Arms += newRoutine.Arms;
            Profile.Core += newRoutine.Core;
            Profile.Shoulders += newRoutine.Shoulders;
            Profile.Pull += newRoutine.Pull;
            Profile.Reps += routine.RepCount;
            Profile.Volume += routine.Volume;
            Profile.Sets += routine.SetCount;
            Profile.Duration += routine.Duration.TotalSeconds;
            Profile.Workouts += 1;

            var workout = DbHelper.ToDbWorkout(newRoutine);
            App.Db.Workouts.Add(workout);
            App.Db.SaveChanges();
            DbHelper.SaveProfile(App.Db, Profile);


        }

        public static void RemoveWorkout(Routine routine)
        {
            if (routine == null) return;

            var dbWorkout = App.Db.Workouts
                .AsEnumerable()
                .FirstOrDefault(w => JsonSerializer.Deserialize<Routine>(w.JsonRoutine)?.Name == routine.Name);

            if (dbWorkout != null)
            {
                App.Db.Workouts.Remove(dbWorkout);
                App.Db.SaveChanges();
            }

            var existingRoutine = Workouts.FirstOrDefault(r => r.Name.Equals(routine.Name, StringComparison.OrdinalIgnoreCase));
            if (existingRoutine != null)
            {
                Workouts.Remove(existingRoutine);
                RoutinesCount -= 1;
            }
        }


        public static bool ValidateRoutineName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            else if (name.Length > 20)
                return false;
            return !Routines.Any(r => r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public static void Init()
        {
            EditedRoutine = null;
            RoutinesCount = 0;
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
            new Exercise { Name = "Barbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Incline Barbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Decline Barbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Dumbbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Incline Dumbbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Decline Dumbbell Bench Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Machine Chest Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Cable Chest Press", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Cable Fly", Categories = new List<string>{ "Chest", "Push" } },
            new Exercise { Name = "Dumbbell Fly", Categories = new List<string>{ "Chest", "Push" } },
            new Exercise { Name = "Machine Fly", Categories = new List<string>{ "Chest", "Push" } },
            new Exercise { Name = "Dip", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },
            new Exercise { Name = "Push-Up", Categories = new List<string>{ "Chest", "Triceps", "Arms", "Push" } },

            new Exercise { Name = "Pull-Up", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Chin-Up", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Machine Seated Row", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Cable Seated Row", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "T Bar Row", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Bent Over Row", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Bench Dumbbell Row", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Machine Lat Pulldown", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Cable Lat Pulldown", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Close Grip Lat Pulldown", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Behind The Neck Lat Pulldown", Categories = new List<string>{ "Back", "Biceps", "Arms", "Pull" } },
            new Exercise { Name = "Reverse Grip Lat Pulldown", Categories = new List<string>{ "Back", "Biceps" , "Arms", "Pull"} },
            new Exercise { Name = "Barbell Shrugs", Categories = new List<string>{ "Back", "Pull" } },
            new Exercise { Name = "Dumbbell Shrugs", Categories = new List<string>{ "Back", "Pull" } },
            new Exercise { Name = "Cable Upright Row", Categories = new List<string>{ "Back" , "Shoulders", "Pull" } },
            new Exercise { Name = "Machine Upright Row", Categories = new List<string>{ "Back", "Shoulders", "Pull" } },
            new Exercise { Name = "Dumbbell Upright Row", Categories = new List<string>{"Back" , "Shoulders", "Pull" } },
            new Exercise { Name = "Barbell Upright Row", Categories = new List<string>{ "Back" , "Shoulders", "Pull"} },

            new Exercise { Name = "Back Extension", Categories = new List<string>{"Core", "Back" } },
            new Exercise { Name = "Superman", Categories = new List<string>{"Core", "Back" } },

            new Exercise { Name = "Barbell Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Dumbbell Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Cable Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Machine Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Reverse Barbell Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Reverse Dumbbell Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Reverse Cable Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Reverse Machine Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Barbell Hammer Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull", "Forearm" } },
            new Exercise { Name = "Dumbbell Hammer Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull", "Forearm" } },
            new Exercise { Name = "Cable Hammer Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull", "Forearm" } },
            new Exercise { Name = "Machine Hammer Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull", "Forearm" } },
            new Exercise { Name = "EZ Bar Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Reverse EZ Bar Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Barbell Preacher Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Dumbbell Preacher Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Cable Preacher Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Machine Preacher Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Overhead Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },
            new Exercise { Name = "Incline Bicep Curl", Categories = new List<string>{ "Arms", "Biceps", "Pull" } },

            new Exercise { Name = "Behind the Back Bicep Wrist Curl", Categories = new List<string>{ "Arms", "Forearm", "Pull"} },
            new Exercise { Name = "Seated Wrist Extension", Categories = new List<string>{ "Arms", "Forearm", "Push" } },
            new Exercise { Name = "Seated Palms Up Wrist Curl", Categories = new List<string>{ "Arms", "Forearm", "Pull" } },
            new Exercise { Name = "Wrist Roller", Categories = new List<string>{ "Arms", "Forearm", "Push" } },
            new Exercise { Name = "Machine Wrist Curl", Categories = new List<string>{ "Arms", "Forearm", "Pull" } },

            new Exercise { Name = "Dumbbell Arnold Press", Categories = new List<string>{ "Shoulders", "Triceps", "Push"} },
            new Exercise { Name = "Barbell Arnold Press", Categories = new List<string>{ "Shoulders", "Triceps", "Push"} },
            new Exercise { Name = "Cable Reverse Fly", Categories = new List<string>{ "Shoulders", "Pull" } },
            new Exercise { Name = "Machine Reverse Fly", Categories = new List<string>{ "Shoulders", "Pull" } },
            new Exercise { Name = "Dumbbell Reverse Fly", Categories = new List<string>{ "Shoulders", "Pull" } },
            new Exercise { Name = "Facepull", Categories = new List<string>{ "Shoulders", "Pull" } },
            new Exercise { Name = "Dumbbell Lateral Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Cable Lateral Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Machine Lateral Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Dumbbell Front Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Cable Front Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Machine Front Raise", Categories = new List<string>{ "Shoulders", "Push" } },
            new Exercise { Name = "Dumbbell Shoulder Press", Categories = new List<string>{ "Shoulders", "Triceps", "Push" } },
            new Exercise { Name = "Barbell Shoulder Press", Categories = new List<string>{ "Shoulders" , "Triceps", "Push" } },
            new Exercise { Name = "Machine Shoulder Press", Categories = new List<string>{ "Shoulders", "Triceps", "Push" } },
            new Exercise { Name = "Cable Shoulder Press", Categories = new List<string>{ "Shoulders", "Triceps", "Push" } },

            new Exercise { Name = "Plank", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Crunch", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Ab Wheel", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Bicycle Crunch", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Bicycle Crunch Raised Legs", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Cable Crunch", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Machine Crunch", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Decline Crunch", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Elbow To Knee", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Hanging Knee Raise", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Hanging Leg Raise", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Leg Raise", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "L-Sit", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Russian Twist", Categories = new List<string>{ "Core" } },
            new Exercise { Name = "Side Plank", Categories = new List<string>{ "Core" } },

            new Exercise { Name = "Barbell Squat", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Goblet Squat", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Smith Machine Barbell Squat", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Leg Press", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Leg Extension", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Lying Leg Curl", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Seated Leg Curl", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Calf Press", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Deadlift", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Barbell Romanian Deadlift", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Stiff Legged Deadlift", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Barbell Hip Thrust", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Machine Hip Thrust", Categories = new List<string>{ "Legs" } },
            new Exercise { Name = "Dumbbell Romanian Deadlift", Categories = new List<string>{ "Legs" } },

            new Exercise { Name = "Tricep Pushdown", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Bench Dip", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Barbell Skullcrusher", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Dumbbell Skullcrusher", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Overhead Barbell Extension", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Overhead Dumbbell Extension", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Overhead Machine Extension", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Overhead Cable Extension", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Barbell Pushdown", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Dumbbell Pushdown", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Machine Pushdown", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Cable Pushdown", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Cable Kickback", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Dumbbell Kickback", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Close Grip Bench Press", Categories = new List<string>{ "Arms", "Triceps", "Push" } },
            new Exercise { Name = "Machine Dip", Categories = new List<string>{ "Arms", "Triceps", "Push" } }
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
                new Category { Name = "Forearm" }
            };

            FilteredExercises = new ObservableCollection<Exercise>(AllExercises.Select(e => new Exercise(e)));

        }
    }
}
