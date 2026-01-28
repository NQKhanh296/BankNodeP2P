# Changelog

Veškeré významné změny v projektu BankNodeP2P jsou zaznamenány v tomto souboru.
Formát je inspirován https://keepachangelog.com/cs/1.1.0/

---

## [Unreleased]
- Napojení TCP serveru a protokolu (řeší Khanh Nguyen)

---

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

## [2026-01-28] – Khanh Nguyen
### Připravováno
- TCP server
- CommandParser / CommandHandler
- Napojení BankService na síťovou komunikaci
