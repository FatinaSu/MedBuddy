using MedBuddy.Model;
using MedBuddy.ViewModel;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;


namespace MedBuddy.Views
{
    /// <summary>
    /// Interaktionslogik für TagebuchView.xaml
    /// </summary>
    public partial class TagebuchView : UserControl, ITab, INotifyPropertyChanged
    {
        private SymptomeViewModel symptomeViewModel;
        private readonly int benutzerId;

        private readonly TagebuchRepository repository = new();


        public TagebuchView(int benutzerId)
        {
            InitializeComponent();
            this.benutzerId = benutzerId;

            DataContext = this;
            symptomeViewModel = new SymptomeViewModel();
            Symptome = symptomeViewModel.GetSymptomItems();
            LadeEintrag();
            BearbeitenSpeichernText = "Bearbeiten";
        }


        public string Title => "Tagebuch";

        private void LadeEintrag()
        {
            var eintrag = repository.LadeEintrag(aktuellesDatum, benutzerId);


            TagebuchEintrag = eintrag.Eintrag;
            if (Enum.TryParse<Stimmung>(eintrag.Stimmung, out var parsedStimmung))
            {
                Stimmung = parsedStimmung;
            }
            else
            {
                Stimmung = Stimmung.Gut;
            }

            foreach (var symptom in Symptome)
            {
                symptom.IsSelected = eintrag.Symptome.Contains(symptom.Name);
            }
        }



        private string bearbeitenSpeichernText = "Bearbeiten";
        public string BearbeitenSpeichernText
        {
            get => bearbeitenSpeichernText;
            set
            {
                bearbeitenSpeichernText = value;
                OnPropertyChanged(nameof(BearbeitenSpeichernText));
            }
        }


        private string bearbeitenSpeichernIcon = "PencilOutline";
        public string BearbeitenSpeichernIcon
        {
            get => bearbeitenSpeichernIcon;
            set
            {
                bearbeitenSpeichernIcon = value;
                OnPropertyChanged(nameof(BearbeitenSpeichernIcon));
            }
        }



        public string AktuellesDatumAnzeige => $"{AktuellesDatum:dd.MM.yyyy}";

        private DateTime aktuellesDatum = DateTime.Today;
        public DateTime AktuellesDatum
        {
            get => aktuellesDatum;
            set
            {
                aktuellesDatum = value;
                OnPropertyChanged(nameof(AktuellesDatum));
                OnPropertyChanged(nameof(AktuellesDatumAnzeige));
                LadeEintrag();
            }
        }

        private string tagebuchEintrag = "";
        public string TagebuchEintrag
        {
            get => tagebuchEintrag;
            set { tagebuchEintrag = value; OnPropertyChanged(nameof(TagebuchEintrag)); }
        }

        private bool istBearbeitbar = false;
        public bool IstBearbeitbar
        {
            get => istBearbeitbar;
            set { istBearbeitbar = value; OnPropertyChanged(nameof(IstBearbeitbar)); }
        }

        public ObservableCollection<SymptomItem> Symptome { get; set; }

        public Array StimmungItems => Enum.GetValues(typeof(Stimmung));

        private Stimmung stimmung = Stimmung.Gut;
        public Stimmung Stimmung
        {
            get => stimmung;
            set
            {
                stimmung = value;
                OnPropertyChanged(nameof(Stimmung));
            }
        }

        private void BtnBearbeiten_Click(object sender, RoutedEventArgs e)
        {
            IstBearbeitbar = !IstBearbeitbar;
        }

        private void BtnZurueck_Click(object sender, RoutedEventArgs e)
        {
            AktuellesDatum = AktuellesDatum.AddDays(-1);
        }

        private void BtnWeiter_Click(object sender, RoutedEventArgs e)
        {
            AktuellesDatum = AktuellesDatum.AddDays(1);
        }


        private void BtnBearbeitenSpeichern_Click(object sender, RoutedEventArgs e)
        {
            if (IstBearbeitbar)
            {
                // Speichern
                var eintrag = new TagebuchEintrag
                {
                    BenutzerId = benutzerId,
                    Datum = AktuellesDatum,
                    Eintrag = TagebuchEintrag,
                    Stimmung = Stimmung.ToString(),
                    Symptome = Symptome.Where(s => s.IsSelected).Select(s => s.Name).ToList()
                };

                repository.SpeichereEintrag(eintrag, benutzerId);

                ZeigeToast();
                Storyboard sb = (Storyboard)FindResource("SpeicherAnimation");
                sb.Begin();


                BearbeitenSpeichernText = "Bearbeiten";
                BearbeitenSpeichernIcon = "PencilOutline";
            }
            else
            {
                BearbeitenSpeichernText = "Speichern";
                BearbeitenSpeichernIcon = "CheckBold";
            }

            IstBearbeitbar = !IstBearbeitbar;
        }


        private void ZeigeToast()
        {
            ToastMessage.Visibility = Visibility.Visible;
            ToastMessage.Opacity = 1;

            var fadeInOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                BeginTime = TimeSpan.FromSeconds(1.5),
                Duration = TimeSpan.FromSeconds(1),
                FillBehavior = FillBehavior.Stop
            };

            fadeInOut.Completed += (s, e) =>
            {
                ToastMessage.Visibility = Visibility.Collapsed;
                ToastMessage.Opacity = 1;
            };

            ToastMessage.BeginAnimation(OpacityProperty, fadeInOut);
        }



        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
