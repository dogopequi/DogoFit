using System.ComponentModel;

namespace GymTracker.Services
{
    public enum SetType
    {
        Normal,
        Warmup,
        Failure,
        Drop
    }
    public enum SideType
    {
        None, Left, Right
    }
    public class Set : INotifyPropertyChanged
    {
        private int _id;
        private int _reps;
        private double _weight;
        private bool _isChecked;
        private string _lastSet;
        private SetType _type;
        private SideType _side;

        public SideType Side
        {
            get => _side;
            set
            {
                if (_side != value)
                {
                    _side = value;
                    OnPropertyChanged(nameof(Side));
                }
            }
        }
        public SetType Type
        {
            get => _type;
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(nameof(Type));
                }
            }
        }


        public string LastSet
        {
            get => _lastSet;
            set
            {
                if (_lastSet != value)
                {
                    _lastSet = value;
                    OnPropertyChanged(nameof(LastSet));
                }
            }
        }
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public int ID
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(ID));
                }
            }
        }

        public int Reps
        {
            get => _reps;
            set
            {
                if (_reps != value)
                {
                    _reps = value;
                    OnPropertyChanged(nameof(Reps));
                }
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged(nameof(Weight));
                }
            }
        }

        public Set() { }
        public Set(Set other)
        {
            ID = other.ID;
            Reps = other.Reps;
            Weight = other.Weight;
            IsChecked = other.IsChecked;
            Type = other.Type;
            Side = other.Side;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

