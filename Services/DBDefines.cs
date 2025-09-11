using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymTracker.Services
{
    public class DbWorkout
    {
        [Key]
        public int Id { get; set; }

        public string JsonRoutine { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class DbExercise
    {
        [Key]
        public int Id { get; set; }

        public string JsonExercise { get; set; }

    }

    public class DbRoutineTemplate
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string JsonRoutine { get; set; }
    }

    public class DbProfile
    {
        [Key]
        public int Id { get; set; } = 1;
        public string JsonProfile { get; set; }
    }

    public class DbWorkoutInProgress
    {
        [Key]
        public int Id { get; set; } = 1;
        public string JsonRoutine { get; set; }
    }
}