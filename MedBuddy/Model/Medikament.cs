using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedBuddy.Model
{
    public class Medikament
    {
        public Medikament()
        {
            Name = string.Empty;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Haeufigkeit { get; set; } = string.Empty; // z. B. "Täglich", "2x täglich"
        public TimeSpan Uhrzeit { get; set; }

        public int BenutzerId { get; set; } // wichtig für die Zuordnung zu einem Patienten
    }
}
