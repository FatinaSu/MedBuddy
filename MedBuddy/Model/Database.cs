using MySql.Data.MySqlClient;
using System;

namespace MedBuddy.Model
{
    internal class Database
    {
        // Setze hier dein echtes Passwort ein
        private readonly string connectionString = "Server=localhost;Database=medbuddy_db;Uid=root;Pwd=Medbuddy2025!;";

        // Gibt eine offene Verbindung zurück
        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        // Testet die Verbindung und gibt true/false zurück
        public bool TestConnection()
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                Console.WriteLine("Verbindung erfolgreich.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Verbindungsfehler: " + ex.Message);
                return false;
            }
        }
    }
}
