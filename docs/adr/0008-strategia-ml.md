# ADR-0008: Strategia ML — trening wsadowy, inferencja na żywo, drabinka modeli

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0009, ADR-0010, ADR-0011, ADR-0015

## Kontekst
Pierwotny opis mówił o „sieciach sekwencyjnych lub zaawansowanej klasteryzacji”. To za mało
konkretne, a klasteryzacja sama w sobie nie produkuje rekomendacji. Ryzyko: utknięcie na modelu
neuronowym, zanim działa reszta systemu.

## Decyzja
**Paradygmat:** rekomendacje sesyjne (session-based recommendation). Wejście: uporządkowana lista
produktów z bieżącej sesji (+ ważone typy interakcji) i kontekst. Wyjście: top-K `productId`.

**Trening wsadowy, inferencja na żywo:** model trenowany raz na dobę, w ML Service „zamrożony”.
Po każdym evencie ML Service aktualizuje `sess:{id}:items` i przelicza rekomendacje.

**Drabinka modeli** (każdy szczebel to działający wynik, wspólny interfejs `Recommender.recommend(session, ctx, k)`):

1. **`popular`** — popularność w kontekście (ADR-0009). Baseline i fallback.
2. **`sknn`** — session-kNN: sesja porównywana z historycznymi sesjami (podobieństwo cosinusowe na zbiorach
   produktów, waga ostatnich interakcji wyższa). Główny model MVP. Wariant `item-cooc` (współwystępowanie
   par produktów) dopuszczalny jako prostszy.
3. **`gru4rec` / `sasrec`** — model sekwencyjny w PyTorch. Opcjonalny, etap 5.

**Wagi interakcji** w wektorze sesji: `product_view` 1, `product_dwell` > 15 s 2,
`image_interaction` 1.5, `add_to_cart` 4. Wagi są konfigurowalne i opisane w pracy.

**Użycie kontekstu:** re-ranking wyniku modelu: `score' = score + λ · popularność_produktu_w_kontekście`,
λ dobierane w ewaluacji offline. Filtry z sesji (rozmiar, przedział cen) działają jako twarde filtry.

**Reguły biznesowe po modelu:** usuń produkty już w koszyku i niedostępne, maks. 3 produkty z jednej kategorii w top-10.

## Rozważane alternatywy
- **Klasteryzacja sesji (k-means)** — może posłużyć do analizy person na dashboardzie, nie jako rekomender.
- **Uczenie online** (aktualizacja wag na żywo) — trudne do ewaluacji i debugowania.
- **Collaborative filtering po użytkowniku** — wymaga trwałej tożsamości, której celowo nie mamy (ADR-0007).

## Konsekwencje
- Pozytywne: działający system już po szczeblu 2; porównanie modeli to gotowy rozdział badawczy.
- Negatywne: session-kNN wymaga trzymania indeksu sesji w pamięci (przy skali projektu bez problemu).
- W kodzie: `ml/service/recommenders/` z jedną klasą na model, wybór przez konfigurację.
