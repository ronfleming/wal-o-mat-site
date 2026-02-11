# 🐋 Wal-O-Mat

**Welcher Wal bist du? | Which whale are you?**

A personality quiz that matches you with whale species, inspired by Germany's [Wahl-O-Mat](https://www.wahl-o-mat.de).

**🚀 Local Development Quick Start:**

```powershell
# Install prerequisites (one-time)
npm install -g azurite
npm install -g azure-functions-core-tools@4

# Enable script execution (one-time, if needed)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Start all services
.\start-local.ps1
```

Open http://localhost:5042 — Press Ctrl+C to stop all services.

> **Note:** If you get "execution policy" errors, see [troubleshooting](#troubleshooting) below.

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

-   **Frontend:** Blazor WebAssembly (Standalone)
-   **Hosting:** Azure Static Web Apps (Free tier)
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

-   **Frontend:** Blazor WebAssembly (Standalone)
-   **Hosting:** Azure Static Web Apps (Free tier)
-   **CI/CD:** GitHub Actions
-   **i18n:** Deutsch/Englisch mit Sprachumschalter

---

## Development

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/) (for Azurite and Azure Functions Core Tools)
- Azurite: `npm install -g azurite`
- Azure Functions Core Tools: `npm install -g azure-functions-core-tools@4`

### Option 1: Start script (all-in-one)

```powershell
.\start-local.ps1
# Starts Azurite, Functions API, and Blazor client
# Press Ctrl+C to stop everything
```

To kill any leftover processes (orphaned Azurite, stuck ports, etc.):

```powershell
.\stop-local.ps1
```

### Option 2: Manual startup (three terminals)

This is useful when you need to restart individual services.

**Terminal 1 — Azurite (Storage Emulator):**
```powershell
azurite --silent --location .azurite --debug .azurite\debug.log
# Runs on ports 10000 (blob), 10001 (queue), 10002 (table)
```

**Terminal 2 — Azure Functions API:**
```powershell
cd Api
func start
# API available at http://localhost:7071/api
```

**Terminal 3 — Blazor Client:**
```powershell
cd Client
dotnet watch run --urls "http://localhost:5042"
# App available at http://localhost:5042
```

> **Start order matters:** Azurite must be running before the Functions API starts, since it uses Azurite for Table Storage. The client can start any time, but sharing features require the API.

### Client-only development

If you're only working on the UI and don't need sharing/API features:

```powershell
cd Client
dotnet watch run
# Open http://localhost:5042
```

## Troubleshooting

### PowerShell script execution errors

If `.\start-local.ps1` fails with "execution policy" error:

```powershell
# Option 1: Enable scripts for your user (recommended)
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser

# Option 2: Bypass for single execution
powershell -ExecutionPolicy Bypass -File .\start-local.ps1
```

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

This is a parody/homage project created for educational and entertainment purposes. It is not affiliated with or endorsed by the Bundeszentrale für politische Bildung (bpb). "Wahl-O-Mat" is a registered trademark of the bpb.
