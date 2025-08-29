using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{

    public class ExerciseRecord
    {
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public int HeaviestWeight { get; set; }
        public double BestSessionVolume { get; set; }
        public (double, int) BestSetSession { get; set; }
        public double BestOneRepMax { get; set; }
        public int TotalSets { get; set; }
        public int TotalReps { get; set; }
        public  double TotalVolume { get; set; }
    }
}
