using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;

namespace GymTracker.Services
{

    public enum ProfileStats
    {
        Empty, MuscleGroup, MuscleFunction, IndividualMuscle, MonthlyReport, Exercise, Weekly
    }
    public enum ProfileExercise
    {
        Empty, Add, Edit
    }
    public enum MuscleFunctions
    {
        Push, Pull, Legs, Core
    }
    public enum WorkoutStates
    {
       None, NewRoutine, EditingRoutine, EmptyWorkout, WorkoutInProgress, StartRoutine
    }
    public enum MuscleGroups
    {
        Legs, Chest, Back, Core, Arms, Shoulders
    }
    public enum Muscles
    {
        Biceps, Triceps, Quadriceps, Hamstrings, Glutes, Calves, Abdominals, Obliques, Traps, LateralDelts, RearDelts, FrontDelts, Forearms, Lats, Chest, LowerBack
    }
    public static class AppState
    {
        public static Muscles[] MusclesList = new Muscles[] { Muscles.Biceps, Muscles.Triceps, Muscles.Quadriceps, Services.Muscles.Hamstrings,
            Muscles.Glutes, Muscles.Calves, Muscles.Abdominals, Muscles.Obliques, Muscles.Traps, Services.Muscles.RearDelts,
            Muscles.FrontDelts, Muscles.Forearms, Muscles.Lats, Muscles.Chest,Muscles.LowerBack };

        public static MuscleGroups[] MuscleGroupsList = new MuscleGroups[] { MuscleGroups.Legs, MuscleGroups.Chest, MuscleGroups.Back,
            MuscleGroups.Core,MuscleGroups.Arms,MuscleGroups.Shoulders};
        public static MuscleFunctions[] MuscleFunctionsList = new MuscleFunctions[] { MuscleFunctions.Pull, MuscleFunctions.Push, MuscleFunctions.Core, MuscleFunctions.Legs};
        public static ProfileStats profileStat { get; set; }
        public static WorkoutStates WorkoutState { get; set; }
        public static ProfileExercise profileExercise { get; set; }
        public static string CurrentRoutineName { get; set; } = "";
        public static ObservableCollection<Routine> Workouts { get; set; } = new ObservableCollection<Routine>();
        public static ObservableCollection<Routine> Routines { get; set; } = new ObservableCollection<Routine>();
        public static HashSet<string> SelectedExerciseIds { get; set; } = new();
        public static Routine? EditedRoutine { get; set; } = new Routine();
        public static Routine? CurrentRoutine { get; set; }
        public static Routine? WorkoutInProgress { get; set; } = null;
        public static ObservableCollection<Category> Categories { get; set; } = new ObservableCollection<Category>();
        public static ObservableCollection<Exercise> AllExercises { get; set; } = new ObservableCollection<Exercise>();
        public static ObservableCollection<Exercise> FilteredExercises { get; set; } = new ObservableCollection<Exercise>();
        public static Exercise SelectedExercise { get; set; }
        public static ObservableCollection<Exercise> SelectedExercises { get; set; } = new ObservableCollection<Exercise>();
        public static Profile Profile { get; set; }
        public static int RoutinesCount{ get; set; }
        public static Exercise EditedExercise { get; set; }
        public static int MaxExercises { get; set; }
        public static Set? GetLastSetForExercise(string exerciseName, int setID, SideType setSide)
        {
            return Workouts
                .OrderByDescending(w => w.StartTime)
                .SelectMany(w => w.Exercises)
                .Where(e => e.Name == exerciseName)
                .SelectMany(e => e.Sets)
                .Where(s => s.ID == setID && s.Side == setSide)
                .Where(s => s.IsChecked)
                .FirstOrDefault();
        }

        public static void RecalculateWeights()
        {
            foreach (var routine in Workouts)
            {
                double routineVolume = 0;
                foreach (var exercise in routine.Exercises)
                {
                    foreach (var set in exercise.Sets)
                    {
                        if (set != null && set.IsChecked)
                        {
                            set.Weight = AppState.Profile.UseMetric ? LbsToKg(set.Weight) : KgToLbs(set.Weight);
                        }
                    }
                }
                DbHelper.UpdateWorkout(routine, routine.ID);
            }

            foreach (var routine in Routines)
            {
                foreach (var exercise in routine.Exercises)
                {
                    foreach (var set in exercise.Sets)
                    {
                        if (set != null)
                            set.Weight = AppState.Profile.UseMetric ? LbsToKg(set.Weight) : KgToLbs(set.Weight);
                    }
                }

                DbHelper.UpdateRoutineTemplate(routine, routine.ID);
            }

            App.Db.SaveChanges();
            DbHelper.SaveProfile(App.Db, Profile);
            App.Db.SaveChanges();
        }


        public static double LbsToKg(double lbs)
        {
            return lbs * 0.45359237;
        }

        public static double KgToLbs(double kg)
        {
            return kg / 0.45359237;
        }
        public static void AddRoutineToWorkouts(Routine routine)
        {
            if (routine == null) return;

            var newRoutine = new Routine(routine, true);
            Workouts.Insert(0, newRoutine);

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
            return true;
        }

        public static void SaveWorkoutInProgress()
        {
            DbHelper.SaveWorkoutInProgress(App.Db, WorkoutInProgress);
        }

        public static void Init()
        {
            WorkoutState = WorkoutStates.None;
            profileStat = ProfileStats.Empty;
            Profile = DbHelper.LoadProfile(App.Db);
            profileExercise = ProfileExercise.Empty;
            WorkoutInProgress = DbHelper.LoadWorkoutInProgress(App.Db);
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

            AllExercises = new ObservableCollection<Exercise>(
                App.Db.Exercises
                    .AsEnumerable()
                    .Select(t => DbHelper.FromDbExercise(t))
                    .Where(e => e != null)
            );

            if(AllExercises.Count <= 0)
            {
                AllExercises = new ObservableCollection<Exercise>
            {
                new Exercise{ Name = "Bench Press (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Bench Press (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise{ Name = "Incline Bench Press (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Incline Bench Press (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise{ Name = "Decline Bench Press (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Decline Bench Press (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise{ Name = "Chest Press (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Chest Press (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Fly (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Fly (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise{ Name = "Fly (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Chest Dip (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Chest Dip", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise{ Name = "Push-Up", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Chest, TargetMuscle = Muscles.Chest, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.FrontDelts}, IsUnilateral = false},

                new Exercise {Name = "Pull-Up", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Chin-Up", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Seated Row (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Seated Row (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "T Bar Row", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Bent Over Row", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Dumbbell Roww", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Lat Pulldown (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Lat Pulldown (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Close Grip Lat Pulldown (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Close Grip Lat Pulldown (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Behind The Neck Lat Pulldown", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Reverse Grip Lat Pulldown", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Lats, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise {Name = "Shrugs (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = true},
                new Exercise {Name = "Shrugs (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise {Name = "Shrugs (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise {Name = "Shrugs (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise {Name = "Upright Row (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise {Name = "Upright Row (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise {Name = "Upright Row (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise {Name = "Upright Row (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Back, TargetMuscle = Muscles.Traps, SecondaryMuscles = new List<Muscles>{Muscles.Biceps, Muscles.FrontDelts}, IsUnilateral = false},

                new Exercise {Name = "Back Extension", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.LowerBack, SecondaryMuscles = new List<Muscles>{Muscles.LowerBack}, IsUnilateral = false},
                new Exercise {Name = "Superman", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.LowerBack, SecondaryMuscles = new List<Muscles>{Muscles.LowerBack}, IsUnilateral = false},

                new Exercise { Name = "Bicep Curl (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Bicep Curl (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = true},
                new Exercise { Name = "Bicep Curl (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Bicep Curl (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Reverse Bicep Curl (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Reverse Bicep Curl (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = true},
                new Exercise { Name = "Reverse Bicep Curl (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Reverse Bicep Curl (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Hammer Curl (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Hammer Curl (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = true},
                new Exercise { Name = "Hammer Curl (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Hammer Curl (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "EZ Bar Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Reverse EZ Bar Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Preacher Curl (Barbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Preacher Curl (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = true},
                new Exercise { Name = "Preacher Curl (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Preacher Curl (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Overhead Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},
                new Exercise { Name = "Incline Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Biceps, SecondaryMuscles = new List<Muscles>{Muscles.Forearms}, IsUnilateral = false},

                new Exercise { Name = "Behind the Back Wrist Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Forearms, SecondaryMuscles = new List<Muscles>{Muscles.Biceps}, IsUnilateral = false},
                new Exercise { Name = "Seated Wrist Extension", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Forearms, SecondaryMuscles = new List<Muscles>{Muscles.Biceps}, IsUnilateral = false},
                new Exercise { Name = "Seated Palms Up Wrist Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Forearms, SecondaryMuscles = new List<Muscles>{Muscles.Biceps}, IsUnilateral = false},
                new Exercise { Name = "Wrist Roller", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Forearms, SecondaryMuscles = new List<Muscles>{Muscles.Biceps}, IsUnilateral = false},
                new Exercise { Name = "Machine Wrist Curl", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Forearms, SecondaryMuscles = new List<Muscles>{Muscles.Biceps}, IsUnilateral = false},

                new Exercise { Name = "Arnold Press (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = true},
                new Exercise { Name = "Arnold Press (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise { Name = "Reverse Fly (Machine)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.RearDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Reverse Fly (Cable)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.RearDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Reverse Fly (Dumbbell)", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.RearDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = true},
                new Exercise { Name = "Face Pull", Function = MuscleFunctions.Pull, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.RearDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps, Muscles.Biceps}, IsUnilateral = false},
                new Exercise { Name = "Lateral Raise (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.LateralDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Lateral Raise (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.LateralDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = true},
                new Exercise { Name = "Lateral Raise (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.LateralDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Front Raise (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Front Raise (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = true},
                new Exercise { Name = "Front Raise (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Traps}, IsUnilateral = false},
                new Exercise { Name = "Shoulder Press (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = true},
                new Exercise { Name = "Shoulder Press (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise { Name = "Shoulder Press (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = false},
                new Exercise { Name = "Shoulder Press (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Shoulders, TargetMuscle = Muscles.FrontDelts, SecondaryMuscles = new List<Muscles>{Muscles.Triceps, Muscles.LateralDelts, Muscles.RearDelts}, IsUnilateral = false},

                new Exercise { Name = "Plank", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques, Muscles.LowerBack}, IsUnilateral = false},
                new Exercise { Name = "Crunch", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Ab Wheel", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques, Muscles.LowerBack}, IsUnilateral = false},
                new Exercise { Name = "Bicycle Crunch", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Bicycle Crunch (Raised Legs)", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Crunch (Cable)", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Crunch (Machine)", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Decline Crunch", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Elbow to Knee", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Hanging Knee Raise", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Hanging Leg Raise", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Leg Raise", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "L-Sit", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Abdominals, SecondaryMuscles = new List<Muscles>{Muscles.Obliques}, IsUnilateral = false},
                new Exercise { Name = "Russian Twist", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Obliques, SecondaryMuscles = new List<Muscles>{Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Side Plank", Function = MuscleFunctions.Core, MuscleGroup = MuscleGroups.Core, TargetMuscle = Muscles.Obliques, SecondaryMuscles = new List<Muscles>{Muscles.Abdominals, Muscles.LowerBack}, IsUnilateral = false},

                new Exercise { Name = "Barbell Squat", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Quadriceps, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.Hamstrings, Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Goblet Squat", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Quadriceps, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.Hamstrings, Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Smith Machine Squat", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Quadriceps, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.Hamstrings}, IsUnilateral = false},
                new Exercise { Name = "Leg Press", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Quadriceps, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.Hamstrings}, IsUnilateral = false},
                new Exercise { Name = "Leg Extension", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Quadriceps, SecondaryMuscles =  new List<Muscles>{Muscles.Glutes }, IsUnilateral = false},
                new Exercise { Name = "Lying Leg Curl", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Calves}, IsUnilateral = false},
                new Exercise { Name = "Seated Leg Curl", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Calves}, IsUnilateral = false},
                new Exercise { Name = "Calf Press", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = new List<Muscles>{Muscles.Hamstrings}, IsUnilateral = false},
                new Exercise { Name = "Deadlift", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.LowerBack, Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Romanian Deadlift (Barbell)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.LowerBack}, IsUnilateral = false},
                new Exercise { Name = "Stiff-Legged Deadlift (Barbell)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.LowerBack}, IsUnilateral = false},
                new Exercise { Name = "Stiff-Legged Deadlift (Dumbbell)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.LowerBack}, IsUnilateral = true},
                new Exercise { Name = "Hip Thrust (Barbell)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Glutes, SecondaryMuscles = new List<Muscles>{Muscles.Hamstrings, Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Hip Thrust (Machine)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Glutes, SecondaryMuscles = new List<Muscles>{Muscles.Hamstrings, Muscles.Abdominals}, IsUnilateral = false},
                new Exercise { Name = "Romanian Deadlift (Dumbbell)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Hamstrings, SecondaryMuscles = new List<Muscles>{Muscles.Glutes, Muscles.LowerBack}, IsUnilateral = true},

                new Exercise { Name = "Standing Calf Raise", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },
                new Exercise { Name = "Standing Calf Raise (Machine)", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },
                new Exercise { Name = "Seated Calf Raise", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },
                new Exercise { Name = "Donkey Calf Raise", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },
                new Exercise { Name = "Smith Machine Calf Raise", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },
                new Exercise { Name = "Leg Press Calf Raise", Function = MuscleFunctions.Legs, MuscleGroup = MuscleGroups.Legs, TargetMuscle = Muscles.Calves, SecondaryMuscles = null },


                new Exercise { Name = "Tricep Pushdown (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Tricep Pushdown (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Bench Dip", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.Chest, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Skullcrusher (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Skullcrusher (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise { Name = "Overhead Extension (Barbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Overhead Extension (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise { Name = "Overhead Extension (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Overhead Extension (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Kickback (Dumbbell)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = true},
                new Exercise { Name = "Kickback (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Kickback (Cable)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Close Grip Bench Press", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.Chest, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Triceps Dip (Machine)", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.Chest, Muscles.FrontDelts}, IsUnilateral = false},
                new Exercise { Name = "Triceps Dip", Function = MuscleFunctions.Push, MuscleGroup = MuscleGroups.Arms, TargetMuscle = Muscles.Triceps, SecondaryMuscles = new List<Muscles>{Muscles.Chest, Muscles.FrontDelts}, IsUnilateral = false},
                };
                foreach(Exercise e in AllExercises)
                {
                    var dbexercise = DbHelper.ToDbExercise(e);
                    App.Db.Exercises.Add(dbexercise);
                    App.Db.SaveChanges();
                }
            }
            Categories = new ObservableCollection<Category>();
            Categories.Add(new Category { Name = "All", IsSelected = true });
            foreach (var mf in MuscleFunctionsList)
                Categories.Add(new Category { Name = MuscleFunctionToString(mf), IsSelected = false });
            foreach (var mg in MuscleGroupsList)
                Categories.Add(new Category { Name = MuscleGroupToString(mg), IsSelected = false });
            foreach (var m in MusclesList)
                Categories.Add(new Category { Name = MuscleToString(m), IsSelected = false });
            Categories = new ObservableCollection<Category>(Categories.GroupBy(c => c.Name).Select(g => g.First()));
            FilteredExercises = new ObservableCollection<Exercise>(AllExercises.Select(e => new Exercise(e)));

           //TestWorkouts();

        }

        private static void TestWorkouts()
        {
            var rnd = new Random();
            var now = DateTime.Now;

            for (int i = 0; i < 50; i++)
            {
                Routine workout = new Routine();
                workout.Name = $"Test Workout {i + 1}";

                DateTime start;
                double roll = rnd.NextDouble();

                if (roll < 0.25)
                {
                    int day = rnd.Next(1, DateTime.DaysInMonth(now.Year, now.Month) + 1);
                    int hour = rnd.Next(0, 24);
                    int minute = rnd.Next(0, 60);

                    start = new DateTime(now.Year, now.Month, day, hour, minute, 0);
                }
                else if (roll < 0.50)
                {
                    DateTime lastMonth = now.AddMonths(-1);
                    int day = rnd.Next(1, DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month) + 1);
                    int hour = rnd.Next(0, 24);
                    int minute = rnd.Next(0, 60);

                    start = new DateTime(lastMonth.Year, lastMonth.Month, day, hour, minute, 0);
                }
                else
                {
                    int daysBack = rnd.Next(0, 3 * 365);
                    int hours = rnd.Next(0, 24);
                    int minutes = rnd.Next(0, 60);

                    start = now.AddDays(-daysBack).AddHours(-hours).AddMinutes(-minutes);
                }

                workout.StartTime = start;
                workout.Duration = TimeSpan.FromMinutes(rnd.Next(30, 121));
                workout.EndTime = workout.StartTime + workout.Duration;
                int exerciseCount = rnd.Next(3, 8);
                for (int e = 0; e < exerciseCount; e++)
                {
                    var exerciseTemplate = AllExercises[rnd.Next(AllExercises.Count)];
                    Exercise exercise = new Exercise(exerciseTemplate);
                    
                    int setCount = rnd.Next(3, 6);
                    
                    for (int s = 0; s < setCount; s++)
                    {
                        if (exerciseTemplate.IsUnilateral)
                        {
                            exercise.AddSet(new Set
                            {
                                ID = s * 2,
                                IsChecked = true,
                                Reps = rnd.Next(5, 16),
                                Weight = Math.Round(rnd.NextDouble() * 100, 1),
                                Type = SetType.Normal,
                                Side = SideType.Left
                            });
                            exercise.AddSet(new Set
                            {
                                ID = s * 2 + 1,
                                IsChecked = true,
                                Reps = rnd.Next(5, 16),
                                Weight = Math.Round(rnd.NextDouble() * 100, 1),
                                Type = SetType.Normal,
                                Side = SideType.Right
                            });
                        }
                        else
                        {
                            exercise.AddSet(new Set
                            {
                                ID = s,
                                IsChecked = true,
                                Reps = rnd.Next(5, 16),
                                Weight = Math.Round(rnd.NextDouble() * 100, 1),
                                Type = SetType.Normal,
                                Side = SideType.None
                            });
                        }
                    }


                    workout.AddExercise(exercise);
                }

                AddRoutineToWorkouts(workout);
            }
        }

        public static String MuscleFunctionToString(MuscleFunctions f)
        {
            return f.ToString();
        }
        public static String MuscleGroupToString(MuscleGroups g)
        {
            return g.ToString();
        }
        public static String MuscleToString(Muscles m)
        {
            switch(m)
            {
                case Muscles.RearDelts:
                    return "Rear Delts";
                case Muscles.FrontDelts:
                    return "Front Delts";
                case Muscles.LateralDelts:
                    return "Lateral Delts";
                case Muscles.LowerBack:
                    return "Lower Back";
                default:
                    return m.ToString();
            }
        }

        public static string MGMToString(object e)
        {
            switch (e)
            {
                case MuscleFunctions f:
                    return MuscleFunctionToString(f);
                case MuscleGroups g:
                    return MuscleGroupToString(g);
                case Muscles m:
                    return MuscleToString(m);
                default:
                    return string.Empty;
            }
        }


        public static void SelectCategory(Category category)
        {
            foreach (var c in Categories)
                c.IsSelected = false;

            category.IsSelected = true;

            FilterByCategory(category.Name, false);
        }

        public static void ClearExerciseFilters()
        {
            foreach (var exercise in AppState.AllExercises)
            {
                exercise.IsSelected = false;
                exercise.Description = string.Empty;
            }
            foreach (var exercise in AppState.FilteredExercises)
            {
                exercise.IsSelected = false;
                exercise.Description = string.Empty;
            }
            AppState.FilteredExercises.Clear();
            AppState.SelectedExercises.Clear();
            AppState.SelectedExerciseIds.Clear();
        }

        public static void FilterByCategory(String category, bool useName)
        {
            FilteredExercises.Clear();
        
            IEnumerable<Exercise> filtered;
        
            if (!useName)
            {
                if (category.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    filtered = AllExercises;
                }
                else if (AppState.MuscleFunctionsList.Any(mf => AppState.MuscleFunctionToString(mf) == category))
                {
                    filtered = AllExercises.Where(e => AppState.MuscleFunctionToString(e.Function) == category);
                }
                else if (AppState.MuscleGroupsList.Any(mg => AppState.MuscleGroupToString(mg) == category))
                {
                    filtered = AllExercises.Where(e => AppState.MuscleGroupToString(e.MuscleGroup) == category);
                }
                else if (AppState.MusclesList.Any(m => AppState.MuscleToString(m) == category))
                {
                    filtered = AllExercises.Where(e => AppState.MuscleToString(e.TargetMuscle) == category);
                }
                else
                {
                    filtered = Enumerable.Empty<Exercise>();
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
                clone.IsSelected = AppState.SelectedExerciseIds.Contains(ex.Name);
                FilteredExercises.Add(clone);
            }
        }

        public static void OnAddSetToExercise(Exercise exercise, SideType type)
        {
            if (exercise == null) return;
            Set set = new Set();
            set.Side = type;
            exercise.AddSet(set);
            exercise.RecalculateSetIndexes();
            if(WorkoutState == WorkoutStates.WorkoutInProgress)
                AppState.SaveWorkoutInProgress();
        }
        public static void SelectExercise(Exercise exercise)
        {
            exercise.IsSelected = !exercise.IsSelected;

            if (exercise.IsSelected)
            {
                if (!AppState.SelectedExercises.Any(e => e.Name == exercise.Name))
                {
                    AppState.SelectedExercises.Add(new Exercise(exercise));
                    AppState.SelectedExerciseIds.Add(exercise.Name);
                }
            }
            else
            {
                var toRemove = AppState.SelectedExercises.FirstOrDefault(e => e.Name == exercise.Name);
                if (toRemove != null)
                    AppState.SelectedExercises.Remove(toRemove);

                AppState.SelectedExerciseIds.Remove(exercise.Name);
            }
        }
        public static string RemoveExercises(ObservableCollection<Exercise> exercises)
        {
            string error = "";
            if (AllExercises == null)
            {
                error += "DogoFit's Exercise List is empty.\n";
                return error;
            }
            if(exercises == null)
            {
                error += "No exercises have been selected.\n";
                return error;
            }
            foreach (Exercise exercise in exercises)
            {
                Exercise e = AllExercises.FirstOrDefault(x => string.Equals(x.Name, exercise.Name, StringComparison.OrdinalIgnoreCase));
        
                if (e == null)
                {
                    error += $"Exercise with name: {exercise.Name} doesn't exist in DogoFit's Exercise List.\n";
                }
                else
                {
                    DbHelper.RemoveExercise(e.ID);
                    App.Db.SaveChanges();
                    AllExercises.Remove(e);
                    error += $"Removed exercise: {exercise.Name}.\n";
                }
            }
            return error;
        }

        public static void FillDisplayedExercises(ObservableCollection<Exercise> exercises)
        {
             exercises.Clear();
             int i = 0;
             foreach(Exercise e in AppState.FilteredExercises)
             {
                 if (i >= AppState.MaxExercises)
                     break;
                 exercises.Add(e);
                 i++;
             }
        }

        public static string UpdateExercise(Exercise updated)
        {
            string r = "";
            if (AllExercises == null)
            {
                r += "DogoFit's exercise list is empty.\n";
                return r;
            }
            if(EditedExercise == null)
            {
                r += "Edited exercise does not exist (null).\n";
                return r;
            }
            Exercise exercise = AllExercises.FirstOrDefault(x => string.Equals(x.Name, EditedExercise.Name, StringComparison.OrdinalIgnoreCase));
            if (exercise == null)
            {
                r += "Can't find the selected exercise in the exercise list.\n";
                return r;
            }
            AllExercises.Remove(exercise);
            AllExercises.Add(updated);
            DbHelper.UpdateExercise(updated, exercise.ID);
            r += "Updated the exercise.\n";
            return r;
        }
        public static (bool, string) AddExercise(Exercise exercise)
        {
            bool r = true;
            string errors = "";
            if (AppState.AllExercises.Any(e => e.Name.Equals(exercise.Name, StringComparison.OrdinalIgnoreCase)))
			{
				errors += "An Exercise with this name already exists.\n";
                r = false;
			}
			if (exercise.Name.Length > 50)
			{
				errors += "Exercise Name over 50 characters.\n";
				r = false;
			}
            if (string.IsNullOrEmpty(exercise.Name))
			{
				errors += "Exercise Name is empty.\n";
				r = false;
			}
            if (r == true)
            {
                AllExercises.Add(exercise);
                var dbexercise = DbHelper.ToDbExercise(exercise);
                App.Db.Exercises.Add(dbexercise);
                App.Db.SaveChanges();
            }

            return (r, errors);
        }

        public static BoxView Helper_CreateSeparator()
        {
            var sep = new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromArgb("#333333"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Margin = new Thickness(0, 5, 0, 0)
            };
            return sep;
        }

        public static Border Helper_CreateContainer()
        {
            var container = new Border
            {
                StrokeShape = new RoundRectangle { CornerRadius = 12 },
                StrokeThickness = 0,
                BackgroundColor = Colors.Transparent,
                Padding = new Thickness(15),
                Shadow = new Shadow
                {
                    Brush = Brush.Black,
                    Opacity = 0.4f,
                    Offset = new Point(0,4),
                    Radius = 8
                }
            };
            return container;
        }

        public static List<Label> CreateSetLabels(string settext)
        {
            List<Label> labels = new List<Label>();
            Label setlabel = new Label { Text = settext, BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#FFB300"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            Label replabel = new Label { Text = "REPS", BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#FFB300"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            Label weightlabel = new Label { Text = "WEIGHT", BackgroundColor = Colors.Transparent, TextColor = Color.FromArgb("#FFB300"), FontSize = 15, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand };
            labels.Add(setlabel);
            labels.Add(replabel);
            labels.Add(weightlabel);
            return labels;
        }

        public static void SetLayout(IEnumerable<Set> sets, Exercise exercise, VerticalStackLayout stack, string settext, Func<Exercise, Set, List<View>> createControls,  Func<string, List<Label>> createLabels, int columns)
        {
            var grid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                },
                RowDefinitions = { },
                HorizontalOptions = LayoutOptions.Fill,
                ColumnSpacing = 0,
                RowSpacing = 5, BackgroundColor = Colors.Transparent
            };

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            List<Label> labels = createLabels(settext);
            int j = 0;
            foreach(var label in labels)
            {
                grid.Add(label, j++, 0);
            }

            int row = 1;
            foreach (Set set in sets)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                var setseparator = AppState.Helper_CreateSeparator();
                Grid.SetRow(setseparator, row);
                Grid.SetColumn(setseparator, 0);
                Grid.SetColumnSpan(setseparator, columns);
                grid.Children.Add(setseparator);

                row++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                List<View> controls = createControls(exercise, set);

                int i = 0;
                foreach(var view in controls)
                {
                    grid.Add(view, i++, row);
                }
                row++;
            }
            stack.Children.Add(grid);
        }
    }
}
