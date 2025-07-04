using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MedBuddy.Views
{
    /// <summary>
    /// Interaction logic for NeuesMedikamentWindow.xaml
    /// </summary>
    public partial class NeuesMedikamentWindow : Window, INotifyPropertyChanged
    {
        public NeuesMedikamentWindow()
        {
            DataContext = this;
            InitializeComponent();
            cmbHaeufigkeit.SelectionChanged += cmbHaeufigkeit_SelectionChanged;
        }

        public int MedikamentId { get; set; } = -1;


        public TimeSpan Uhrzeit { get; set; } = TimeSpan.Zero;

        private string medikament;
        public string Medikament
        {
            get => medikament;
            set
            {
                medikament = value;
                OnPropertyChanged(nameof(Medikament));
            }
        }

        private int stunde;
        public int Stunde
        {
            get => stunde;
            set
            {
                stunde = value;
                OnPropertyChanged(nameof(Stunde));
            }
        }

        private int minute;
        public int Minute
        {
            get => minute;
            set
            {
                minute = value;
                OnPropertyChanged(nameof(Minute));
            }
        }

        private int stunde2;
        public int Stunde2
        {
            get => stunde2;
            set { stunde2 = value; OnPropertyChanged(nameof(Stunde2)); }
        }
        private int minute2;
        public int Minute2
        {
            get => minute2;
            set { minute2 = value; OnPropertyChanged(nameof(Minute2)); }
        }
        private int stunde3;
        public int Stunde3
        {
            get => stunde3;
            set { stunde3 = value; OnPropertyChanged(nameof(Stunde3)); }
        }
        private int minute3;
        public int Minute3
        {
            get => minute3;
            set { minute3 = value; OnPropertyChanged(nameof(Minute3)); }
        }

        public string Haeufigkeit { get; set; } = "Täglich";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Haeufigkeit = (cmbHaeufigkeit.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Täglich";
            this.DialogResult = true;
            this.Close();
        }

        private void cmbHaeufigkeit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (cmbHaeufigkeit.SelectedItem as ComboBoxItem)?.Content.ToString();
            spUhrzeit2.Visibility = (selected == "2x täglich" || selected == "3x täglich") ? Visibility.Visible : Visibility.Collapsed;
            spUhrzeit3.Visibility = (selected == "3x täglich") ? Visibility.Visible : Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void SetzeWerteZumBearbeiten(string name, int stunde, int minute, string haeufigkeit)
        {
            Medikament = name;
            Stunde = stunde;
            Minute = minute;
            Haeufigkeit = haeufigkeit;
        }

    }
}
