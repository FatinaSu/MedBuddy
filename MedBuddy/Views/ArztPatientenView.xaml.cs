using MedBuddy.Model;
using MedBuddy.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MedBuddy.Views
{
    public partial class ArztPatientenView : UserControl, ITab, INotifyPropertyChanged
    {
        public ObservableCollection<PatientViewModel> Patienten { get; set; } = new();
        public ObservableCollection<PatientViewModel> GefiltertePatienten { get; set; } = new();
        public ObservableCollection<Medikament> Medikamente { get; set; } = new();

        private PatientViewModel selectedPatient;
        public PatientViewModel SelectedPatient
        {
            get => selectedPatient;
            set
            {
                selectedPatient = value;
                OnPropertyChanged(nameof(SelectedPatient));
                LadeMedikamente(selectedPatient?.Id ?? -1);
            }
        }

        public ArztPatientenView()
        {
            InitializeComponent();
            DataContext = this;

            var repo = new PatientRepository();
            var ausDatenbank = repo.LadeAllePatienten();
            foreach (var patient in ausDatenbank)
                Patienten.Add(patient);
            // Initialisiere gefilterte Liste mit allen Patienten
            foreach (var patient in Patienten)
                GefiltertePatienten.Add(patient);
        }

        public string Title => "Patientenverwaltung";

        private void LadeMedikamente(int benutzerId)
        {
            Medikamente.Clear();
            if (benutzerId < 0) return;
            var repo = new MedikamentRepository();
            var medikamente = repo.LadeMedikamente(benutzerId);
            foreach (var med in medikamente)
                Medikamente.Add(med);
        }

        private void AddMedikament_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Bitte zuerst einen Patienten auswählen.");
                return;
            }

            var neuesMedikament = new NeuesMedikamentWindow
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (neuesMedikament.ShowDialog() == true)
            {
                var med = new Medikament
                {
                    Name = neuesMedikament.Medikament,
                    Uhrzeit = new TimeSpan(neuesMedikament.Stunde, neuesMedikament.Minute, 0),
                    Haeufigkeit = neuesMedikament.Haeufigkeit,
                    BenutzerId = SelectedPatient.Id
                };

                var repo = new MedikamentRepository();
                repo.SpeichereMedikament(med);
                LadeMedikamente(SelectedPatient.Id);
            }
        }

        private void DeleteMedikament_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Bitte zuerst einen Patienten auswählen.");
                return;
            }
            var button = sender as Button;
            var med = button?.Tag as Medikament;
            if (med == null)
            {
                MessageBox.Show("Bitte ein Medikament auswählen.");
                return;
            }
            if (MessageBox.Show($"Möchtest du '{med.Name}' wirklich löschen?", "Bestätigung", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var repo = new MedikamentRepository();
                repo.LöscheMedikamentMitId(med.Id);
                LadeMedikamente(SelectedPatient.Id);
            }
        }

        private void BearbeitenMedikament_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Bitte zuerst einen Patienten auswählen.");
                return;
            }
            var button = sender as Button;
            var med = button?.Tag as Medikament;
            if (med == null)
            {
                MessageBox.Show("Bitte ein Medikament auswählen.");
                return;
            }
            var bearbeiteFenster = new NeuesMedikamentWindow();
            bearbeiteFenster.SetzeWerteZumBearbeiten(med.Name, med.Uhrzeit.Hours, med.Uhrzeit.Minutes, med.Haeufigkeit);
            bearbeiteFenster.MedikamentId = med.Id;
            if (bearbeiteFenster.ShowDialog() == true)
            {
                med.Name = bearbeiteFenster.Medikament;
                med.Uhrzeit = new TimeSpan(bearbeiteFenster.Stunde, bearbeiteFenster.Minute, 0);
                med.Haeufigkeit = bearbeiteFenster.Haeufigkeit;
                var repo = new MedikamentRepository();
                repo.AktualisiereMedikament(med);
                LadeMedikamente(SelectedPatient.Id);
            }
        }

        private void EinnahmenAnzeigen_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Bitte zuerst einen Patienten auswählen.");
                return;
            }
            var dialog = new EinnahmeverlaufWindow(SelectedPatient.Id)
            {
                Owner = Application.Current.MainWindow
            };
            dialog.ShowDialog();
        }

        private void EinnahmeStatistik_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Bitte zuerst einen Patienten auswählen.");
                return;
            }
            var fenster = new EinnahmeDiagrammWindow(SelectedPatient.Id)
            {
                Owner = Application.Current.MainWindow
            };
            fenster.ShowDialog();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 