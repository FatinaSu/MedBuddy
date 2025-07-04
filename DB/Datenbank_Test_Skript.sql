-- =====================================================
-- MedBuddy Datenbank - Vollständiges Test-Skript
-- =====================================================
-- Ausführung: mysql -u root -p medbuddy_db < Datenbank_Test_Skript.sql
-- =====================================================

-- Test-Header
SELECT '=====================================================' as separator;
SELECT 'MEDBUDDY DATENBANK-TEST STARTET' as test_header;
SELECT NOW() as test_datum;
SELECT '=====================================================' as separator;

-- =====================================================
-- 1. VERBINDUNGSTEST
-- =====================================================
SELECT '1. VERBINDUNGSTEST' as test_section;
SELECT '=====================================================' as separator;

SELECT 1 as connection_test, 'Verbindung erfolgreich' as status;

-- Datenbankexistenz prüfen
SELECT 'Datenbankexistenz:' as test, 
       CASE 
           WHEN DATABASE() = 'medbuddy_db' THEN 'OK'
           ELSE 'FEHLER: Falsche Datenbank'
       END as status;

-- =====================================================
-- 2. TABELLENSTRUKTUR-TESTS
-- =====================================================
SELECT '2. TABELLENSTRUKTUR-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Alle Tabellen auflisten
SELECT 'Verfügbare Tabellen:' as info;
SHOW TABLES;

-- Tabellenschemas prüfen
SELECT 'Schema benutzer:' as info;
DESCRIBE benutzer;

SELECT 'Schema medikamente:' as info;
DESCRIBE medikamente;

SELECT 'Schema einnahmen:' as info;
DESCRIBE einnahmen;

SELECT 'Schema tagebuch:' as info;
DESCRIBE tagebuch;

SELECT 'Schema patienten_medikamente:' as info;
DESCRIBE patienten_medikamente;

-- =====================================================
-- 3. DATENINTEGRITÄT-TESTS
-- =====================================================
SELECT '3. DATENINTEGRITÄT-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Referentielle Integrität prüfen
SELECT 'Referentielle Integrität - Waisendatensätze:' as test;

-- Waisendatensätze in medikamente
SELECT 
    'medikamente' as tabelle,
    COUNT(*) as waisendatensaetze,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Waisendatensätze gefunden'
    END as status
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL;

-- Waisendatensätze in einnahmen
SELECT 
    'einnahmen' as tabelle,
    COUNT(*) as waisendatensaetze,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Waisendatensätze gefunden'
    END as status
FROM einnahmen e
LEFT JOIN benutzer b ON e.patient_id = b.id
WHERE b.id IS NULL;

-- Waisendatensätze in tagebuch
SELECT 
    'tagebuch' as tabelle,
    COUNT(*) as waisendatensaetze,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Waisendatensätze gefunden'
    END as status
FROM tagebuch t
LEFT JOIN benutzer b ON t.benutzer_id = b.id
WHERE b.id IS NULL;

-- =====================================================
-- 4. DATENVALIDIERUNG
-- =====================================================
SELECT '4. DATENVALIDIERUNG' as test_section;
SELECT '=====================================================' as separator;

-- NULL-Werte in wichtigen Feldern prüfen
SELECT 'NULL-Werte in Pflichtfeldern:' as test;

SELECT 
    'benutzer.vorname' as feld,
    COUNT(*) as null_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: NULL-Werte gefunden'
    END as status
FROM benutzer WHERE vorname IS NULL

UNION ALL

SELECT 
    'benutzer.nachname' as feld,
    COUNT(*) as null_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: NULL-Werte gefunden'
    END as status
FROM benutzer WHERE nachname IS NULL

UNION ALL

SELECT 
    'medikamente.name' as feld,
    COUNT(*) as null_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: NULL-Werte gefunden'
    END as status
FROM medikamente WHERE name IS NULL OR TRIM(name) = '';

-- Datumsvalidierung
SELECT 'Datumsvalidierung:' as test;

SELECT 
    'tagebuch.datum' as feld,
    COUNT(*) as ungültige_daten,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Ungültige Datumswerte'
    END as status
FROM tagebuch 
WHERE datum IS NULL OR datum = '0000-00-00'

UNION ALL

SELECT 
    'einnahmen.datum' as feld,
    COUNT(*) as ungültige_daten,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Ungültige Datumswerte'
    END as status
FROM einnahmen 
WHERE datum IS NULL OR datum = '0000-00-00';

-- Zeitvalidierung
SELECT 'Zeitvalidierung:' as test;

SELECT 
    'medikamente.uhrzeit' as feld,
    COUNT(*) as ungültige_zeiten,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Ungültige Uhrzeitwerte'
    END as status
FROM medikamente
WHERE uhrzeit IS NULL OR uhrzeit < '00:00:00' OR uhrzeit > '23:59:59';

-- =====================================================
-- 5. GESCHÄFTSREGELN-TESTS
-- =====================================================
SELECT '5. GESCHÄFTSREGELN-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Eindeutigkeit von Benutzernamen
SELECT 'Eindeutigkeit Benutzernamen:' as test;

SELECT 
    COUNT(*) as doppelte_namen,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Doppelte Benutzernamen gefunden'
    END as status
FROM (
    SELECT vorname, nachname, COUNT(*) as anzahl
    FROM benutzer
    GROUP BY vorname, nachname
    HAVING COUNT(*) > 1
) as duplicates;

-- Rollen-Validierung
SELECT 'Rollen-Validierung:' as test;

SELECT 
    rolle,
    COUNT(*) as anzahl,
    CASE 
        WHEN rolle IN ('Arzt', 'Patient') THEN 'OK'
        ELSE 'FEHLER: Ungültige Rolle'
    END as status
FROM benutzer
GROUP BY rolle;

-- =====================================================
-- 6. SICHERHEITS-TESTS
-- =====================================================
SELECT '6. SICHERHEITS-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Passwort-Sicherheit
SELECT 'Passwort-Sicherheit:' as test;

SELECT 
    COUNT(*) as leere_passwoerter,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Leere Passwörter gefunden'
    END as status
FROM benutzer
WHERE passwort IS NULL OR TRIM(passwort) = '';

-- =====================================================
-- 7. DATENKONSISTENZ-TESTS
-- =====================================================
SELECT '7. DATENKONSISTENZ-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Konsistenz zwischen Tabellen
SELECT 'Medikamenten-Konsistenz:' as test;

SELECT 
    COUNT(*) as inkonsistente_medikamente,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'FEHLER: Inkonsistente Medikamentendaten'
    END as status
FROM (
    SELECT DISTINCT e.medikament_name
    FROM einnahmen e
    LEFT JOIN medikamente m ON e.medikament_name = m.name
    WHERE m.name IS NULL
) as inconsistent;

-- Datumsbereich-Validierung
SELECT 'Datumsbereich-Validierung:' as test;

SELECT 
    'Zukünftige Einnahmen' as test_typ,
    COUNT(*) as anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'WARNUNG: Zukünftige Einnahmen gefunden'
    END as status
FROM einnahmen
WHERE datum > CURDATE()

UNION ALL

SELECT 
    'Sehr alte Einnahmen (>2 Jahre)' as test_typ,
    COUNT(*) as anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'OK'
        ELSE 'INFO: Alte Einnahmen gefunden'
    END as status
FROM einnahmen
WHERE datum < DATE_SUB(CURDATE(), INTERVAL 2 YEAR);

-- =====================================================
-- 8. PERFORMANCE-TESTS
-- =====================================================
SELECT '8. PERFORMANCE-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Index-Prüfung
SELECT 'Index-Prüfung:' as test;

SELECT 'Indizes in benutzer:' as info;
SHOW INDEX FROM benutzer;

SELECT 'Indizes in medikamente:' as info;
SHOW INDEX FROM medikamente;

SELECT 'Indizes in einnahmen:' as info;
SHOW INDEX FROM einnahmen;

SELECT 'Indizes in tagebuch:' as info;
SHOW INDEX FROM tagebuch;

-- =====================================================
-- 9. VOLLSTÄNDIGKEITS-TESTS
-- =====================================================
SELECT '9. VOLLSTÄNDIGKEITS-TESTS' as test_section;
SELECT '=====================================================' as separator;

-- Datensatz-Anzahl
SELECT 'Datensatz-Anzahl pro Tabelle:' as test;

SELECT 
    'benutzer' as tabelle,
    COUNT(*) as anzahl
FROM benutzer

UNION ALL

SELECT 
    'medikamente' as tabelle,
    COUNT(*) as anzahl
FROM medikamente

UNION ALL

SELECT 
    'einnahmen' as tabelle,
    COUNT(*) as anzahl
FROM einnahmen

UNION ALL

SELECT 
    'tagebuch' as tabelle,
    COUNT(*) as anzahl
FROM tagebuch

UNION ALL

SELECT 
    'patienten_medikamente' as tabelle,
    COUNT(*) as anzahl
FROM patienten_medikamente;

-- =====================================================
-- 10. ZUSAMMENFASSENDER TEST-REPORT
-- =====================================================
SELECT '10. ZUSAMMENFASSENDER TEST-REPORT' as test_section;
SELECT '=====================================================' as separator;

-- Vollständiger Integritätstest
SELECT 'Vollständiger Integritätstest:' as test;

SELECT 
    'Referentielle Integrität' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL

UNION ALL

SELECT 
    'Datenvalidierung' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis
FROM medikamente
WHERE name IS NULL OR TRIM(name) = ''

UNION ALL

SELECT 
    'Datumvalidierung' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis
FROM tagebuch
WHERE datum IS NULL OR datum = '0000-00-00'

UNION ALL

SELECT 
    'Sicherheit' as test_kategorie,
    COUNT(*) as fehler_anzahl,
    CASE 
        WHEN COUNT(*) = 0 THEN 'BESTANDEN'
        ELSE 'FEHLGESCHLAGEN'
    END as ergebnis
FROM benutzer
WHERE passwort IS NULL OR TRIM(passwort) = '';

-- Test-Report
SELECT 'DATENBANK-TEST-REPORT' as report_header;
SELECT NOW() as test_datum;

SELECT 
    'Zusammenfassung' as section,
    COUNT(*) as total_benutzer,
    (SELECT COUNT(*) FROM medikamente) as total_medikamente,
    (SELECT COUNT(*) FROM einnahmen) as total_einnahmen,
    (SELECT COUNT(*) FROM tagebuch) as total_tagebuch_eintraege
FROM benutzer;

-- =====================================================
-- TEST-ENDE
-- =====================================================
SELECT '=====================================================' as separator;
SELECT 'MEDBUDDY DATENBANK-TEST ABGESCHLOSSEN' as test_footer;
SELECT NOW() as test_ende;
SELECT '=====================================================' as separator; 