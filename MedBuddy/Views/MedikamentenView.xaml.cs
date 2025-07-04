using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MedBuddy.Model;
using MedBuddy.ViewModel;
using System.ComponentModel;
using System.Collections.Generic;

namespace MedBuddy.Views
{
    public partial class MedikamentenView : UserControl, ITab, INotifyPropertyChanged
    {
        public int BenutzerId { get; }

        public MedikamentenView(int benutzerId)
        {
            InitializeComponent();
            BenutzerId = benutzerId;
            DataContext = this;

            MedikamentenListe = new ObservableCollection<MedikamentViewModel>();

            LadeNaechsteOffeneMedikamente();
        }

        public string Title => "Medikamentenplan";

        public ObservableCollection<MedikamentViewModel> MedikamentenListe { get; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler MedikamentenListeGeaendert;

        private void NeuesMedikament_Closed(object sender, EventArgs e)
        {
            var eintrag = sender as NeuesMedikamentWindow;
            if (eintrag != null && eintrag.DialogResult == true)
            {
                if (string.IsNullOrWhiteSpace(eintrag.Medikament)) return;
                var repo = new MedikamentRepository();
                // Liste für alle zu speichernden Medikamente
                var medikamente = new List<Medikament>();
                // Immer das erste Uhrzeitfeld
                medikamente.Add(new Medikament
                {
                    Name = eintrag.Medikament,
                    Uhrzeit = new TimeSpan(eintrag.Stunde, eintrag.Minute, 0),
                    BenutzerId = BenutzerId,
                    Haeufigkeit = eintrag.Haeufigkeit
                });
                // Zweites Feld bei 2x/3x täglich
                if (eintrag.Haeufigkeit == "2x täglich" || eintrag.Haeufigkeit == "3x täglich")
                {
                    medikamente.Add(new Medikament
                    {
                        Name = eintrag.Medikament,
                        Uhrzeit = new TimeSpan(eintrag.Stunde2, eintrag.Minute2, 0),
                        BenutzerId = BenutzerId,
                        Haeufigkeit = eintrag.Haeufigkeit
                    });
                }
                // Drittes Feld bei 3x täglich
                if (eintrag.Haeufigkeit == "3x täglich")
                {
                    medikamente.Add(new Medikament
                    {
                        Name = eintrag.Medikament,
                        Uhrzeit = new TimeSpan(eintrag.Stunde3, eintrag.Minute3, 0),
                        BenutzerId = BenutzerId,
                        Haeufigkeit = eintrag.Haeufigkeit
                    });
                }
                // Alle speichern und anzeigen
                foreach (var med in medikamente)
                {
                    repo.SpeichereMedikament(med);
                    MedikamentenListe.Add(new MedikamentViewModel
                    {
                        Name = med.Name,
                        Uhrzeit = med.Uhrzeit,
                        Haeufigkeit = med.Haeufigkeit
                    });
                }
                MedikamentenListeGeaendert?.Invoke(this, EventArgs.Empty);
            }
        }

        private void BearbeitenMedikament_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var medVm = button?.Tag as MedikamentViewModel;
            if (medVm == null) return;

            var repo = new MedikamentRepository();
            // Alle Einträge mit gleichem Namen und BenutzerId laden
            var alleMedikamente = repo.LadeMedikamente(BenutzerId)
                .Where(m => m.Name == medVm.Name)
                .OrderBy(m => m.Uhrzeit)
                .ToList();

            // Häufigkeit bestimmen
            string haeufigkeit = "Täglich";
            if (alleMedikamente.Count == 2) haeufigkeit = "2x täglich";
            if (alleMedikamente.Count == 3) haeufigkeit = "3x täglich";

            // Uhrzeiten extrahieren
            int stunde1 = alleMedikamente.Count > 0 ? alleMedikamente[0].Uhrzeit.Hours : 0;
            int minute1 = alleMedikamente.Count > 0 ? alleMedikamente[0].Uhrzeit.Minutes : 0;
            int stunde2 = alleMedikamente.Count > 1 ? alleMedikamente[1].Uhrzeit.Hours : 0;
            int minute2 = alleMedikamente.Count > 1 ? alleMedikamente[1].Uhrzeit.Minutes : 0;
            int stunde3 = alleMedikamente.Count > 2 ? alleMedikamente[2].Uhrzeit.Hours : 0;
            int minute3 = alleMedikamente.Count > 2 ? alleMedikamente[2].Uhrzeit.Minutes : 0;

            var bearbeiteFenster = new NeuesMedikamentWindow();
            bearbeiteFenster.SetzeWerteZumBearbeiten(medVm.Name, stunde1, minute1, haeufigkeit);
            bearbeiteFenster.Stunde2 = stunde2;
            bearbeiteFenster.Minute2 = minute2;
            bearbeiteFenster.Stunde3 = stunde3;
            bearbeiteFenster.Minute3 = minute3;

            if (bearbeiteFenster.ShowDialog() == true)
            {
                // Zuerst alle alten Einträge löschen
                foreach (var med in alleMedikamente)
                {
                    repo.LoescheMedikament(med.Id);
                }
                // Dann neue Einträge wie beim Hinzufügen anlegen
                var neueMedikamente = new List<Medikament>();
                neueMedikamente.Add(new Medikament
                {
                    Name = bearbeiteFenster.Medikament,
                    Uhrzeit = new TimeSpan(bearbeiteFenster.Stunde, bearbeiteFenster.Minute, 0),
                    BenutzerId = BenutzerId,
                    Haeufigkeit = bearbeiteFenster.Haeufigkeit
                });
                if (bearbeiteFenster.Haeufigkeit == "2x täglich" || bearbeiteFenster.Haeufigkeit == "3x täglich")
                {
                    neueMedikamente.Add(new Medikament
                    {
                        Name = bearbeiteFenster.Medikament,
                        Uhrzeit = new TimeSpan(bearbeiteFenster.Stunde2, bearbeiteFenster.Minute2, 0),
                        BenutzerId = BenutzerId,
                        Haeufigkeit = bearbeiteFenster.Haeufigkeit
                    });
                }
                if (bearbeiteFenster.Haeufigkeit == "3x täglich")
                {
                    neueMedikamente.Add(new Medikament
                    {
                        Name = bearbeiteFenster.Medikament,
                        Uhrzeit = new TimeSpan(bearbeiteFenster.Stunde3, bearbeiteFenster.Minute3, 0),
                        BenutzerId = BenutzerId,
                        Haeufigkeit = bearbeiteFenster.Haeufigkeit
                    });
                }
                // Neue Einträge speichern und Liste aktualisieren
                foreach (var med in neueMedikamente)
                {
                    repo.SpeichereMedikament(med);
                }
                // MedikamentenListe neu laden
                MedikamentenListe.Clear();
                foreach (var med in repo.LadeMedikamente(BenutzerId))
                {
                    MedikamentenListe.Add(new MedikamentViewModel
                    {
                        Name = med.Name,
                        Uhrzeit = med.Uhrzeit,
                        Haeufigkeit = med.Haeufigkeit
                    });
                }
                MedikamentenListeGeaendert?.Invoke(this, EventArgs.Empty);
            }
        }

        private void LoeschenMedikament_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var medVm = button?.Tag as MedikamentViewModel;
            if (medVm == null) return;
            var result = MessageBox.Show($"Möchtest du das Medikament '{medVm.Name}' wirklich löschen?", "Löschen bestätigen", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                var repo = new MedikamentRepository();
                var med = repo.LadeMedikamente(BenutzerId).FirstOrDefault(m => m.Name == medVm.Name && m.Uhrzeit == medVm.Uhrzeit);
                if (med != null)
                {
                    repo.LoescheMedikament(med.Id);
                    MedikamentenListe.Remove(medVm);
                    MedikamentenListeGeaendert?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void HinzufuegenMedikament_Click(object sender, RoutedEventArgs e)
        {
            var neuesMedikamentWindow = new NeuesMedikamentWindow();
            neuesMedikamentWindow.Closed += NeuesMedikament_Closed;
            neuesMedikamentWindow.ShowDialog();
        }

        public void LadeNaechsteOffeneMedikamente()
        {
            MedikamentenListe.Clear();
            var medRepo = new MedikamentRepository();
            var einnahmeRepo = new EinnahmeRepository();
            var alleMedikamente = medRepo.LadeMedikamente(BenutzerId)
                .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                .ToList();
            var einnahmen = einnahmeRepo.LadeEinnahmenFuerPatient(BenutzerId)
                .Where(e => e.Datum.Date == DateTime.Today)
                .ToList();

            // Gruppieren nach Name
            var gruppiert = alleMedikamente.GroupBy(m => m.Name);
            foreach (var gruppe in gruppiert)
            {
                // Alle geplanten Uhrzeiten für dieses Medikament
                var geplanteUhrzeiten = gruppe.OrderBy(m => m.Uhrzeit).ToList();
                // Finde die nächste offene Uhrzeit (Status != 'genommen')
                foreach (var med in geplanteUhrzeiten)
                {
                    var einnahme = einnahmen.FirstOrDefault(e => e.MedikamentName == med.Name && e.Datum.Date == DateTime.Today && e.Status == "genommen" && e.Hinweis == med.Uhrzeit.ToString());
                    if (einnahme == null)
                    {
                        // Noch nicht genommen -> anzeigen
                        MedikamentenListe.Add(new MedikamentViewModel
                        {
                            Name = med.Name,
                            Uhrzeit = med.Uhrzeit,
                            Haeufigkeit = med.Haeufigkeit
                        });
                        break; // Nur die nächste offene Uhrzeit anzeigen
                    }
                }
            }
        }

        public void EinnahmeBestaetigt()
        {
            LadeNaechsteOffeneMedikamente();
        }
    }
} 