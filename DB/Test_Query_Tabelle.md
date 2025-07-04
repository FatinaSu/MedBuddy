# MedBuddy Datenbank - Test Query Tabelle

## Übersicht aller SQL-Tests mit Erwartungen

| **Test-Kategorie** | **SQL-Query** | **Erwartetes Ergebnis** | **Beschreibung** |
|-------------------|---------------|-------------------------|------------------|
| **1. VERBINDUNGSTEST** |
| Verbindung prüfen | `SELECT 1 as connection_test;` | `1` | Datenbankverbindung funktioniert |
| Datenbankexistenz | `SELECT DATABASE() = 'medbuddy_db';` | `1` | Richtige Datenbank ausgewählt |
| **2. TABELLENSTRUKTUR** |
| Tabellen auflisten | `SHOW TABLES;` | `benutzer, medikamente, einnahmen, tagebuch, patienten_medikamente` | Alle 5 Tabellen vorhanden |
| Benutzer-Schema | `DESCRIBE benutzer;` | Spalten: `id, vorname, nachname, passwort, email, rolle` | Korrekte Tabellenstruktur |
| Medikamente-Schema | `DESCRIBE medikamente;` | Spalten: `id, name, haeufigkeit, uhrzeit, benutzer_id` | Korrekte Tabellenstruktur |
| **3. REFERENTIELLE INTEGRITÄT** |
| Waisendatensätze Medikamente | ```sql<br>SELECT COUNT(*) as orphaned<br>FROM medikamente m<br>LEFT JOIN benutzer b ON m.benutzer_id = b.id<br>WHERE b.id IS NULL;``` | `0` | Keine Waisendatensätze |
| Waisendatensätze Einnahmen | ```sql<br>SELECT COUNT(*) as orphaned<br>FROM einnahmen e<br>LEFT JOIN benutzer b ON e.patient_id = b.id<br>WHERE b.id IS NULL;``` | `0` | Keine Waisendatensätze |
| Waisendatensätze Tagebuch | ```sql<br>SELECT COUNT(*) as orphaned<br>FROM tagebuch t<br>LEFT JOIN benutzer b ON t.benutzer_id = b.id<br>WHERE b.id IS NULL;``` | `0` | Keine Waisendatensätze |
| **4. DATENVALIDIERUNG** |
| NULL Vorname | `SELECT COUNT(*) FROM benutzer WHERE vorname IS NULL;` | `0` | Keine leeren Vornamen |
| NULL Nachname | `SELECT COUNT(*) FROM benutzer WHERE nachname IS NULL;` | `0` | Keine leeren Nachnamen |
| NULL Medikamentenname | `SELECT COUNT(*) FROM medikamente WHERE name IS NULL OR TRIM(name) = '';` | `0` | Keine leeren Medikamentennamen |
| NULL Passwort | `SELECT COUNT(*) FROM benutzer WHERE passwort IS NULL OR TRIM(passwort) = '';` | `0` | Keine leeren Passwörter |
| **5. DATUMSVALIDIERUNG** |
| Ungültige Tagebuch-Daten | ```sql<br>SELECT COUNT(*) FROM tagebuch<br>WHERE datum IS NULL OR datum = '0000-00-00';``` | `0` | Keine ungültigen Datumswerte |
| Ungültige Einnahmen-Daten | ```sql<br>SELECT COUNT(*) FROM einnahmen<br>WHERE datum IS NULL OR datum = '0000-00-00';``` | `0` | Keine ungültigen Datumswerte |
| **6. ZEITVALIDIERUNG** |
| Ungültige Uhrzeiten | ```sql<br>SELECT COUNT(*) FROM medikamente<br>WHERE uhrzeit IS NULL OR uhrzeit < '00:00:00'<br>OR uhrzeit > '23:59:59';``` | `0` | Keine ungültigen Uhrzeitwerte |
| **7. GESCHÄFTSREGELN** |
| Doppelte Benutzernamen | ```sql<br>SELECT COUNT(*) FROM (<br>SELECT vorname, nachname, COUNT(*) as anzahl<br>FROM benutzer<br>GROUP BY vorname, nachname<br>HAVING COUNT(*) > 1<br>) as duplicates;``` | `0` | Keine doppelten Benutzernamen |
| Gültige Rollen | ```sql<br>SELECT DISTINCT rolle FROM benutzer;``` | `Arzt, Patient` | Nur gültige Rollen vorhanden |
| **8. DATENKONSISTENZ** |
| Inkonsistente Medikamente | ```sql<br>SELECT COUNT(*) FROM (<br>SELECT DISTINCT e.medikament_name<br>FROM einnahmen e<br>LEFT JOIN medikamente m ON e.medikament_name = m.name<br>WHERE m.name IS NULL<br>) as inconsistent;``` | `0` | Alle Medikamente in Einnahmen existieren |
| Zukünftige Einnahmen | `SELECT COUNT(*) FROM einnahmen WHERE datum > CURDATE();` | `0` | Keine zukünftigen Einnahmen |
| **9. PERFORMANCE** |
| Indizes Benutzer | `SHOW INDEX FROM benutzer;` | Mindestens `PRIMARY KEY` auf `id` | Optimale Indizierung |
| Indizes Medikamente | `SHOW INDEX FROM medikamente;` | Mindestens `PRIMARY KEY` auf `id` | Optimale Indizierung |
| **10. VOLLSTÄNDIGKEIT** |
| Datensatz-Anzahl | ```sql<br>SELECT 'benutzer' as tabelle, COUNT(*) as anzahl FROM benutzer<br>UNION ALL<br>SELECT 'medikamente' as tabelle, COUNT(*) as anzahl FROM medikamente<br>UNION ALL<br>SELECT 'einnahmen' as tabelle, COUNT(*) as anzahl FROM einnahmen<br>UNION ALL<br>SELECT 'tagebuch' as tabelle, COUNT(*) as anzahl FROM tagebuch;``` | Positive Zahlen | Alle Tabellen enthalten Daten |

## Detaillierte Test-Queries mit Erklärungen

### **Kritische Tests (Muss bestanden werden)**

| **Test** | **Query** | **Erwartung** | **Bedeutung** |
|----------|-----------|---------------|---------------|
| **Verbindung** | `SELECT 1;` | `1` | Datenbank erreichbar |
| **Referentielle Integrität** | ```sql<br>SELECT COUNT(*) FROM medikamente m<br>LEFT JOIN benutzer b ON m.benutzer_id = b.id<br>WHERE b.id IS NULL;``` | `0` | Keine verwaisten Datensätze |
| **Datenvalidierung** | `SELECT COUNT(*) FROM benutzer WHERE vorname IS NULL;` | `0` | Alle Pflichtfelder gefüllt |
| **Sicherheit** | `SELECT COUNT(*) FROM benutzer WHERE passwort IS NULL;` | `0` | Keine leeren Passwörter |

### **Warnung-Tests (Sollten bestanden werden)**

| **Test** | **Query** | **Erwartung** | **Bedeutung** |
|----------|-----------|---------------|---------------|
| **Doppelte Namen** | ```sql<br>SELECT COUNT(*) FROM (<br>SELECT vorname, nachname<br>FROM benutzer<br>GROUP BY vorname, nachname<br>HAVING COUNT(*) > 1<br>) as dup;``` | `0` | Eindeutige Benutzernamen |
| **Zukünftige Daten** | `SELECT COUNT(*) FROM einnahmen WHERE datum > CURDATE();` | `0` | Keine zukünftigen Einträge |
| **Alte Daten** | `SELECT COUNT(*) FROM einnahmen WHERE datum < DATE_SUB(CURDATE(), INTERVAL 2 YEAR);` | `< 1000` | Nicht zu viele alte Daten |

### **Info-Tests (Zur Dokumentation)**

| **Test** | **Query** | **Erwartung** | **Bedeutung** |
|----------|-----------|---------------|---------------|
| **Tabellenanzahl** | `SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = 'medbuddy_db';` | `5` | Alle Tabellen vorhanden |
| **Benutzeranzahl** | `SELECT COUNT(*) FROM benutzer;` | `> 0` | Mindestens ein Benutzer |
| **Rollenverteilung** | ```sql<br>SELECT rolle, COUNT(*) as anzahl<br>FROM benutzer<br>GROUP BY rolle;``` | `Arzt: >0, Patient: >0` | Beide Rollen vertreten |

## Automatisierter Vollständigkeitstest

```sql
-- Vollständiger Test aller kritischen Aspekte
SELECT 
    'VERBINDUNG' as test_kategorie,
    CASE WHEN (SELECT 1) = 1 THEN 'BESTANDEN' ELSE 'FEHLGESCHLAGEN' END as ergebnis

UNION ALL

SELECT 
    'REFERENTIELLE_INTEGRITÄT' as test_kategorie,
    CASE 
        WHEN (
            SELECT COUNT(*) FROM medikamente m
            LEFT JOIN benutzer b ON m.benutzer_id = b.id
            WHERE b.id IS NULL
        ) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis

UNION ALL

SELECT 
    'DATENVALIDIERUNG' as test_kategorie,
    CASE 
        WHEN (
            SELECT COUNT(*) FROM benutzer 
            WHERE vorname IS NULL OR nachname IS NULL
        ) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis

UNION ALL

SELECT 
    'SICHERHEIT' as test_kategorie,
    CASE 
        WHEN (
            SELECT COUNT(*) FROM benutzer 
            WHERE passwort IS NULL OR TRIM(passwort) = ''
        ) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis;
```

## Test-Ausführungsreihenfolge

1. **Verbindungstest** → Muss erfolgreich sein
2. **Tabellenstruktur** → Alle Tabellen müssen existieren
3. **Referentielle Integrität** → Keine Waisendatensätze
4. **Datenvalidierung** → Keine NULL-Werte in Pflichtfeldern
5. **Sicherheitstests** → Keine leeren Passwörter
6. **Geschäftsregeln** → Eindeutigkeit und gültige Werte
7. **Performance** → Indizes vorhanden
8. **Vollständigkeit** → Daten vorhanden

## Fehlerbehandlung

| **Fehler** | **Ursache** | **Lösung** |
|------------|-------------|------------|
| `Access denied` | Falsche Zugangsdaten | Passwort überprüfen |
| `Unknown database` | Datenbank existiert nicht | `CREATE DATABASE medbuddy_db;` |
| `Table doesn't exist` | Tabellen fehlen | Tabellen aus Code erstellen |
| `Waisendatensätze > 0` | Referentielle Integrität verletzt | Verwaiste Datensätze bereinigen |
| `NULL-Werte > 0` | Datenvalidierung fehlgeschlagen | Pflichtfelder nachfüllen |

## Erfolgreicher Test-Report

```
+------------------------+-------------+----------+
| test_kategorie         | fehler_anzahl | ergebnis |
+------------------------+-------------+----------+
| VERBINDUNG             | 0           | BESTANDEN |
| REFERENTIELLE_INTEGRITÄT | 0        | BESTANDEN |
| DATENVALIDIERUNG       | 0           | BESTANDEN |
| SICHERHEIT             | 0           | BESTANDEN |
+------------------------+-------------+----------+
```

**Alle Tests bestanden = Datenbank ist konsistent und integer!** 