using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows.Input;

namespace GymTracker.Services
{
    public class Profile : INotifyPropertyChanged
    {
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
        public string DurationString
        {
            get
            {
                var ts = TimeSpan.FromSeconds(AppState.Workouts.Sum(w => w.Duration.TotalSeconds));

                if (ts.TotalHours >= 24)
                {
                    return $"{ts.Days}d {ts.Hours:00}:{ts.Minutes:00}";
                }

                return $"{(int)ts.TotalHours:00}:{ts.Minutes:00}";
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
