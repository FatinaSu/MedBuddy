# 💊 MedBuddy – Medikamentenerinnerungs-App

MedBuddy ist eine benutzerfreundliche Windows-Web-App, die ältere Menschen bei der strukturierten und regelmäßigen Einnahme von Medikamenten unterstützt. Die App bietet individuelle Erinnerungsfunktionen, eine Tagebuchansicht für persönliche Einträge und eine Kommunikationsschnittstelle für Ärzte oder Pflegepersonal. 

---

## 🚀 Projektübersicht

**Ziel:**  
Entwicklung einer barrierearmen App mit klarer Benutzeroberfläche, automatisierten Erinnerungen und sicherer Datenspeicherung.

**Zielgruppen:**  
- Ältere Menschen
- Pflegepersonal / Angehörige
- Ärzte

---

## 🔧 Technologien

| Bereich      | Technologie          |
|-------------|----------------------|
| Frontend     | XAML (.NET WPF)      |
| Backend      | C# / .NET Core Web API |
| Datenbank    | MySQL                |
| Tools        | Visual Studio, GitHub, Figma |

---

---

## 🧪 Features

- ✅ Medikamentenübersicht & -erinnerung
- ✅ „Eingenommen“-Button mit Protokollierung
- ✅ Wöchentliche Auswertung der Einnahmen
- ✅ Tagebuchfunktion für Symptome & Stimmung
- ✅ Benutzer- und Rollenverwaltung (Patient / Arzt)
- Optional: Kalenderfunktion

---

# 💊 MedBuddy

**MedBuddy** ist eine WPF-basierte Desktop-Anwendung zur Unterstützung von Patienten bei der täglichen Medikamenteneinnahme.  
Die App wurde im Rahmen eines Projekts entwickelt und richtet sich sowohl an Patienten als auch an medizinisches Fachpersonal.

---

## 📌 Funktionen

- Benutzerrollen: **Patient** und **Arzt**
- Patienten können:
  - Medikamente anzeigen
  - Einnahmeerinnerungen erhalten
  - Tagebucheinträge (Symptome/Stimmung) erstellen
- Ärzte können:
  - Medikamente für Patienten anlegen oder anpassen
  - Einnahmen kontrollieren
  - Tagebuchverläufe einsehen
- Lokale Speicherung in einer **MySQL-Datenbank**
- Benutzerbezogene Datenverarbeitung mit Login-Funktion
- Datenschutzkonformes Konzept (keine Cloud-Anbindung)

---

## ▶️ Erste Schritte

Nach dem Start der Anwendung kann direkt mit der Anmeldung begonnen werden.  
Keine neue Registrierung erforderlich: bereit Testbenutzer stehen zur Verfügung:

### 🔐 Testzugang – Anmeldung

**Als Arzt:**
- Benutzername: `Maxi Mustermann`
- Passwort: `maxi123`

**Als Patient:**
- Benutzername: `Sara Khan`
- Passwort: `sara123`

Nach dem Login wird je nach Benutzerrolle automatisch die entsprechende Oberfläche (Patienten- oder Arztansicht) geöffnet.

---

## ⚙️ Voraussetzungen

- Visual Studio 2022 oder höher
- .NET Framework
- MySQL (lokal installiert)
- Optional: MySQL Workbench zur Datenbankverwaltung

---

## 🚀 Installation & Ausführung

1. Repository klonen:
   ```bash
   git clone https://github.com/FatinaSu/MedBuddy.git


---

2. MySQL-Server starten und Datenbank einrichten (medbuddy_db)

3. Verbindungsdaten in der App-Konfiguration anpassen

4. Projekt in Visual Studio öffnen

5. App starten mit F5

--

## 👩‍💻 Team

- Fatina Sulejmani
- Sara Khan
- Außenkontakt: PKA
