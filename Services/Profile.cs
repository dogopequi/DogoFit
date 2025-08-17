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

        //muscle split
        private int _arms;
        private int _shoulders;
        private int _chest;
        private int _back;
        private int _legs;
        private int _core;

        private int _pull;
        private int _push;

        public int Arms
        {
            get => _arms;
            set
            {
                if (_arms != value)
                {
                    _arms = value;
                    OnPropertyChanged(nameof(Arms));
                }
            }
        }
        public int Shoulders
        {
            get => _shoulders;
            set
            {
                if (_shoulders != value)
                {
                    _shoulders = value;
                    OnPropertyChanged(nameof(Shoulders));
                }
            }
        }
        public int Chest
        {
            get => _chest;
            set
            {
                if (_chest != value)
                {
                    _chest = value;
                    OnPropertyChanged(nameof(Chest));
                }
            }
        }
        public int Back
        {
            get => _back;
            set
            {
                if (_back != value)
                {
                    _back = value;
                    OnPropertyChanged(nameof(Back));
                }
            }
        }
        public int Legs
        {
            get => _legs;
            set
            {
                if (_legs != value)
                {
                    _legs = value;
                    OnPropertyChanged(nameof(Legs));
                }
            }
        }
        public int Core
        {
            get => _core;
            set
            {
                if (_core != value)
                {
                    _core = value;
                    OnPropertyChanged(nameof(Core));
                }
            }
        }

        public int Pull
        {
            get => _pull;
            set
            {
                if (_pull != value)
                {
                    _pull = value;
                    OnPropertyChanged(nameof(Pull));
                }
            }
        }

        public int Push
        {
            get => _push;
            set
            {
                if (_push != value)
                {
                    _push = value;
                    OnPropertyChanged(nameof(Push));
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
