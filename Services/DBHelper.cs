using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GymTracker.Services
{
    public static class DbHelper
    {
        public static DbWorkout ToDbWorkout(Routine routine)
        {
            return new DbWorkout
            {
                JsonRoutine = JsonSerializer.Serialize(routine),
                Date = DateTime.Now
            };
        }

        public static Routine FromDbWorkout(DbWorkout dbWorkout)
        {
            return JsonSerializer.Deserialize<Routine>(dbWorkout.JsonRoutine);
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
        public static Routine FromDbRoutineTemplate(DbRoutineTemplate template)
        {
            return JsonSerializer.Deserialize<Routine>(template.JsonRoutine);
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
    }
}
