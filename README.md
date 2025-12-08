# 🐋 Wal-O-Mat

**Welcher Wal bist du? | Which whale are you?**

A personality quiz that matches you with whale species, inspired by Germany's [Wahl-O-Mat](https://www.wahl-o-mat.de).

---

## English

### What is this?

Wal-O-Mat is a personality quiz inspired by Germany's Wahl-O-Mat voting advice application. Instead of matching you with political parties, it matches you with whale species. The name is a pun: "Wahl" (German for "election/choice") vs. "Wal" (German for "whale").

### How does it work?

The quiz presents a series of statements. You can agree, stay neutral, or disagree with each one. Based on your answers, we calculate which whale species best matches your personality.

**Scoring** (identical to the real Wahl-O-Mat):

-   **2 points** for exact match
-   **1 point** for adjacent positions (one is neutral)
-   **0 points** for opposite positions

Weighted questions count double. Skipped questions are excluded.

### Technology

-   **Frontend:** Blazor WebAssembly
-   **Backend:** ASP.NET Core (Hosted model)
-   **Hosting:** Azure App Service
-   **CI/CD:** GitHub Actions
-   **i18n:** German/English with runtime toggle

---

## Deutsch

### Was ist das?

Wal-O-Mat ist ein Persönlichkeitsquiz, das als liebevolle Parodie des deutschen Wahl-O-Mat entstanden ist. Statt politischer Parteien werden hier Walarten verglichen.

### Wie funktioniert es?

Das Quiz präsentiert eine Reihe von Aussagen. Du kannst jeder Aussage zustimmen, neutral bleiben oder nicht zustimmen. Basierend auf deinen Antworten wird berechnet, welche Walart am besten zu dir passt.

**Punktevergabe** (identisch zum echten Wahl-O-Mat):

-   **2 Punkte** bei exakter Übereinstimmung
-   **1 Punkt** bei Annäherung (einer neutral)
-   **0 Punkte** bei Gegensätzen

Gewichtete Fragen zählen doppelt. Übersprungene Fragen werden nicht gewertet.

### Technologie

-   **Frontend:** Blazor WebAssembly
-   **Backend:** ASP.NET Core (Hosted-Modell)
-   **Hosting:** Azure App Service
-   **CI/CD:** GitHub Actions
-   **i18n:** Deutsch/Englisch mit Sprachumschalter

---

## Development

```bash
# Clone the repository
git clone https://github.com/ronfleming/wal-o-mat-site.git
cd wal-o-mat-site

# Run with hot reload
cd Server
dotnet watch run

# Open http://localhost:5000
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

This is a parody/homage project created for educational and entertainment purposes. It is not affiliated with or endorsed by the Bundeszentrale für politische Bildung (bpb). "Wahl-O-Mat" is a registered trademark of the bpb.
