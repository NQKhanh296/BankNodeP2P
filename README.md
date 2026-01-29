# BankNodeP2P

## Autoři
- **Nguyen Quoc Khanh**
- **Tobiáš Gruber**

## Popis projektu
**BankNodeP2P** je desktopová WinForms aplikace implementující bankovní uzel v P2P síti.  
Aplikace umožňuje vytváření a správu bankovních účtů, zpracování bankovních příkazů a komunikaci mezi bankovními uzly pomocí TCP protokolu.

Projekt splňuje zadání **ESSENTIALS BANK NODE** a klade důraz na:
- správnou síťovou komunikaci,
- práci s timeouty,
- thread-safe zpracování klientů,
- přehledné uživatelské rozhraní pro monitoring a ovládání serveru.

---

## Funkcionalita
Podporované příkazy bankovního protokolu:
- `BC` – kontrola dostupnosti banky
- `AC` – vytvoření účtu
- `AD` – vklad na účet
- `AW` – výběr z účtu
- `AB` – zjištění zůstatku účtu
- `AR` – odstranění účtu (pouze s nulovým zůstatkem)
- `BA` – celkový objem peněz v bance
- `BN` – počet klientů (účtů)

Aplikace automaticky:
- proxy přeposílá příkazy na jiný bankovní uzel v P2P síti,
- rozlišuje lokální a vzdálené banky podle IP adresy.

---

## Konfigurace aplikace
Konfigurace se **nenačítá ze souboru**, ale je nastavena **za běhu aplikace přes uživatelské rozhraní**:

- **IP adresa banky**  
  - je detekována automaticky z aktivního síťového rozhraní počítače
  - není editovatelná uživatelem

- **TCP port**
- **Timeout pro zpracování příkazu** (výchozí: 5000 ms)
- **Timeout nečinného klienta**

Pokud některá operace nebo komunikace přesáhne nastavený timeout, je považována za chybu a je vrácena odpověď typu `ER`.

---

## Uživatelské rozhraní (WinForms)
UI umožňuje:
- nastavit port a timeouty před spuštěním serveru,
- spustit a zastavit bankovní uzel,
- sledovat stav serveru (Running / Stopped),
- zobrazovat logy aplikace v reálném čase.

---

## Persistentní data
Aplikace ukládá data do složky `data/`:
- **bank-state.json** – stav bankovních účtů
- **logs.jsonl** – aplikační logy (formát JSON Lines)

Data jsou zachována i po restartu aplikace.

---

## Spuštění aplikace (bez Visual Studia)
Hotová verze aplikace je připravena ke spuštění **bez potřeby Visual Studia**.

### 📦 Stažení
Zabalená (release) verze aplikace je dostupná ve složce: `BankNodeP2P/Release/`.

V této složce se nachází **ZIP archiv**:
- `BankNodeP2P.zip`

### ▶️ Spuštění
1. Rozbalte ZIP archiv
2. Spusťte `BankNodeP2P.exe`
3. Nastavte port a timeouty v UI
4. Klikněte na **Start**

---

## Použité technologie
- **C# (.NET)**
- **WinForms**
- **TCP sockets**
- **JSON (System.Text.Json)**

---
