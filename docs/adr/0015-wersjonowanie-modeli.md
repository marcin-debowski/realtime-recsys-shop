# ADR-0015: Wersjonowanie i hot-reload modeli

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0008, ADR-0011

## Kontekst
Model jest trenowany codziennie. Trzeba wiedzieć, która wersja wygenerowała daną rekomendację,
i podmieniać model bez przerywania konsumpcji z Kafki.

## Decyzja
- Wersja modelu: `{typ}_{RRRR-MM-DD}[_{n}]`, np. `sknn_2026-10-21`.
- Artefakty w `ml/models/{wersja}/` (`model.pkl` lub `model.pt` + `meta.json` z parametrami i metrykami).
  W Dockerze katalog jako wolumen współdzielony przez `ml-trainer` i `ml-service`.
- Po treningu: wpis do `model_runs` (`version`, `model_type`, `trained_at`, `train_range`, `params jsonb`,
  `metrics jsonb`, `is_active`) i ustawienie `model:current` w Redisie — **tylko** gdy metryki nie są gorsze
  od aktywnej wersji o więcej niż 5% (Recall@20), inaczej model zostaje zapisany, ale nieaktywny.
- ML Service co 60 s sprawdza `model:current` (oraz `POST /admin/reload`); nowy model ładuje w tle
  i podmienia referencję atomowo. Stary obsługuje ruch do czasu podmiany.
- Każda rekomendacja niesie `modelVersion` (ADR-0005) → trafia do `rec_impression`.

## Rozważane alternatywy
- **MLflow** — pełny rejestr modeli, ale kolejny serwis; możliwy, jeśli zostanie czas.

## Konsekwencje
- Pozytywne: wyniki online można przypisać do wersji modelu; brak przestojów.
- Negatywne: przez chwilę w pamięci są dwa modele (przy tej skali bez znaczenia).
