using MySql.Data.MySqlClient;
using System;

namespace MedBuddy.Model
{
    internal class BenutzerRepository
    {
        public bool LoginErfolgreich(string name, string passwort)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"SELECT COUNT(*) FROM benutzer 
                     WHERE CONCAT(vorname, ' ', nachname) = @name 
                     AND passwort = @passwort";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@passwort", passwort);

            var result = Convert.ToInt32(cmd.ExecuteScalar());
            return result > 0;
        }

        public string? GetBenutzerRolle(string name, string passwort)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"SELECT rolle FROM benutzer 
                     WHERE CONCAT(vorname, ' ', nachname) = @name 
                     AND passwort = @passwort";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@passwort", passwort);

            var result = cmd.ExecuteScalar();
            return result?.ToString();
        }

        public int GetBenutzerId(string name, string passwort)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"SELECT id FROM benutzer 
                     WHERE CONCAT(vorname, ' ', nachname) = @name 
                     AND passwort = @passwort";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@passwort", passwort);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : -1;
        }



    }
}

