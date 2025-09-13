namespace GymTracker.Services
{
    public struct WeekStat
    {
        public int WeekNumber { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public double Volume { get; set; }
        public TimeSpan Duration { get; set; }
        public int Reps { get; set; }
        public int Sets { get; set; }
    }

}
