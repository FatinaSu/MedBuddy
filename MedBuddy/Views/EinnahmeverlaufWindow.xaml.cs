using System.Collections.ObjectModel;
using System.Windows;
using MedBuddy.Model;

namespace MedBuddy.Views
{
    public partial class EinnahmeverlaufWindow : Window
    {
        public ObservableCollection<EinnahmeViewModel> Einnahmen { get; set; }
        private int _patientId;

        public EinnahmeverlaufWindow(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            var repo = new EinnahmeRepository();
            Einnahmen = new ObservableCollection<EinnahmeViewModel>(repo.LadeEinnahmenFuerPatient(_patientId));
            DataContext = this;
            EinnahmenTabelle.ItemsSource = Einnahmen;
        }

        private void Speichern_Click(object sender, RoutedEventArgs e)
        {
            var repo = new MedBuddy.Model.EinnahmeRepository();
            repo.SpeichereEinnahmen(Einnahmen.ToList(), _patientId);
            Einnahmen.Clear();
            foreach(var einnahme in repo.LadeEinnahmenFuerPatient(_patientId))
                Einnahmen.Add(einnahme);
            MessageBox.Show("Änderungen erfolgreich gespeichert.", "Bestätigung");
        }

        private void Diagramm_Click(object sender, RoutedEventArgs e)
        {
            var diagrammWindow = new EinnahmeDiagrammWindow(_patientId);
            diagrammWindow.Owner = this;
            diagrammWindow.ShowDialog();
        }
    }
} 