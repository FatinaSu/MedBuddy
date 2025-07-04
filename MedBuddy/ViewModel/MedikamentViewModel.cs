using System;
using System.ComponentModel;

namespace MedBuddy.ViewModel
{
    public class MedikamentViewModel : INotifyPropertyChanged
    {
        private string _name;
        private TimeSpan _uhrzeit;
        private bool _istEingenommen;
        private string _haeufigkeit;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public TimeSpan Uhrzeit
        {
            get => _uhrzeit;
            set
            {
                if (_uhrzeit != value)
                {
                    _uhrzeit = value;
                    OnPropertyChanged(nameof(Uhrzeit));
                    OnPropertyChanged(nameof(UhrzeitAnzeige));
                }
            }
        }

        public string UhrzeitAnzeige => Uhrzeit.ToString(@"hh\:mm");

        public bool IstEingenommen
        {
            get => _istEingenommen;
            set
            {
                if (_istEingenommen != value)
                {
                    _istEingenommen = value;
                    OnPropertyChanged(nameof(IstEingenommen));
                }
            }
        }

        public string Haeufigkeit
        {
            get => _haeufigkeit;
            set
            {
                if (_haeufigkeit != value)
                {
                    _haeufigkeit = value;
                    OnPropertyChanged(nameof(Haeufigkeit));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
