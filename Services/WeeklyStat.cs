using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public class WeekStat
    {
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public double Volume { get; set; }
        public double Duration { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
    }

}
