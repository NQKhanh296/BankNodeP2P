# Changelog

Veškeré významné změny v projektu BankNodeP2P jsou zaznamenány v tomto souboru.
Formát je inspirován https://keepachangelog.com/cs/1.1.0/


## [2026-01-28] – Tobiáš Gruber
### Přidáno
- Základní WinForms UI pro monitoring běhu nodu (Start/Stop, stav, logy)
- Logovací systém (Logger, JSONL logy, UI výpis)
- Doménová logika banky (BankService)
- Perzistence stavu banky do JSON (bank-state.json)
- Bootstrap aplikace (AppComposition)
- Konfigurace aplikace pomocí appsettings.json
- Příprava UI na napojení TCP serveru (NodeController)

### Implementované bankovní operace
- AC – vytvoření účtu
- AB – zůstatek účtu
- AD – vklad
- AW – výběr
- AR – smazání účtu (jen při zůstatku 0)
- BA – celkový objem financí v bance
- BN – počet klientů

### Technické poznámky
- Thread-safe práce s účty (lock)
- Atomické ukládání JSON souborů
- Asynchronní připravenost UI pro Start/Stop serveru

---

## [2026-01-28] – Quoc Khanh Nguyen
### Přidáno
- TCP server pro obsluhu bankovního nodu (BankTcpServer)
- Paralelní obsluha více klientů pomocí TcpListener a Task
- Obsluha jednotlivých klientů (ClientHandler)
- Implementace aplikačního protokolu nad TCP
- Timeouty pro zpracování příkazů a nečinnost klienta
- Napojení síťové vrstvy na doménovou logiku banky (IBankService)
- Proxy funkcionalita pro příkazy AD, AW a AB (ESSENTIALS BANK NODE)
- Rozpoznání cizího bankovního kódu (IP) v příkazu
- Forwardování příkazu na cílový bankovní node přes TCP
- Přenos odpovědi vzdálené banky zpět původnímu klientovi
- Timeouty a ošetření chyb při komunikaci s cizím nodem

### Implementované síťové funkcionality
- Naslouchání na konfigurovatelném portu (65525–65535)
- Podpora připojení pomocí PuTTY 
- Zpracování jednořádkových příkazů 
- Odpovědi dle specifikace protokolu (BC, AC, AD, AW, AB, AR, BA, BN, ER)
- Zachování spojení po odpovědi (ukončení pouze při timeoutu nebo odpojení klienta)

### Technické poznámky
- Asynchronní accept-loop serveru běžící na pozadí
- Bezpečné ukončení serveru pomocí CancellationToken
- Ošetření timeoutů bez blokování UI
- Proxy vrstva oddělena od lokální bankovní logiky
- Zachování jednotného protokolu a formátu odpovědí