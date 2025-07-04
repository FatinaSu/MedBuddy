using System;
using System.Collections.Generic;

public class TagebuchEintrag
{
    public int Id { get; set; } // Optional
    public int BenutzerId { get; set; }

    public DateTime Datum { get; set; }
    public string Eintrag { get; set; } = string.Empty;
    public string Stimmung { get; set; } = string.Empty;
    public List<string> Symptome { get; set; } = new();
}
