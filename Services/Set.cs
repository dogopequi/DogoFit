using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public class Set : INotifyPropertyChanged
    {
        private int _id;
        private int _reps;
        private int _weight;

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

        public int Weight
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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

