using MedBuddy.ViewModel;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MedBuddy.Model
{
    public class PatientRepository
    {
        public List<PatientViewModel> LadeAllePatienten()
        {
            var liste = new List<PatientViewModel>();

            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "SELECT id, vorname, nachname FROM benutzer WHERE rolle = 'Patient'";
            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var patient = new PatientViewModel
                {
                    Id = reader.GetInt32("id"),
                    Name = $"{reader.GetString("vorname")} {reader.GetString("nachname")}"
                };
                liste.Add(patient);
            }

            return liste;
        }
    }
}
