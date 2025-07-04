using System.Windows;

namespace MedBuddy.Views
{
    public partial class MedikamentErinnerungWindow : Window
    {
        public bool Eingenommen { get; private set; } = false;
        public bool Abgemeldet { get; private set; } = false;

        public MedikamentErinnerungWindow(string medikamentName, bool istZweiteErinnerung = false, string patientName = null)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(patientName))
            {
                MedikamentText.Text = istZweiteErinnerung
                    ? $"Erneute Erinnerung: Bitte {medikamentName} einnehmen!"
                    : $"Zeit für {medikamentName}!";
                PatientText.Text = $"für {patientName}";
                PatientText.Visibility = Visibility.Visible;
            }
            else
            {
                MedikamentText.Text = istZweiteErinnerung
                    ? $"Erneute Erinnerung: Bitte {medikamentName} einnehmen!"
                    : $"Zeit für {medikamentName}!";
                PatientText.Visibility = Visibility.Collapsed;
            }
        }

        private void Eingenommen_Click(object sender, RoutedEventArgs e)
        {
            Eingenommen = true;
            this.Close();
        }

        private void Abmelden_Click(object sender, RoutedEventArgs e)
        {
            Abgemeldet = true;
            this.Close();
        }
    }
} 