using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymTracker.Services
{
    public class Exercise : INotifyPropertyChanged
    {

        private bool isSelected;
        public string Name { get; set; }

        public string Function { get; set; }
        public string TargetMuscle { get; set; }
        public string MuscleGroup { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Set> Sets { get; set; }
        public IEnumerable<Set> CheckedSets => Sets?.Where(s => s.IsChecked == true);

        public int SetCount => Sets.Count;
        public int SetCountChecked => Sets.Count(s => s.IsChecked);
        public List<string> SecondaryMuscles { get; set; } = new List<string>();
        public Exercise()
        {
            Sets = new ObservableCollection<Set>();
            Sets.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Set set in e.NewItems)
                        set.PropertyChanged += Set_PropertyChanged;
                }
                if (e.OldItems != null)
                {
                    foreach (Set set in e.OldItems)
                        set.PropertyChanged -= Set_PropertyChanged;
                }

                OnPropertyChanged(nameof(SetCount));
                OnPropertyChanged(nameof(SetCountChecked));
                OnPropertyChanged(nameof(CheckedSets));
            };
        }
        public Exercise(Exercise other)
        {
            Name = other.Name;
            Description = other.Description;
            Sets = new ObservableCollection<Set>(other.Sets.Select(s => new Set(s)));
            Sets.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null)
                {
                    foreach (Set set in e.NewItems)
                        set.PropertyChanged += Set_PropertyChanged;
                }
                if (e.OldItems != null)
                {
                    foreach (Set set in e.OldItems)
                        set.PropertyChanged -= Set_PropertyChanged;
                }

                OnPropertyChanged(nameof(SetCount));
                OnPropertyChanged(nameof(SetCountChecked));
                OnPropertyChanged(nameof(CheckedSets));
            };
            foreach (var set in Sets)
                set.PropertyChanged += Set_PropertyChanged;
            SecondaryMuscles = new List<string>(other.SecondaryMuscles);
            Function = other.Function;
            TargetMuscle = other.TargetMuscle;
            MuscleGroup = other.MuscleGroup;
            isSelected = false;
        }
        private void Set_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Set.IsChecked))
            {
                OnPropertyChanged(nameof(SetCountChecked));
                OnPropertyChanged(nameof(CheckedSets));
            }
        }
        public void AddSet(Set set)
        {
            var newset = new Set(set);
            Sets.Add(newset);
        }
        public void RemoveSet(int setId)
        {
            var setToRemove = Sets.FirstOrDefault(s => s.ID == setId);
            if (setToRemove != null)
            {
                Sets.Remove(setToRemove);
            }
        }
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
