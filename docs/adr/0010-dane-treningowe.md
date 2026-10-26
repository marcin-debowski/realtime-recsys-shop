# ADR-0010: Dane treningowe — publiczny dataset i symulator ruchu

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0008, ADR-0011

## Kontekst
System uczy się na logach, ale projekt nie będzie miał prawdziwego ruchu. Bez danych nie da się
wytrenować modelu, pokazać dashboardu ani przetestować pipeline'u pod obciążeniem.

## Decyzja
Dwa niezależne źródła:

**1. Publiczny dataset sesyjny** — do treningu i ewaluacji offline algorytmów:
RetailRocket (Kaggle) jako główny; opcjonalnie YOOCHOOSE (RecSys Challenge 2015) lub Diginetica.
Loader w `ml/training/datasets/` mapuje je na wspólny format `(session_id, item_id, ts, event_type)`.
Te dane **nie** trafiają do sklepu (inne produkty); służą do rzetelnego porównania modeli.

**2. Symulator ruchu** (`ml/simulator`) — generuje sesje w naszym sklepie przez prawdziwe `POST /api/events`:
- 4–6 person, każda z preferowanymi kategoriami, budżetem, kontekstem i rozkładem pory dnia, np.
  `sneakerhead` (mobile, wieczór, social), `prezent` (desktop, popołudnie, search),
  `okazje` (filtr ceny, sortowanie), `przeglądacz` (krótkie sesje, brak koszyka);
- przejścia między produktami z prawdopodobieństwem zależnym od persony i podobieństwa produktów;
- w trybie „z rekomendacjami” persona klika rekomendację z prawdopodobieństwem zależnym od trafności
  (dzięki temu CTR modelu vs kontroli ma sens na dashboardzie);
- 20% sesji bez zgody (ADR-0007);
- parametry: liczba sesji, zakres dat (backfill historii z przeszłymi `ts`), seed losowości.

## Konsekwencje
- Pozytywne: ewaluacja na uznanym zbiorze + demonstracja całego systemu na własnych danych.
- Negatywne: wyniki na danych symulowanych odzwierciedlają założenia symulatora; trzeba to uczciwie
  opisać w pracy (dlatego ewaluację algorytmów robimy na zbiorze publicznym).
- W kodzie: symulator nie omija API, inaczej nie testuje pipeline'u. Backfill wymaga, by API
  akceptowało stare `ts` z flagą `X-Simulator: true` (tylko w środowisku dev).
