using MedBuddy.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MedBuddy.Views
{
    /// <summary>
    /// Interaction logic for PatientenView.xaml
    /// </summary>
    public partial class PatientenView : UserControl, INotifyPropertyChanged
    {
        private MedikamentenView medikamentenView;
        private TagebuchView tagebuchView;
        private readonly int benutzerId;
        private MedikamentReminderService _reminderService;

        public PatientenView(int benutzerId)
        {
            this.benutzerId = benutzerId;
            DataContext = this;
            Loaded += PatientenView_Loaded;
            InitializeComponent();
        }

        private MainWindow? mainWindow => Application.Current.MainWindow as MainWindow;

        private void Abmelden_Click(object sender, RoutedEventArgs e)
        {
            _reminderService?.Stop();
            mainWindow?.SwitchToView(new LoginView());
        }

        private void PatientenView_Loaded(object? sender, EventArgs e)
        {
            medikamentenView = new MedikamentenView(benutzerId);
            tagebuchView = new TagebuchView(benutzerId);

            // Tabs füllen
            Tabs.Add(medikamentenView);
            Tabs.Add(tagebuchView);

            // ReminderService starten
            StarteOderAktualisiereReminderService();

            // ReminderService bei jeder Änderung neu starten
            medikamentenView.MedikamentenListeGeaendert += (s, args) => StarteOderAktualisiereReminderService();
        }

        private void StarteOderAktualisiereReminderService()
        {
            if (_reminderService != null)
            {
                _reminderService.Stop();
                _reminderService = null;
            }
            var medikamente = new ObservableCollection<MedBuddy.Model.Medikament>(
                medikamentenView.MedikamentenListe.Select(vm => new MedBuddy.Model.Medikament
                {
                    Name = vm.Name,
                    Uhrzeit = vm.Uhrzeit,
                    // Falls weitere Properties wie Haeufigkeit benötigt werden:
                    Haeufigkeit = vm.Haeufigkeit
                }));
            _reminderService = new MedikamentReminderService(medikamente, benutzerId, null);
        }

        public ObservableCollection<ITab> Tabs { get; } = new ObservableCollection<ITab>();

        private ITab selectedTab;
        public ITab SelectedTab
        {
            get { return selectedTab; }
            set { selectedTab = value; OnPropertyChanged(nameof(SelectedTab)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
