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
        public string Description { get; set; }
        public ObservableCollection<Set> Sets { get; set; }
        public int SetCount => Sets.Count;
        public List<string> Categories { get; set; } = new List<string>();
        public Exercise()
        {
            Sets = new ObservableCollection<Set>();
            Sets.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SetCount));
            };
        }
        public Exercise(Exercise other)
        {
            Name = other.Name;
            Description = other.Description;
            Sets = new ObservableCollection<Set>(other.Sets.Select(s => new Set(s)));
            Sets.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SetCount));
            };
            Categories = new List<string>(other.Categories);
            isSelected = false;
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
