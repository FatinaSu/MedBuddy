using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MedBuddy.Model
{
    public class MedikamentRepository
    {
        public void SpeichereMedikament(Medikament medikament)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"
                INSERT INTO medikamente (name, haeufigkeit, uhrzeit, benutzer_id)
                VALUES (@name, @haeufigkeit, @uhrzeit, @benutzerId)";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", medikament.Name);
            cmd.Parameters.AddWithValue("@haeufigkeit", medikament.Haeufigkeit);
            cmd.Parameters.AddWithValue("@uhrzeit", medikament.Uhrzeit);
            cmd.Parameters.AddWithValue("@benutzerId", medikament.BenutzerId);

            cmd.ExecuteNonQuery();
        }

        public List<Medikament> LadeMedikamente(int benutzerId)
        {
            var liste = new List<Medikament>();

            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "SELECT * FROM medikamente WHERE benutzer_id = @benutzerId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@benutzerId", benutzerId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var medikament = new Medikament
                {
                    Id = reader.GetInt32("id"),
                    Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "" : reader.GetString("name"),
                    Haeufigkeit = reader.IsDBNull(reader.GetOrdinal("haeufigkeit")) ? "" : reader.GetString("haeufigkeit"),
                    Uhrzeit = reader.IsDBNull(reader.GetOrdinal("uhrzeit")) ? TimeSpan.Zero : reader.GetTimeSpan("uhrzeit"),
                    BenutzerId = benutzerId
                };
                liste.Add(medikament);
            }

            return liste;
        }

        public void LoescheMedikament(int medikamentId)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "DELETE FROM medikamente WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", medikamentId);
            cmd.ExecuteNonQuery();
        }


        public void LöscheMedikamentById(int id)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "DELETE FROM medikamente WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }


        public void LöscheMedikamentMitId(int id)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "DELETE FROM medikamente WHERE id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        public void AktualisiereMedikament(Medikament medikament)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"
        UPDATE medikamente
        SET name = @name, haeufigkeit = @haeufigkeit, uhrzeit = @uhrzeit
        WHERE id = @id";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", medikament.Name);
            cmd.Parameters.AddWithValue("@haeufigkeit", medikament.Haeufigkeit);
            cmd.Parameters.AddWithValue("@uhrzeit", medikament.Uhrzeit);
            cmd.Parameters.AddWithValue("@id", medikament.Id);

            cmd.ExecuteNonQuery();
        }



    }
}
