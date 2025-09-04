using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{

    public struct ExerciseRecord
    {
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public double HeaviestWeight { get; set; }
        public double SessionVolume { get; set; }
        public (double, int) BestSet { get; set; }
        public double OneRepMax { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
    }
}
