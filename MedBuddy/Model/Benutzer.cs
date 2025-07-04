using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedBuddy.Model
{
    public class Benutzer
    {
        public Benutzer()
        {
            Name = string.Empty;
            Vorname = string.Empty;
            Adresse = string.Empty;
            Password = string.Empty;
        }

        public int ID { get; set; }
        public Benutzerrolle Benutzerrolle { get; set; }
        public string Name { get; set; }
        public string Vorname { get; set; }
        public TimeSpan Geburtstag { get; set; }
        public string Adresse { get; set; }
        public double Telefon { get; set; }
        public string Password { get; set; } // TODO encrypten 
    }

    public enum Benutzerrolle
    {
        Arzt,
        Patient
    }
}
