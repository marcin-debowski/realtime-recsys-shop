# ADR-0007: Dane kontekstowe zamiast fingerprintingu, zgoda użytkownika

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0006, ADR-0009

## Kontekst
Pierwotna koncepcja nazywała zbieranie danych o urządzeniu „browser fingerprintingiem”.
Fingerprinting to technika identyfikacji i śledzenia konkretnego urządzenia. Wg wytycznych EDPB 2/2023
dotyczących art. 5(3) dyrektywy ePrivacy odczyt informacji z urządzenia (także zapis `sessionId`
w `sessionStorage`) co do zasady wymaga zgody, jeśli nie jest niezbędny do świadczenia usługi.
Projekt deklaruje First-Party Data i brak wymuszania logowania, więc musi to robić zgodnie z RODO.

## Decyzja
1. Nazwa w kodzie i dokumentacji: **dane kontekstowe sesji** (`context`), nie fingerprinting.
2. Zbieramy wyłącznie gruboziarniste cechy, które nie identyfikują urządzenia:

| Pole | Wartości | Źródło |
|---|---|---|
| `deviceType` | `mobile | tablet | desktop` | szerokość viewportu + `matchMedia('(pointer:coarse)')` |
| `os` | `android | ios | windows | macos | linux | other` | User-Agent Client Hints / UA, tylko rodzina |
| `browser` | `chrome | safari | firefox | edge | other` | j.w. |
| `viewportClass` | `xs | sm | md | lg | xl` | progi 576/768/992/1200 px |
| `timezone` | np. `Europe/Warsaw` | `Intl.DateTimeFormat().resolvedOptions().timeZone` |
| `localHour` | 0–23 | czas lokalny przy starcie sesji |
| `dayOfWeek` | 1–7 | j.w. |
| `daypart` | `morning | afternoon | evening | night` | wyliczane z `localHour` |
| `referrerType` | `direct | search | social | email | other` | domena `document.referrer` zmapowana na typ |
| `utmSource` | tekst ≤ 50 znaków lub null | parametr URL |

3. **Nie zbieramy:** pełnego User-Agenta, dokładnej rozdzielczości, listy czcionek, canvas/WebGL/audio
   fingerprintu, adresu IP w bazie, pełnego URL referrera.
4. **Zgoda:** banner przy pierwszym wejściu z wyborem „Personalizacja i analityka: tak / nie”.
   Decyzja zapisywana w `localStorage` (niezbędne do zapamiętania wyboru). Bez zgody: brak trackera,
   rekomendacje tylko z popularności ogólnej (bez kontekstu sesji).
5. Brak trwałego identyfikatora użytkownika między sesjami. Retencja surowych eventów: 90 dni
   (zadanie czyszczące partycje).

## Rozważane alternatywy
- **Pełny fingerprinting** — lepsze dane, ale sprzeczny z deklaracją projektu i ryzykowny prawnie.
- **Brak zgody, „bo bez cookies”** — błędne założenie; ePrivacy dotyczy każdego zapisu/odczytu z urządzenia.

## Konsekwencje
- Pozytywne: argument na obronie („privacy by design”), prostszy model danych.
- Negatywne: część użytkowników odmówi zgody; symulator (ADR-0010) musi to uwzględniać (np. 20% bez zgody).
- W kodzie: nowe pole kontekstowe wymaga nowego ADR-a.
