using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MedBuddy.Model
{
    public class TagebuchRepository
    {
        public TagebuchEintrag LadeEintrag(DateTime datum, int benutzerId)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = "SELECT * FROM tagebuch WHERE datum = @datum AND benutzer_id = @id";
            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@datum", datum.Date);
            cmd.Parameters.AddWithValue("@id", benutzerId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new TagebuchEintrag
                {
                    Datum = datum,
                    Eintrag = reader.GetString("eintrag"),
                    Stimmung = reader.GetString("stimmung"),
                    Symptome = new List<string>(reader.GetString("symptome").Split(','))
                };
            }

            return new TagebuchEintrag
            {
                BenutzerId = benutzerId,
                Datum = datum,
                Stimmung = " ",
                Symptome = new List<string>()
            };
        }



        public void SpeichereEintrag(TagebuchEintrag eintrag, int benutzerId)
        {
            using var conn = new Database().GetConnection();
            conn.Open();

            string query = @"
        INSERT INTO tagebuch (datum, eintrag, symptome, stimmung, benutzer_id)
        VALUES (@datum, @eintrag, @symptome, @stimmung, @benutzerId)
        ON DUPLICATE KEY UPDATE
            eintrag = @eintrag,
            symptome = @symptome,
            stimmung = @stimmung";

            using var cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@datum", eintrag.Datum.Date);
            cmd.Parameters.AddWithValue("@eintrag", eintrag.Eintrag);
            cmd.Parameters.AddWithValue("@symptome", string.Join(",", eintrag.Symptome));
            cmd.Parameters.AddWithValue("@stimmung", eintrag.Stimmung);
            cmd.Parameters.AddWithValue("@benutzerId", benutzerId);

            cmd.ExecuteNonQuery();
        }


    }
}

