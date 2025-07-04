# Anleitung zur Ausführung der MedBuddy Datenbank-Tests

## Voraussetzungen

1. **MySQL Server** muss installiert und laufen
2. **MedBuddy-Datenbank** (`medbuddy_db`) muss existieren
3. **MySQL Client** (mysql-Befehl) muss verfügbar sein
4. **Zugangsdaten** für die Datenbank (Standard: root/Medbuddy2025!)

## Ausführungsmethoden

### Methode 1: Kommandozeile (Empfohlen)

```bash
# Im DB-Verzeichnis ausführen:
cd DB

# Vollständiges Test-Skript ausführen
mysql -u root -p medbuddy_db < Datenbank_Test_Skript.sql

# Oder mit explizitem Passwort (nicht empfohlen für Produktion)
mysql -u root -pMedbuddy2025! medbuddy_db < Datenbank_Test_Skript.sql
```

### Methode 2: MySQL Workbench

1. MySQL Workbench öffnen
2. Verbindung zur `medbuddy_db` herstellen
3. Datei `Datenbank_Test_Skript.sql` öffnen
4. Skript ausführen (Strg+Shift+Enter)

### Methode 3: phpMyAdmin

1. phpMyAdmin öffnen
2. Datenbank `medbuddy_db` auswählen
3. SQL-Tab öffnen
4. Inhalt von `Datenbank_Test_Skript.sql` einfügen
5. Ausführen klicken

## Einzeltests ausführen

### Verbindungstest
```sql
-- Einfacher Verbindungstest
SELECT 1 as connection_test;
```

### Tabellenstruktur prüfen
```sql
-- Alle Tabellen auflisten
SHOW TABLES FROM medbuddy_db;

-- Schema einer spezifischen Tabelle
DESCRIBE benutzer;
```

### Datenintegrität prüfen
```sql
-- Waisendatensätze in medikamente finden
SELECT COUNT(*) as orphaned_medikamente
FROM medikamente m
LEFT JOIN benutzer b ON m.benutzer_id = b.id
WHERE b.id IS NULL;
```

### Datenvalidierung
```sql
-- NULL-Werte in wichtigen Feldern
SELECT COUNT(*) as null_vorname
FROM benutzer WHERE vorname IS NULL;
```

## Erwartete Ergebnisse

### Erfolgreiche Tests zeigen:
- **Verbindungstest:** `1` als Ergebnis
- **Tabellenexistenz:** Alle 5 Tabellen aufgelistet
- **Referentielle Integrität:** `0` Waisendatensätze
- **Datenvalidierung:** `0` NULL-Werte in Pflichtfeldern
- **Sicherheit:** `0` leere Passwörter

### Fehlerbehandlung

#### Verbindungsfehler
```bash
# Fehler: "Access denied"
# Lösung: Passwort überprüfen
mysql -u root -p

# Fehler: "Unknown database"
# Lösung: Datenbank erstellen
mysql -u root -p
CREATE DATABASE medbuddy_db;
```

#### Tabellenfehler
```sql
-- Fehler: "Table doesn't exist"
-- Lösung: Tabellen erstellen (falls nicht vorhanden)
CREATE TABLE benutzer (
    id INT PRIMARY KEY AUTO_INCREMENT,
    vorname VARCHAR(50) NOT NULL,
    nachname VARCHAR(50) NOT NULL,
    passwort VARCHAR(255) NOT NULL,
    email VARCHAR(100),
    rolle ENUM('Arzt', 'Patient') NOT NULL
);
```

## Test-Report interpretieren

### Erfolgreicher Test:
```
+------------------+-------------+----------+
| test_kategorie   | fehler_anzahl | ergebnis |
+------------------+-------------+----------+
| Referentielle Integrität | 0 | BESTANDEN |
| Datenvalidierung | 0 | BESTANDEN |
| Datumvalidierung | 0 | BESTANDEN |
| Sicherheit       | 0 | BESTANDEN |
+------------------+-------------+----------+
```

### Fehlgeschlagener Test:
```
+------------------+-------------+----------+
| test_kategorie   | fehler_anzahl | ergebnis |
+------------------+-------------+----------+
| Referentielle Integrität | 5 | FEHLGESCHLAGEN |
```

## Automatisierung

### Batch-Skript für Windows
```batch
@echo off
echo MedBuddy Datenbank-Test startet...
mysql -u root -pMedbuddy2025! medbuddy_db < Datenbank_Test_Skript.sql > test_ergebnis.txt
echo Test abgeschlossen. Ergebnis in test_ergebnis.txt
pause
```

### Shell-Skript für Linux/Mac
```bash
#!/bin/bash
echo "MedBuddy Datenbank-Test startet..."
mysql -u root -p medbuddy_db < Datenbank_Test_Skript.sql > test_ergebnis.txt
echo "Test abgeschlossen. Ergebnis in test_ergebnis.txt"
```

## Häufige Probleme und Lösungen

### Problem: "Access denied for user"
**Lösung:** 
```sql
-- Benutzerrechte prüfen
SHOW GRANTS FOR 'root'@'localhost';

-- Falls nötig, Rechte gewähren
GRANT ALL PRIVILEGES ON medbuddy_db.* TO 'root'@'localhost';
FLUSH PRIVILEGES;
```

### Problem: "Table doesn't exist"
**Lösung:** Tabellen aus dem Code erstellen oder Backup wiederherstellen

### Problem: "Connection timeout"
**Lösung:** MySQL-Service neu starten
```bash
# Windows
net stop mysql
net start mysql

# Linux
sudo systemctl restart mysql
```

## Dokumentation der Testergebnisse

### Testprotokoll erstellen
```sql
-- Testprotokoll mit Zeitstempel
SELECT 
    'TEST-PROTOKOLL' as header,
    NOW() as test_datum,
    USER() as test_benutzer,
    DATABASE() as test_datenbank;
```

### Ergebnisse exportieren
```bash
# Ergebnisse in Datei speichern
mysql -u root -p medbuddy_db < Datenbank_Test_Skript.sql > test_ergebnis_$(date +%Y%m%d_%H%M%S).txt
```

## Empfohlene Testfrequenz

- **Entwicklung:** Vor jedem Commit
- **Testing:** Täglich
- **Produktion:** Wöchentlich
- **Nach Updates:** Sofort

## Kontakt bei Problemen

Bei Problemen mit den Tests:
1. Fehlermeldung dokumentieren
2. MySQL-Logs prüfen
3. Datenbankverbindung testen
4. Tabellenstruktur überprüfen 