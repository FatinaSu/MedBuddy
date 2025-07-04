using MedBuddy.Model;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace MedBuddy.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            DataContext = this;
            InitializeComponent();
        }

        private MainWindow? mainWindow => Application.Current.MainWindow as MainWindow;

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string benutzer = txtBenutzername.Text.Trim();
                string passwort = txtPasswort.Password;

                var repo = new BenutzerRepository();
                string? rolle = repo.GetBenutzerRolle(benutzer, passwort);
                int benutzerId = repo.GetBenutzerId(benutzer, passwort);

                if (rolle == "Arzt")
                {
                    mainWindow?.SwitchToView(new ArztView());
                }
                else if (rolle == "Patient")
                {
                    mainWindow?.SwitchToView(new PatientenView(benutzerId));
                }
                else if (rolle != null)
                {
                    MessageBox.Show("Login erfolgreich, aber Rolle unbekannt.");
                }
                else
                {
                    MessageBox.Show("Login fehlgeschlagen!");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Fehler beim Anmelden: {ex.Message}");
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            if (mainWindow != null)
            {
                mainWindow.SwitchToView(new RegisterView());
            }
        }
    }
}
