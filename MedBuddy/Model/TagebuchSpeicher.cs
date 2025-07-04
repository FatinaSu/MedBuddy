using MedBuddy.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

public class TagebuchSpeicher
{
    public void SpeichereEintrag(TagebuchEintrag eintrag)
    {
        using var conn = new Database().GetConnection();
        conn.Open();

        string symptomeString = string.Join(",", eintrag.Symptome);

        string query = @"INSERT INTO tagebuch (datum, eintrag, symptome, stimmung)
                         VALUES (@datum, @eintrag, @symptome, @stimmung)
                         ON DUPLICATE KEY UPDATE
                         eintrag = VALUES(eintrag),
                         symptome = VALUES(symptome),
                         stimmung = VALUES(stimmung);";

        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@datum", eintrag.Datum.Date);
        cmd.Parameters.AddWithValue("@eintrag", eintrag.Eintrag);
        cmd.Parameters.AddWithValue("@symptome", symptomeString);
        cmd.Parameters.AddWithValue("@stimmung", eintrag.Stimmung);
        cmd.ExecuteNonQuery();
    }

    public TagebuchEintrag LadeEintrag(DateTime datum)
    {
        using var conn = new Database().GetConnection();
        conn.Open();

        string query = "SELECT * FROM tagebuch WHERE datum = @datum";
        using var cmd = new MySqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@datum", datum.Date);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new TagebuchEintrag
            {
                Datum = reader.GetDateTime("datum"),
                Eintrag = reader.GetString("eintrag"),
                Stimmung = reader.GetString("stimmung"),
                Symptome = new List<string>(reader.GetString("symptome").Split(','))
            };
        }

        return new TagebuchEintrag { Datum = datum };
    }
}

