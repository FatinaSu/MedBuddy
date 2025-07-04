using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace MedBuddy.Model
{
    public class PatientenMedikamentRepository
    {
        public void MedikamentHinzufuegen(int benutzerId, string name, string dosis, string haeufigkeit)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "INSERT INTO patienten_medikamente (benutzer_id, medikament, dosis, haeufigkeit) VALUES (@id, @name, @dosis, @haeufigkeit)";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", benutzerId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@dosis", dosis);
            cmd.Parameters.AddWithValue("@haeufigkeit", haeufigkeit);
            cmd.ExecuteNonQuery();
        }

        public List<string> LadeMedikamente(int benutzerId)
        {
            var result = new List<string>();
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "SELECT medikament FROM patienten_medikamente WHERE benutzer_id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", benutzerId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(reader.GetString("medikament"));
            }
            return result;
        }
    }
}

