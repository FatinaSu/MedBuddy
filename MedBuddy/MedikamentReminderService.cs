using System;
using System.Collections.ObjectModel;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using MedBuddy.Model;
using MedBuddy.Views;
using System.IO;
using MedBuddy;

namespace MedBuddy
{
    public class MedikamentReminderService
    {
        private readonly ObservableCollection<Medikament> _medikamente;
        private readonly int _patientId;
        private readonly string _patientName;
        private DispatcherTimer _timer;
        private bool _reminderActive = false;

        public MedikamentReminderService(ObservableCollection<Medikament> medikamente, int patientId, string patientName = null)
        {
            _medikamente = medikamente;
            _patientId = patientId;
            _patientName = patientName;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(1) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_reminderActive) return; // Nur eine Erinnerung gleichzeitig

            var now = DateTime.Now.TimeOfDay;
            foreach (var med in _medikamente)
            {
                // Prüfe, ob jetzt Einnahmezeit ist (±1 Minute)
                if (Math.Abs((med.Uhrzeit - now).TotalMinutes) < 1)
                {
                    _reminderActive = true;
                    ZeigeErinnerung(med);
                    break;
                }
            }
        }

        private async void ZeigeErinnerung(Medikament med)
        {
            bool eingenommen = false;

            // Längeren, auffälligeren Sound abspielen
            var soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sounds", "reminder.wav");
            if (File.Exists(soundPath))
            {
                var player = new System.Media.SoundPlayer(soundPath);
                player.Play();
            }

            var popup = new MedikamentErinnerungWindow(med.Name, false, _patientName);
            popup.Owner = Application.Current.MainWindow;
            popup.Show();

            // Warte 1 Minute oder bis der Patient bestätigt oder abmeldet
            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);
                if (popup.Eingenommen || popup.Abgemeldet)
                {
                    break;
                }
            }
            popup.Close();

            if (popup.Abgemeldet)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow?.SwitchToView(new MedBuddy.Views.LoginView());
                });
                return;
            }

            if (popup.Eingenommen)
            {
                SpeichereEinnahme(med, "genommen");
                _reminderActive = false;
                return;
            }

            // Nach 5 Minuten erneut erinnern
            await Task.Delay(TimeSpan.FromMinutes(5));

            // Längeren, auffälligeren Sound abspielen
            if (File.Exists(soundPath))
            {
                var player = new System.Media.SoundPlayer(soundPath);
                player.Play();
            }

            var popup2 = new MedikamentErinnerungWindow(med.Name, true, _patientName);
            popup2.Owner = Application.Current.MainWindow;
            popup2.Show();

            for (int i = 0; i < 60; i++)
            {
                await Task.Delay(1000);
                if (popup2.Eingenommen || popup2.Abgemeldet)
                {
                    break;
                }
            }
            popup2.Close();

            if (popup2.Abgemeldet)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    mainWindow?.SwitchToView(new MedBuddy.Views.LoginView());
                });
                return;
            }

            if (popup2.Eingenommen)
            {
                SpeichereEinnahme(med, "genommen");
            }
            else
            {
                SpeichereEinnahme(med, "vergessen");
            }
            _reminderActive = false;
        }

        private void SpeichereEinnahme(Medikament med, string status)
        {
            var repo = new MedBuddy.Model.EinnahmeRepository();
            var einnahme = new MedBuddy.Model.EinnahmeViewModel
            {
                Datum = DateTime.Today,
                MedikamentName = med.Name,
                Status = status,
                Hinweis = ""
            };
            repo.SpeichereEinnahmen(new System.Collections.Generic.List<MedBuddy.Model.EinnahmeViewModel> { einnahme }, _patientId);
        }

        public void Stop()
        {
            
        }
    }
} 