using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace GymTracker.Services
{
    public class Profile : INotifyPropertyChanged
    {
        public double LatsPercentage => Sets == 0 ? 0 : (double)Lats / Sets;
        public double TricepsPercentage => Sets == 0 ? 0 : (double)Triceps / Sets;
        public double BicepsPercentage => Sets == 0 ? 0 : (double)Biceps / Sets;
        public double QuadsPercentage => Sets == 0 ? 0 : (double)Quadriceps / Sets;
        public double HamsPercentage => Sets == 0 ? 0 : (double)Hamstrings / Sets;
        public double GlutesPercentage => Sets == 0 ? 0 : (double)Glutes / Sets;
        public double CalvesPercentage => Sets == 0 ? 0 : (double)Calves / Sets;
        public double AbsPercentage => Sets == 0 ? 0 : (double)Abdominals / Sets;
        public double ObliquesPercentage => Sets == 0 ? 0 : (double)Obliques / Sets;
        public double FDPercentage => Sets == 0 ? 0 : (double)FrontDelts / Sets;
        public double RDPercentage => Sets == 0 ? 0 : (double)RearDelts / Sets;
        public double LDPercentage => Sets == 0 ? 0 : (double)LateralDelts / Sets;
        public double TrapsPercentage => Sets == 0 ? 0 : (double)Traps / Sets;
        public double ForearmsPercentage => Sets == 0 ? 0 : (double)Forearms / Sets;
        public double PushPercentage => Sets == 0 ? 0 : (double)Push / Sets;
        public double ChestPercentage => Sets == 0 ? 0 : (double)Chest / Sets;
        public double BackPercentage => Sets == 0 ? 0 : (double)Back / Sets;
        public double LegsPercentage => Sets == 0 ? 0 : (double)Legs / Sets;
        public double ArmsPercentage => Sets == 0 ? 0 : (double)Arms / Sets;
        public double CorePercentage => Sets == 0 ? 0 : (double)Core / Sets;
        public double ShouldersPercentage => Sets == 0 ? 0 : (double)Shoulders / Sets;
        public double PullPercentage => Sets == 0 ? 0 : (double)Pull / Sets;
        [JsonInclude]
        private bool _useMetric = true;

        [JsonInclude]
        public bool UseMetric
        {
            get => _useMetric;
            set
            {
                if (_useMetric != value)
                {
                    _useMetric = value;
                    OnPropertyChanged(nameof(UseMetric));
                    OnPropertyChanged(nameof(WeightUnit));
                }
            }
        }
        public string WeightUnit => UseMetric ? "kg" : "lbs";


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

        private double volume;
        public double Volume
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
                    OnPropertyChanged(nameof(LatsPercentage));
                    OnPropertyChanged(nameof(TricepsPercentage));
                    OnPropertyChanged(nameof(BicepsPercentage));
                    OnPropertyChanged(nameof(QuadsPercentage));
                    OnPropertyChanged(nameof(HamsPercentage));
                    OnPropertyChanged(nameof(GlutesPercentage));
                    OnPropertyChanged(nameof(CalvesPercentage));
                    OnPropertyChanged(nameof(AbsPercentage));
                    OnPropertyChanged(nameof(ObliquesPercentage));
                    OnPropertyChanged(nameof(FDPercentage));
                    OnPropertyChanged(nameof(RDPercentage));
                    OnPropertyChanged(nameof(LDPercentage));
                    OnPropertyChanged(nameof(TrapsPercentage));
                    OnPropertyChanged(nameof(ForearmsPercentage));
                    OnPropertyChanged(nameof(PushPercentage));
                    OnPropertyChanged(nameof(ChestPercentage));
                    OnPropertyChanged(nameof(BackPercentage));
                    OnPropertyChanged(nameof(LegsPercentage));
                    OnPropertyChanged(nameof(ArmsPercentage));
                    OnPropertyChanged(nameof(CorePercentage));
                    OnPropertyChanged(nameof(ShouldersPercentage));
                    OnPropertyChanged(nameof(PullPercentage));
                }
            }
        }

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
                    OnPropertyChanged(nameof(ArmsPercentage));
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
                    OnPropertyChanged(nameof(ShouldersPercentage));
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
                    OnPropertyChanged(nameof(ChestPercentage));
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
                    OnPropertyChanged(nameof(BackPercentage));
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
                    OnPropertyChanged(nameof(LegsPercentage));
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
                    OnPropertyChanged(nameof(CorePercentage));
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
                    OnPropertyChanged(nameof(PullPercentage));
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
                    OnPropertyChanged(nameof(PushPercentage));
                }
            }
        }

        private int _triceps;
        public int Triceps
        {
            get => _triceps;
            set
            {
                if (_triceps != value)
                {
                    _triceps = value;
                    OnPropertyChanged(nameof(Triceps));
                    OnPropertyChanged(nameof(TricepsPercentage));
                }
            }
        }

        private int _biceps;
        public int Biceps
        {
            get => _biceps;
            set
            {
                if (_biceps != value)
                {
                    _biceps = value;
                    OnPropertyChanged(nameof(Biceps));
                    OnPropertyChanged(nameof(BicepsPercentage));
                }
            }
        }

        private int _quadriceps;
        public int Quadriceps
        {
            get => _quadriceps;
            set
            {
                if (_quadriceps != value)
                {
                    _quadriceps = value;
                    OnPropertyChanged(nameof(Quadriceps));
                    OnPropertyChanged(nameof(QuadsPercentage));
                }
            }
        }

        private int _hamstrings;
        public int Hamstrings
        {
            get => _hamstrings;
            set
            {
                if (_hamstrings != value)
                {
                    _hamstrings = value;
                    OnPropertyChanged(nameof(Hamstrings));
                    OnPropertyChanged(nameof(HamsPercentage));
                }
            }
        }

        private int _glutes;
        public int Glutes
        {
            get => _glutes;
            set
            {
                if (_glutes != value)
                {
                    _glutes = value;
                    OnPropertyChanged(nameof(Glutes));
                    OnPropertyChanged(nameof(GlutesPercentage));
                }
            }
        }

        private int _calves;
        public int Calves
        {
            get => _calves;
            set
            {
                if (_calves != value)
                {
                    _calves = value;
                    OnPropertyChanged(nameof(Calves));
                    OnPropertyChanged(nameof(CalvesPercentage));
                }
            }
        }

        private int _abdominals;
        public int Abdominals
        {
            get => _abdominals;
            set
            {
                if (_abdominals != value)
                {
                    _abdominals = value;
                    OnPropertyChanged(nameof(Abdominals));
                    OnPropertyChanged(nameof(AbsPercentage));
                }
            }
        }

        private int _obliques;
        public int Obliques
        {
            get => _obliques;
            set
            {
                if (_obliques != value)
                {
                    _obliques = value;
                    OnPropertyChanged(nameof(Obliques));
                    OnPropertyChanged(nameof(ObliquesPercentage));
                }
            }
        }

        private int _traps;
        public int Traps
        {
            get => _traps;
            set
            {
                if (_traps != value)
                {
                    _traps = value;
                    OnPropertyChanged(nameof(Traps));
                    OnPropertyChanged(nameof(TrapsPercentage));
                }
            }
        }

        private int _lateralDelts;
        public int LateralDelts
        {
            get => _lateralDelts;
            set
            {
                if (_lateralDelts != value)
                {
                    _lateralDelts = value;
                    OnPropertyChanged(nameof(LateralDelts));
                    OnPropertyChanged(nameof(LDPercentage));
                }
            }
        }

        private int _frontDelts;
        public int FrontDelts
        {
            get => _frontDelts;
            set
            {
                if (_frontDelts != value)
                {
                    _frontDelts = value;
                    OnPropertyChanged(nameof(FrontDelts));
                    OnPropertyChanged(nameof(FDPercentage));
                }
            }
        }

        private int _rearDelts;
        public int RearDelts
        {
            get => _rearDelts;
            set
            {
                if (_rearDelts != value)
                {
                    _rearDelts = value;
                    OnPropertyChanged(nameof(RearDelts));
                    OnPropertyChanged(nameof(RDPercentage));
                }
            }
        }
        private int _formarms;
        public int Forearms
        {
            get => _formarms;
            set
            {
                if (_formarms != value)
                {
                    _formarms = value;
                    OnPropertyChanged(nameof(Forearms));
                    OnPropertyChanged(nameof(ForearmsPercentage));
                }
            }
        }

        private int _lats;
        public int Lats
        {
            get => _lats;
            set
            {
                if (_lats != value)
                {
                    _lats = value;
                    OnPropertyChanged(nameof(Lats));
                    OnPropertyChanged(nameof(LatsPercentage));
                }
            }
        }

        private bool _isSplitVisible;
        public bool IsSplitVisible
        {
            get => _isSplitVisible;
            set { _isSplitVisible = value; OnPropertyChanged(nameof(IsSplitVisible)); }
        }

        private string _splitButtonText = "Show Splits";
        public string SplitButtonText
        {
            get => _splitButtonText;
            set { _splitButtonText = value; OnPropertyChanged(nameof(SplitButtonText)); }
        }

        private bool _isMuscleGroupVisible;
        public bool IsMuscleGroupVisible
        {
            get => _isMuscleGroupVisible;
            set { _isMuscleGroupVisible = value; OnPropertyChanged(nameof(IsMuscleGroupVisible)); }
        }

        private string _muscleGroupButtonText = "Show Muscle Groups";
        public string MuscleGroupButtonText
        {
            get => _muscleGroupButtonText;
            set { _muscleGroupButtonText = value; OnPropertyChanged(nameof(MuscleGroupButtonText)); }
        }

        private bool _isIndividualMusclesVisible;
        public bool IsIndividualMusclesVisible
        {
            get => _isIndividualMusclesVisible;
            set { _isIndividualMusclesVisible = value; OnPropertyChanged(nameof(IsIndividualMusclesVisible)); }
        }

        private string _individualMusclesButtonText = "Show Indivual Muscles";
        public string IndividualMusclesButtonText
        {
            get => _individualMusclesButtonText;
            set { _individualMusclesButtonText = value; OnPropertyChanged(nameof(IndividualMusclesButtonText)); }
        }
        [JsonIgnore]
        public ICommand ToggleSplitsCommand { get; }
        [JsonIgnore]
        public ICommand ToggleMuscleGroupsCommand { get; }

        [JsonIgnore]
        public ICommand ToggleIndividualMusclesCommand { get; }

        public Profile()
        {
            ToggleSplitsCommand = new Command(() =>
            {
                IsSplitVisible = !IsSplitVisible;
                SplitButtonText = IsSplitVisible ? "Hide Splits" : "Show Splits";
            });
            ToggleMuscleGroupsCommand = new Command(() =>
            {
                IsMuscleGroupVisible = !IsMuscleGroupVisible;
                MuscleGroupButtonText = IsMuscleGroupVisible ? "Hide Muscle Groups" : "Show Muscle Groups";
            });
            ToggleIndividualMusclesCommand = new Command(() =>
            {
                IsIndividualMusclesVisible = !IsIndividualMusclesVisible;
                IndividualMusclesButtonText = IsIndividualMusclesVisible ? "Hide Individual Muscles" : "Show Individual Muscles";
            });

        }
        public string DurationString => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
