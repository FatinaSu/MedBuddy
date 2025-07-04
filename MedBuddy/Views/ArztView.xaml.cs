using MedBuddy.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MedBuddy.Views
{
    public partial class ArztView : UserControl, INotifyPropertyChanged
    {
        private ArztPatientenView arztPatientenView;

        public ArztView()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ArztView_Loaded;
        }

        private void ArztView_Loaded(object? sender, System.EventArgs e)
        {
            arztPatientenView = new ArztPatientenView();

            // Tabs füllen
            Tabs.Add(arztPatientenView);
        }

        public ObservableCollection<ITab> Tabs { get; } = new ObservableCollection<ITab>();

        private ITab selectedTab;
        public ITab SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; OnPropertyChanged(nameof(SelectedTab)); }
        }

        private void Abmelden_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow?.SwitchToView(new LoginView());
        }

        private void EinnahmeStatistik_Click(object sender, RoutedEventArgs e)
        {
            // Versuche, die PatientId aus dem Patienten-Tab zu holen
            if (SelectedTab is ArztPatientenView patientenView && patientenView.SelectedPatient != null)
            {
                var fenster = new EinnahmeDiagrammWindow(patientenView.SelectedPatient.Id)
                {
                    Owner = Application.Current.MainWindow
                };
                fenster.ShowDialog();
            }
            else
            {
                MessageBox.Show("Bitte zuerst einen Patienten im Tab 'Patientenverwaltung' auswählen.");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}


