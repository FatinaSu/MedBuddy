using MedBuddy.Model;
using MySql.Data.MySqlClient;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace MedBuddy.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl, INotifyPropertyChanged
    {
        private MainWindow? mainWindow => Application.Current.MainWindow as MainWindow;

        public RegisterView()
        {
            DataContext = this;
            InitializeComponent();
        }

        public Array Rollen => Enum.GetValues(typeof(Benutzerrolle));

        private Benutzerrolle benutzerrolle;
        public Benutzerrolle Benutzerrolle
        {
            get => benutzerrolle;
            set
            {
                benutzerrolle = value;
                OnPropertyChanged(nameof(Benutzerrolle));
            }
        }

        private string benutzername;
        public string Benutzername
        {
            get => benutzername;
            set
            {
                benutzername = value;
                BenutzernameFehlermeldung = string.IsNullOrWhiteSpace(value) ? "Pflichtfeld" : null;
                OnPropertyChanged(nameof(Benutzername));
            }
        }



        private string vorname;
        public string Vorname
        {
            get => vorname;
            set
            {
                vorname = value;
                VornameFehlermeldung = string.IsNullOrWhiteSpace(value) ? "Pflichtfeld" : null;
                OnPropertyChanged(nameof(Vorname));
            }
        }


        private DateTime geburtstag = DateTime.Today;
        public DateTime Geburtstag
        {
            get => geburtstag;
            set
            {
                geburtstag = value;
                OnPropertyChanged(nameof(Geburtstag));

                if (geburtstag > DateTime.Today)
                    GeburtstagFehlermeldung = "Geburtsdatum darf nicht in der Zukunft liegen.";
                else
                    GeburtstagFehlermeldung = null;
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;

                // Validierung
                if (string.IsNullOrWhiteSpace(value))
                    EmailFehlermeldung = "Pflichtfeld";
                else if (!value.Contains("@") || !value.Contains("."))
                    EmailFehlermeldung = "Ungültiges E-Mail-Format";
                else
                    EmailFehlermeldung = null;

                OnPropertyChanged(nameof(Email));
            }
        }


        private string adresse;
        public string Adresse
        {
            get => adresse;
            set
            {
                adresse = value;
                OnPropertyChanged(nameof(Adresse));
            }
        }

        private string telefon;
        public string Telefon
        {
            get => telefon;
            set
            {
                telefon = value;
                OnPropertyChanged(nameof(Telefon));
            }
        }

        private string passwort;
        public string Passwort
        {
            get => passwort;
            set
            {
                passwort = value;

                if (string.IsNullOrWhiteSpace(value))
                    PasswortFehlermeldung = "Pflichtfeld";
                else if (value.Length < 6)
                    PasswortFehlermeldung = "Mindestens 6 Zeichen";
                else
                    PasswortFehlermeldung = null;

                OnPropertyChanged(nameof(Passwort));
            }
        }



        private string? geburtstagFehlermeldung;
        public string? GeburtstagFehlermeldung
        {
            get => geburtstagFehlermeldung;
            set
            {
                geburtstagFehlermeldung = value;
                OnPropertyChanged(nameof(GeburtstagFehlermeldung));
            }
        }

        private string? vornameFehlermeldung;
        public string? VornameFehlermeldung
        {
            get => vornameFehlermeldung;
            set
            {
                vornameFehlermeldung = value;
                OnPropertyChanged(nameof(VornameFehlermeldung));
            }
        }

        private string? emailFehlermeldung;
        public string? EmailFehlermeldung
        {
            get => emailFehlermeldung;
            set
            {
                emailFehlermeldung = value;
                OnPropertyChanged(nameof(EmailFehlermeldung));
            }
        }


        private string? passwortFehlermeldung;
        public string? PasswortFehlermeldung
        {
            get => passwortFehlermeldung;
            set
            {
                passwortFehlermeldung = value;
                OnPropertyChanged(nameof(PasswortFehlermeldung));
            }
        }


        private string? benutzernameFehlermeldung;
        public string? BenutzernameFehlermeldung
        {
            get => benutzernameFehlermeldung;
            set
            {
                benutzernameFehlermeldung = value;
                OnPropertyChanged(nameof(BenutzernameFehlermeldung));
            }
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Schritt 1: Pflichtfelder prüfen
            if (string.IsNullOrWhiteSpace(Vorname) ||
                string.IsNullOrWhiteSpace(Benutzername) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(txtPasswort.Password))
            {
                MessageBox.Show("Bitte fülle alle Pflichtfelder aus!",
                                "Fehlende Eingabe", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Schritt 2: E-Mail-Format prüfen
            if (!Email.Contains("@") || !Email.Contains("."))
            {
                MessageBox.Show("Bitte gib eine gültige E-Mail-Adresse ein.",
                                "Ungültige E-Mail", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Geburtstag > DateTime.Today)
            {
                MessageBox.Show("Das Geburtsdatum darf nicht in der Zukunft liegen.",
                                "Ungültiger Geburtstag", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }




            try
            {
                string vorname = Vorname;
                string nachname = Benutzername;
                string passwort = txtPasswort.Password;
                string email = Email;
                string rolle = Benutzerrolle.ToString();

                using var conn = new Database().GetConnection();
                conn.Open();

                string query = @"INSERT INTO benutzer (vorname, nachname, passwort, email, rolle)
                         VALUES (@vorname, @nachname, @passwort, @email, @rolle)";
                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@vorname", vorname);
                cmd.Parameters.AddWithValue("@nachname", nachname);
                cmd.Parameters.AddWithValue("@passwort", passwort);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@rolle", rolle);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Benutzer erfolgreich registriert!");
                mainWindow?.SwitchToView(new LoginView());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler bei der Registrierung:\n" + ex.Message,
                                "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void txtPasswort_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Passwort = txtPasswort.Password;
        }
    }
}
