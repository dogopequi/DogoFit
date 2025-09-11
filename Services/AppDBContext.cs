using Microsoft.EntityFrameworkCore;

namespace GymTracker.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<DbWorkout> Workouts { get; set; }
        public DbSet<DbExercise> Exercises { get; set; }
        public DbSet<DbRoutineTemplate> RoutineTemplates { get; set; }
        public DbSet<DbProfile> Profiles { get; set; }

        public DbSet<DbWorkoutInProgress> WorkoutInProgress { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "gymtracker.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

    }
}