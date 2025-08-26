using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GymTracker.Services
{
    public static class DbHelper
    {
        public static DbWorkout ToDbWorkout(Routine routine)
        {
            if (routine.IsRunning)
                routine.Duration = DateTime.Now - routine.StartTime;

            return new DbWorkout
            {
                JsonRoutine = JsonSerializer.Serialize(routine, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.Never
                }),
                Date = routine.StartTime != default
                    ? routine.StartTime
                    : DateTime.Now
            };
        }

        public static Routine FromDbWorkout(DbWorkout dbWorkout)
        {
            if (dbWorkout == null || string.IsNullOrEmpty(dbWorkout.JsonRoutine))
                return null;

            var routine = JsonSerializer.Deserialize<Routine>(dbWorkout.JsonRoutine);
            if (routine != null)
            {
                routine.ID = dbWorkout.Id;
            }

            return routine;
        }


        public static DbRoutineTemplate ToDbRoutineTemplate(Routine routine)
        {
            return new DbRoutineTemplate
            {
                Name = routine.Name,
                JsonRoutine = JsonSerializer.Serialize(routine)
            };
        }
        public static void UpdateRoutineTemplate(Routine routine, int templateId)
        {
            using var db = App.Db;
            var template = db.RoutineTemplates.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            {
                template.JsonRoutine = JsonSerializer.Serialize(routine);
                db.SaveChanges();
            }
        }

        public static void DeleteRoutineTemplate(int templateId)
        {
            using var db = App.Db;
            var template = db.RoutineTemplates.FirstOrDefault(t => t.Id == templateId);
            if (template != null)
            {
                db.RoutineTemplates.Remove(template);
                db.SaveChanges();
            }
        }
        public static Routine FromDbRoutineTemplate(DbRoutineTemplate dbTemplate)
        {
            if (dbTemplate == null || string.IsNullOrEmpty(dbTemplate.JsonRoutine))
                return null;

            var routine = JsonSerializer.Deserialize<Routine>(dbTemplate.JsonRoutine);
            if (routine != null)
            {
                routine.ID = dbTemplate.Id;
            }

            return routine;
        }
        public static Profile FromDbProfile(DbProfile dbProfile)
        {
            return JsonSerializer.Deserialize<Profile>(dbProfile.JsonProfile);
        }

        public static DbProfile ToDbProfile(Profile profile)
        {
            return new DbProfile
            {
                JsonProfile = JsonSerializer.Serialize(profile)
            };
        }

        public static Profile LoadProfile(AppDbContext db)
        {
            var dbProfile = db.Profiles.FirstOrDefault(p => p.Id == 1);
            if (dbProfile == null)
            {
                var profile = new Profile();
                SaveProfile(db, profile);
                return profile;
            }
            return DbHelper.FromDbProfile(dbProfile);
        }

        public static void SaveProfile(AppDbContext db, Profile profile)
        {
            var dbProfile = db.Profiles.FirstOrDefault(p => p.Id == 1);
            if (dbProfile == null)
            {
                dbProfile = new DbProfile
                {
                    Id = 1,
                    JsonProfile = JsonSerializer.Serialize(profile)
                };
                db.Profiles.Add(dbProfile);
            }
            else
            {
                dbProfile.JsonProfile = JsonSerializer.Serialize(profile);
                db.Profiles.Update(dbProfile);
            }
            db.SaveChanges();
        }
        public static void DeleteDatabase(String path)
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, path);
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Console.WriteLine("Database deleted.");
            }
        }

        public static DbWorkoutInProgress ToDbWorkoutInProgress(Routine routine)
        {
            return new DbWorkoutInProgress
            {
                JsonRoutine = JsonSerializer.Serialize(routine)
            };
        }

        public static Routine FromDbWorkoutInProgress(DbWorkoutInProgress dbWorkoutInProgress)
        {
            if (dbWorkoutInProgress == null || string.IsNullOrEmpty(dbWorkoutInProgress.JsonRoutine))
                return null;
            var routine = JsonSerializer.Deserialize<Routine>(dbWorkoutInProgress.JsonRoutine);
            if (routine != null)
            {
                routine.ID = dbWorkoutInProgress.Id;
            }
            return routine;
        }

        public static void SaveWorkoutInProgress(AppDbContext db, Routine routine)
        {
            var dbWorkoutInProgress = db.WorkoutInProgress.FirstOrDefault(w => w.Id == 1);

            if (dbWorkoutInProgress == null)
            {
                dbWorkoutInProgress = new DbWorkoutInProgress
                {
                    Id = 1,
                    JsonRoutine = JsonSerializer.Serialize(routine)
                };
                db.WorkoutInProgress.Add(dbWorkoutInProgress);
            }
            else
            {
                dbWorkoutInProgress.JsonRoutine = JsonSerializer.Serialize(routine);
                db.WorkoutInProgress.Update(dbWorkoutInProgress);
            }

            db.SaveChanges();
        }


        public static Routine? LoadWorkoutInProgress(AppDbContext db)
        {
            var dbWorkoutInProgress = db.WorkoutInProgress.FirstOrDefault(w => w.Id == 1);
            return FromDbWorkoutInProgress(dbWorkoutInProgress);
        }


        public static void DeleteWorkoutInProgress(AppDbContext db)
        {
            var dbWorkoutInProgress = db.WorkoutInProgress.FirstOrDefault(w => w.Id == 1);
            if (dbWorkoutInProgress != null)
            {
                db.WorkoutInProgress.Remove(dbWorkoutInProgress);
                db.SaveChanges();
            }
        }


    }
}
