using System;

namespace MedBuddy.Model
{
    public class EinnahmeViewModel
    {
        public DateTime Datum { get; set; }
        public string MedikamentName { get; set; }
        public string Status { get; set; }
        public bool IstAusreichend { get; set; }
        public bool IstUnzureichend { get; set; }
        public string Hinweis { get; set; }
    }
} 