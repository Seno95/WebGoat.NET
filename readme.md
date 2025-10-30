# WebGoat.NET

**Dette er en lokal kopi af det originale projekt: [tobyash86/WebGoat.NET](https://github.com/tobyash86/WebGoat.NET)**  
Kopien anvendes udelukkende til undervisnings- og uddannelsesformål.

> 📌 **Bemærk:** Læs den fulde dokumentation i det oprindelige repository for yderligere tekniske detaljer.

---

## 🚀 Udrulning og sikkerhedsanbefalinger

Da applikationen er designet til at indeholde sårbarheder, **må den aldrig køre direkte på en produktionsmaskine** eller eksponeres for internettet. Følg derfor disse anbefalinger nøje:

- ✅ Kør applikationen i en isoleret **Docker-container**.
- 🔒 Sørg for, at netværksporte **ikke er eksponeret** mod internettet – anvend fx firewall-regler eller `--network host`-isolation.

---

## 🐳 Kørsel i Docker (anbefalet)

1. Byg containeren:
   ```bash
   docker build --pull --rm -t webgoat.net .
   ```

2. Kør containeren:
   ```bash
   docker run --rm -d -p 5000:8080 --name webgoat.net webgoat.net
   ```

   Webapplikationen vil være tilgængelig på `http://localhost:5000`.

> 🛡️ Overvej at tilføje `--network bridge` og sikre, at den kun er tilgængelig lokalt.

---

## ⚙️ Lokal kørsel (ikke anbefalet)

**Krav:**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

**Sådan starter du applikationen:**
1. Naviger til projektmappen `WebGoat.NET`.
2. Kør:
   ```bash
   dotnet run
   ```

> ❗Denne metode giver **ingen isolation** og bør kun anvendes i lukkede testmiljøer.

---

## 🧩 Database: Reset og persistens

WebGoat.NET anvender en SQLite-database (`NORTHWND.sqlite`) til persistent lagring.

**Sådan nulstilles databasen:**
- Erstat den nuværende `NORTHWND.sqlite` med en frisk kopi fra repository’et.



