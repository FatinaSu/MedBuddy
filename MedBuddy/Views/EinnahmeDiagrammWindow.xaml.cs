using LiveCharts;
using LiveCharts.Wpf;
using MedBuddy.Model;
using System.Windows;
using System.Windows.Media;

namespace MedBuddy.Views
{
    public partial class EinnahmeDiagrammWindow : Window
    {
        private DateTime _startDatum;
        private DateTime _endDatum => _startDatum.AddDays(6);
        private int _patientId;

        public SeriesCollection SeriesCollection { get; set; }
        public List<string> Labels { get; set; }

        public EinnahmeDiagrammWindow(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            _startDatum = DateTime.Today.AddDays(-6);
            LadeUndSetzeDaten();
        }

        private void LadeUndSetzeDaten()
        {
            var repo = new EinnahmeRepository();
            var alleEinnahmen = repo.LadeEinnahmenFuerPatient(_patientId);
            var tage = Enumerable.Range(0, 7)
                .Select(i => _startDatum.AddDays(i))
                .ToList();
            var medikamente = alleEinnahmen
                .Select(e => e.MedikamentName)
                .Distinct()
                .OrderBy(n => n)
                .ToList();
            SeriesCollection = new SeriesCollection();
            var farben = new[] { Colors.SteelBlue, Colors.Orange, Colors.MediumSeaGreen, Colors.MediumVioletRed, Colors.Goldenrod, Colors.MediumSlateBlue, Colors.Teal, Colors.Crimson };
            int colorIdx = 0;
            foreach (var med in medikamente)
            {
                var werte = tage.Select(tag =>
                {
                    var einnahme = alleEinnahmen.FirstOrDefault(e => e.Datum.Date == tag.Date && e.MedikamentName == med);
                    if (einnahme == null) return 0;
                    return einnahme.Status == "genommen" ? 1 : 0;
                });
                var series = new LineSeries
                {
                    Title = med,
                    Values = new ChartValues<int>(werte),
                    Stroke = new SolidColorBrush(farben[colorIdx % farben.Length]),
                    Fill = Brushes.Transparent,
                    PointGeometrySize = 14
                };
                SeriesCollection.Add(series);
                colorIdx++;
            }
            var tagebuchRepo = new TagebuchRepository();
            var stimmungsWerte = tage.Select(tag =>
            {
                var eintrag = tagebuchRepo.LadeEintrag(tag, _patientId);
                if (Enum.TryParse<Stimmung>(eintrag.Stimmung, out var stimmung))
                {
                    return stimmung switch
                    {
                        Stimmung.Super => 4,
                        Stimmung.Gut => 3,
                        Stimmung.Gereizt => 2,
                        Stimmung.Schlecht => 1,
                        Stimmung.Müde => 0,
                        _ => 0
                    };
                }
                return 0;
            });
            SeriesCollection.Add(new LineSeries
            {
                Title = "Stimmung",
                Values = new ChartValues<int>(stimmungsWerte),
                Stroke = new SolidColorBrush(Colors.DarkSlateGray),
                Fill = Brushes.Transparent,
                PointGeometrySize = 10,
                StrokeDashArray = new DoubleCollection { 2, 2 },
                ScalesYAt = 1
            });
            Labels = tage.Select(t => t.ToString("dd.MM.")).ToList();
            DataContext = null;
            DataContext = this;
            IntervallText.Text = $"{_startDatum:dd.MM.yyyy} - {_endDatum:dd.MM.yyyy}";
        }

        private void VorherigeWoche_Click(object sender, RoutedEventArgs e)
        {
            _startDatum = _startDatum.AddDays(-7);
            LadeUndSetzeDaten();
        }

        private void NaechsteWoche_Click(object sender, RoutedEventArgs e)
        {
            if (_startDatum.AddDays(7) <= DateTime.Today)
            {
                _startDatum = _startDatum.AddDays(7);
                LadeUndSetzeDaten();
            }
        }
    }
} 