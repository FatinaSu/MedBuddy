using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedBuddy.ViewModel
{
    public class SymptomeViewModel
    {
        public ObservableCollection<SymptomItem> Symptome { get; set; }

        public SymptomeViewModel() { }

        public virtual ObservableCollection<SymptomItem> GetSymptomItems()
        {
            return new ObservableCollection<SymptomItem>
            {
                new SymptomItem("Kopfschmerzen"),
                new SymptomItem("Übelkeit"),
                new SymptomItem("Schwindel"),
                new SymptomItem("Schlafstörung"),
                new SymptomItem("Appetitlosigkeit")
            };
        }
    }


    public class SymptomItem : INotifyPropertyChanged
    {
        public SymptomItem(string name, bool selected = false)
        {
            Name = name;
            IsSelected = selected;
        }

        public string Name { get; }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set { isSelected = value; OnPropertyChanged(nameof(IsSelected)); }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
