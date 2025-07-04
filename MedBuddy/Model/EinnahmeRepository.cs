using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MedBuddy.Model
{
    public class EinnahmeRepository
    {
        public List<EinnahmeViewModel> LadeEinnahmenFuerPatient(int patientId)
        {
            var result = new List<EinnahmeViewModel>();
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"SELECT datum, medikament_name, status, hinweis
                             FROM einnahmen
                             WHERE patient_id = @patientId";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);

            var vorhandene = new List<EinnahmeViewModel>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    vorhandene.Add(new EinnahmeViewModel
                    {
                        Datum = reader.GetDateTime("datum"),
                        MedikamentName = reader.GetString("medikament_name"),
                        Status = reader.GetString("status"),
                        Hinweis = reader.GetString("hinweis")
                    });
                }
            }

            // Medikamente des Patienten laden
            var medRepo = new MedikamentRepository();
            var medikamente = medRepo.LadeMedikamente(patientId);
            // Die letzten 7 Tage
            var tage = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i)).ToList();
            var neueEinnahmen = new List<EinnahmeViewModel>();
            foreach (var tag in tage)
            {
                foreach (var med in medikamente)
                {
                    if (!vorhandene.Any(e => e.Datum.Date == tag.Date && e.MedikamentName == med.Name))
                    {
                        // Fehlender Eintrag: als 'vergessen' anlegen
                        neueEinnahmen.Add(new EinnahmeViewModel
                        {
                            Datum = tag,
                            MedikamentName = med.Name,
                            Status = "vergessen",
                            Hinweis = ""
                        });
                    }
                }
            }
            if (neueEinnahmen.Count > 0)
            {
                SpeichereEinnahmen(neueEinnahmen, patientId);
                vorhandene.AddRange(neueEinnahmen);
            }
            return vorhandene.OrderBy(e => e.Datum).ThenBy(e => e.MedikamentName).ToList();
        }

        // Speichert die geänderten Einnahmen (Platzhalter, später echte DB-Anbindung)
        public void SpeichereEinnahmen(List<EinnahmeViewModel> einnahmen, int patientId)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            foreach (var einnahme in einnahmen)
            {
                string query = @"
                    INSERT INTO einnahmen (patient_id, datum, medikament_name, status, hinweis)
                    VALUES (@patientId, @datum, @medikamentName, @status, @hinweis)
                    ON DUPLICATE KEY UPDATE
                        status = @status,
                        hinweis = @hinweis;";

                using var cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientId", patientId);
                cmd.Parameters.AddWithValue("@datum", einnahme.Datum.Date);
                cmd.Parameters.AddWithValue("@medikamentName", einnahme.MedikamentName);
                cmd.Parameters.AddWithValue("@status", einnahme.Status);
                cmd.Parameters.AddWithValue("@hinweis", einnahme.Hinweis ?? "");
                cmd.ExecuteNonQuery();
            }
        }

        public bool HatAlleEingenommen(int patientId, DateTime tag)
        {
            using var conn = new Database().GetConnection();
            conn.Open();
            string query = @"SELECT COUNT(*) FROM einnahmen WHERE patient_id = @patientId AND datum = @datum AND status != 'eingenommen'";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@patientId", patientId);
            cmd.Parameters.AddWithValue("@datum", tag.Date);
            var count = Convert.ToInt32(cmd.ExecuteScalar());
            return count == 0;
        }
    }
} 