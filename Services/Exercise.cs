using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace GymTracker.Services
{
    public class Exercise : INotifyPropertyChanged
    {

        private bool isSelected;
        public string Name { get; set; }

        public MuscleFunctions Function { get; set; }
        public Muscles TargetMuscle { get; set; }
        public MuscleGroups MuscleGroup { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Set> Sets { get; set; }
        public IEnumerable<Set> CheckedSets => Sets?.Where(s => s.IsChecked == true);

        public int SetCount => Sets.Count;
        public int SetCountChecked => Sets.Count(s => s.IsChecked);
        public List<Muscles>? SecondaryMuscles { get; set; } = new List<Muscles>();
        public bool IsUnilateral { get; set; }
        [JsonIgnore]
        public int ID { get; set; }
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
            SecondaryMuscles = other.SecondaryMuscles;
            Function = other.Function;
            TargetMuscle = other.TargetMuscle;
            MuscleGroup = other.MuscleGroup;
            isSelected = false;
            IsUnilateral = other.IsUnilateral;
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
            RecalculateSetIndexes();
        }
        public void RemoveSet(int setId, SideType side)
        {
            var setToRemove = Sets.FirstOrDefault(s => s.ID == setId && s.Side == side);
            if (setToRemove != null)
            {
                Sets.Remove(setToRemove);
            }
            RecalculateSetIndexes();
        }

        public void RecalculateSetIndexes()
        {
            if (IsUnilateral)
            {
                int left = 1;
                int right = 1;
                foreach (Set set in Sets)
                {
                    if (set.Side == SideType.Left)
                        set.ID = left++;
                    else if (set.Side == SideType.Right)
                        set.ID = right++;
                }
            }
            else
            {
                int i = 1;
                foreach (Set set in Sets)
                {
                    set.ID = i++;
                }
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
