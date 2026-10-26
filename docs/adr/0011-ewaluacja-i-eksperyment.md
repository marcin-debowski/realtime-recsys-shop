# ADR-0011: Ewaluacja offline i grupa kontrolna

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0006, ADR-0008, ADR-0012

## Kontekst
Trzeba pokazać, że rekomendacje działają: offline (jakość algorytmu) i online (czy ludzie klikają).
Heatmapa „skuteczności rekomendacji” na dashboardzie wymaga danych o wyświetleniach i kliknięciach.

## Decyzja

### Ewaluacja offline
- Podział czasowy: ostatni dzień (RetailRocket: ostatnie 7 dni) jako test, reszta jako trening.
- Protokół: dla każdej sesji testowej przewidujemy następny produkt po każdym prefiksie sesji.
- Metryki: **HitRate@K / Recall@K, MRR@K, NDCG@K** dla K ∈ {10, 20}, dodatkowo pokrycie katalogu.
- Wyniki zapisywane w `model_runs` (ADR-0015) i eksportowane do tabeli do pracy.

### Eksperyment online
- Wariant przydzielany przez API przy pierwszej paczce sesji: `variant = hash(sessionId) % 100 < 10 ? "control" : "model"`.
- `control` dostaje `popular:global` (bez kontekstu i bez historii sesji), `model` — pełną ścieżkę.
- Wariant zapisywany w `sessions.variant` i doklejany do każdego eventu w Kafce.

### Atrybucja
- Każde wyświetlenie sekcji rekomendacji → `rec_impression` (ADR-0006).
- Kliknięcie → `rec_click` + `product_view` z `fromSource: "recs"`.
- Metryki online: **CTR** = rec_click / rec_impression, **konwersja do koszyka** produktów z rekomendacji
  w ciągu sesji, z podziałem na wariant, `strategy`, `modelVersion`, kategorię i godzinę.

## Konsekwencje
- Pozytywne: dashboard pokazuje porównanie model vs kontrola, a nie tylko „ładne wykresy”.
- Negatywne: 10% ruchu dostaje gorsze rekomendacje (w projekcie bez znaczenia).
