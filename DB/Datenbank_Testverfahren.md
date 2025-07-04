# MedBuddy Datenbank - Testverfahren für Konsistenz und Integrität

## 1. Übersicht der Datenbankstruktur

Basierend auf der Codeanalyse wurden folgende Tabellen identifiziert:

- **benutzer** - Benutzerkonten (Ärzte und Patienten)
- **medikamente** - Medikamentendaten
- **einnahmen** - Medikamenteneinnahmen
- **tagebuch** - Patiententagebuch
- **patienten_medikamente** - Verknüpfung Patienten-Medikamente

## 2. Datenbankverbindung testen

### 2.1 Verbindungstest
```sql
-- Test der Datenbankverbindung
SELECT 1 as connection_test;
```

**Erwartetes Ergebnis:** Eine Zeile mit dem Wert 1

### 2.2 Datenbankexistenz prüfen
```sql
-- Prüfen ob Datenbank existiert
SHOW DATABASES LIKE 'medbuddy_db';
```

**Erwartetes Ergebnis:** Eine Zeile mit 'medbuddy_db'

## 3. Tabellenstruktur-Tests

### 3.1 Tabellenexistenz prüfen
```sql
-- Alle Tabellen auflisten
SHOW TABLES FROM medbuddy_db;

-- Erwartete Tabellen:
-- benutzer
-- medikamente  
-- einnahmen
-- tagebuch
-- patienten_medikamente
```

### 3.2 Tabellenschema prüfen
```sql
-- Schema der benutzer-Tabelle
DESCRIBE benutzer;

-- Schema der medikamente-Tabelle  
DESCRIBE medikamente;

-- Schema der einnahmen-Tabelle
DESCRIBE einnahmen;

-- Schema der tagebuch-Tabelle
DESCRIBE tagebuch;

-- Schema der patienten_medikamente-Tabelle
DESCRIBE patienten_medikamente;
```

## 4. Datenintegrität-Tests

### 4.1 Referentielle Integrität

#### 4.1.1 Fremdschlüssel-Beziehungen prüfen
```sql
-- Prüfen ob alle medikamente.benutzer_id in benutzer.id existieren
SELECT m.id, m.benutzer_id, b.id as benutzer_exists
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL;

-- Erwartetes Ergebnis: Keine Zeilen (alle benutzer_id sollten existieren)

-- Prüfen ob alle einnahmen.patient_id in benutzer.id existieren
SELECT e.patient_id, b.id as benutzer_exists
FROM einnahmen e
LEFT JOIN benutzer b ON e.patient_id = b.id
WHERE b.id IS NULL;

-- Erwartetes Ergebnis: Keine Zeilen

-- Prüfen ob alle tagebuch.benutzer_id in benutzer.id existieren
SELECT t.benutzer_id, b.id as benutzer_exists
FROM tagebuch t
LEFT JOIN benutzer b ON t.benutzer_id = b.id
WHERE b.id IS NULL;

-- Erwartetes Ergebnis: Keine Zeilen
```

#### 4.1.2 Waisendatensätze finden
```sql
-- Waisendatensätze in medikamente finden
SELECT COUNT(*) as orphaned_medikamente
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL;

-- Waisendatensätze in einnahmen finden
SELECT COUNT(*) as orphaned_einnahmen
FROM einnahmen e
LEFT JOIN benutzer b ON e.patient_id = b.id
WHERE b.id IS NULL;
```

### 4.2 Datentyp-Validierung

#### 4.2.1 Datumsvalidierung
```sql
-- Prüfen auf ungültige Datumswerte
SELECT id, datum, 'tagebuch' as tabelle
FROM tagebuch 
WHERE datum IS NULL OR datum = '0000-00-00'
UNION ALL
SELECT id, datum, 'einnahmen' as tabelle
FROM einnahmen 
WHERE datum IS NULL OR datum = '0000-00-00';

-- Erwartetes Ergebnis: Keine Zeilen
```

#### 4.2.2 Zeitvalidierung
```sql
-- Prüfen auf ungültige Uhrzeitwerte in medikamente
SELECT id, uhrzeit, name
FROM medikamente
WHERE uhrzeit IS NULL OR uhrzeit < '00:00:00' OR uhrzeit > '23:59:59';

-- Erwartetes Ergebnis: Keine Zeilen
```

### 4.3 Geschäftsregeln-Tests

#### 4.3.1 Eindeutigkeit von Benutzernamen
```sql
-- Prüfen auf doppelte Benutzernamen
SELECT vorname, nachname, COUNT(*) as anzahl
FROM benutzer
GROUP BY vorname, nachname
HAVING COUNT(*) > 1;

-- Erwartetes Ergebnis: Keine Zeilen
```

#### 4.3.2 Medikamentennamen-Validierung
```sql
-- Prüfen auf leere Medikamentennamen
SELECT id, name, benutzer_id
FROM medikamente
WHERE name IS NULL OR TRIM(name) = '';

-- Erwartetes Ergebnis: Keine Zeilen
```

## 5. Datenkonsistenz-Tests

### 5.1 Konsistenz zwischen Tabellen
```sql
-- Prüfen ob Medikamente in einnahmen auch in medikamente existieren
SELECT DISTINCT e.medikament_name
FROM einnahmen e
LEFT JOIN medikamente m ON e.medikament_name = m.name
WHERE m.name IS NULL;

-- Erwartetes Ergebnis: Keine Zeilen
```

### 5.2 Datumsbereich-Validierung
```sql
-- Prüfen auf zukünftige Einnahmen (sollten nicht existieren)
SELECT COUNT(*) as future_einnahmen
FROM einnahmen
WHERE datum > CURDATE();

-- Prüfen auf sehr alte Einnahmen (älter als 2 Jahre)
SELECT COUNT(*) as old_einnahmen
FROM einnahmen
WHERE datum < DATE_SUB(CURDATE(), INTERVAL 2 YEAR);
```

## 6. Performance-Tests

### 6.1 Index-Prüfung
```sql
-- Prüfen ob wichtige Indizes existieren
SHOW INDEX FROM benutzer;
SHOW INDEX FROM medikamente;
SHOW INDEX FROM einnahmen;
SHOW INDEX FROM tagebuch;
```

### 6.2 Query-Performance
```sql
-- Performance-Test für häufige Abfragen
EXPLAIN SELECT * FROM medikamente WHERE benutzer_id = 1;

EXPLAIN SELECT * FROM einnahmen WHERE patient_id = 1 AND datum >= CURDATE() - INTERVAL 7 DAY;

EXPLAIN SELECT * FROM tagebuch WHERE benutzer_id = 1 AND datum = CURDATE();
```

## 7. Sicherheits-Tests

### 7.1 Passwort-Sicherheit
```sql
-- Prüfen auf leere Passwörter
SELECT id, vorname, nachname
FROM benutzer
WHERE passwort IS NULL OR TRIM(passwort) = '';

-- Erwartetes Ergebnis: Keine Zeilen
```

### 7.2 Rollen-Validierung
```sql
-- Prüfen auf gültige Rollen
SELECT DISTINCT rolle
FROM benutzer;

-- Erwartete Rollen: 'Arzt', 'Patient'
```

## 8. Vollständigkeit-Tests

### 8.1 Datenvollständigkeit
```sql
-- Prüfen auf NULL-Werte in wichtigen Feldern
SELECT 'benutzer' as tabelle, COUNT(*) as null_vorname
FROM benutzer WHERE vorname IS NULL
UNION ALL
SELECT 'benutzer' as tabelle, COUNT(*) as null_nachname
FROM benutzer WHERE nachname IS NULL
UNION ALL
SELECT 'medikamente' as tabelle, COUNT(*) as null_name
FROM medikamente WHERE name IS NULL;
```

### 8.2 Datensatz-Anzahl prüfen
```sql
-- Anzahl Datensätze pro Tabelle
SELECT 'benutzer' as tabelle, COUNT(*) as anzahl FROM benutzer
UNION ALL
SELECT 'medikamente' as tabelle, COUNT(*) as anzahl FROM medikamente
UNION ALL
SELECT 'einnahmen' as tabelle, COUNT(*) as anzahl FROM einnahmen
UNION ALL
SELECT 'tagebuch' as tabelle, COUNT(*) as anzahl FROM tagebuch
UNION ALL
SELECT 'patienten_medikamente' as tabelle, COUNT(*) as anzahl FROM patienten_medikamente;
```

## 9. Automatisierte Test-Skripte

### 9.1 Vollständiger Integritätstest
```sql
-- Vollständiger Test aller Integritätsregeln
SELECT 
    'Referentielle Integrität' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    'Fremdschlüssel-Beziehungen verletzt' as beschreibung
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL

UNION ALL

SELECT 
    'Datenvalidierung' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    'Leere Medikamentennamen' as beschreibung
FROM medikamente
WHERE name IS NULL OR TRIM(name) = ''

UNION ALL

SELECT 
    'Datumvalidierung' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    'Ungültige Datumswerte' as beschreibung
FROM tagebuch
WHERE datum IS NULL OR datum = '0000-00-00'

UNION ALL

SELECT 
    'Sicherheit' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    'Leere Passwörter' as beschreibung
FROM benutzer
WHERE passwort IS NULL OR TRIM(passwort) = '';
```

### 9.2 Test-Report generieren
```sql
-- Erstelle einen Test-Report
SELECT 
    'DATENBANK-TEST-REPORT' as report_header,
    NOW() as test_datum;

-- Zusammenfassung der Tests
SELECT 
    'Zusammenfassung' as section,
    COUNT(*) as total_benutzer,
    (SELECT COUNT(*) FROM medikamente) as total_medikamente,
    (SELECT COUNT(*) FROM einnahmen) as total_einnahmen,
    (SELECT COUNT(*) FROM tagebuch) as total_tagebuch_eintraege
FROM benutzer;
```

## 10. Empfohlene Testreihenfolge

1. **Verbindungstest** (Abschnitt 2)
2. **Tabellenstruktur-Tests** (Abschnitt 3)
3. **Datenintegrität-Tests** (Abschnitt 4)
4. **Datenkonsistenz-Tests** (Abschnitt 5)
5. **Performance-Tests** (Abschnitt 6)
6. **Sicherheits-Tests** (Abschnitt 7)
7. **Vollständigkeit-Tests** (Abschnitt 8)
8. **Automatisierte Tests** (Abschnitt 9)

## 11. Erwartete Ergebnisse

Alle Tests sollten folgende Ergebnisse liefern:
- **Verbindungstest:** Erfolgreich
- **Tabellenexistenz:** Alle 5 Tabellen vorhanden
- **Referentielle Integrität:** Keine Waisendatensätze
- **Datenvalidierung:** Keine NULL-Werte in Pflichtfeldern
- **Sicherheit:** Keine leeren Passwörter
- **Performance:** Alle Queries mit Indizes optimiert

## 12. Fehlerbehandlung

Bei Fehlern in den Tests:
1. Fehler dokumentieren
2. Ursache analysieren
3. Korrekturmaßnahmen implementieren
4. Tests wiederholen
5. Dokumentation aktualisieren 