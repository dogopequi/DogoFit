using System;
using System.ComponentModel;

namespace GymTracker.Services
{
    public class Profile : INotifyPropertyChanged
    {
        private int workouts;
        public int Workouts
        {
            get => workouts;
            set
            {
                if (workouts != value)
                {
                    workouts = value;
                    OnPropertyChanged(nameof(Workouts));
                }
            }
        }

        private double duration;
        public double Duration
        {
            get => duration;
            set
            {
                if (Math.Abs(duration - value) > 0.001)
                {
                    duration = value;
                    OnPropertyChanged(nameof(Duration));
                    OnPropertyChanged(nameof(DurationString));
                }
            }
        }

        private int volume;
        public int Volume
        {
            get => volume;
            set
            {
                if (volume != value)
                {
                    volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        private int reps;
        public int Reps
        {
            get => reps;
            set
            {
                if (reps != value)
                {
                    reps = value;
                    OnPropertyChanged(nameof(Reps));
                }
            }
        }

        private int sets;
        public int Sets
        {
            get => sets;
            set
            {
                if (sets != value)
                {
                    sets = value;
                    OnPropertyChanged(nameof(Sets));
                }
            }
        }

        public string DurationString => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
