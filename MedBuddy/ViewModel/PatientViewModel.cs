using System;

namespace MedBuddy.ViewModel
{
    public class PatientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Telefon { get; set; } = string.Empty;
        public DateTime Geburtsdatum { get; set; }
    }
}
