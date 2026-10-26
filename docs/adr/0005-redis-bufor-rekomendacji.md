# ADR-0005: Redis jako bufor rekomendacji i stanu sesji

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0002, ADR-0009

## Kontekst
ML liczy rekomendacje asynchronicznie, a API ma je oddać w kilka milisekund. Potrzebny jest
szybki magazyn wymiany, a ML musi trzymać krótką historię każdej sesji.

## Decyzja
Redis jest jedynym kanałem ML → API. ML pisze, API tylko czyta. Osobne API FastAPI do pobierania
rekomendacji nie powstaje.

**Kontrakt kluczy (wersja 1):**

| Klucz | Typ | Pisze | Czyta | TTL | Zawartość |
|---|---|---|---|---|---|
| `recs:{sessionId}` | string (JSON) | ML | API | 30 min | patrz niżej |
| `sess:{sessionId}:items` | list | ML | ML | 30 min | ostatnie 50 `productId` oglądanych w sesji |
| `sess:{sessionId}:ctx` | hash | ML | ML | 30 min | kontekst sesji (ADR-0007) |
| `popular:{contextKey}` | string (JSON) | trening | API, ML | 25 h | top-50 produktów dla kontekstu (ADR-0009) |
| `model:current` | string | trening | ML | brak | wersja aktualnego modelu |

**Wartość `recs:{sessionId}`:**
```json
{
  "schemaVersion": 1,
  "items": [{ "productId": 123, "score": 0.82 }],
  "modelVersion": "sknn_2026-10-21",
  "strategy": "model",
  "generatedAt": "2026-10-21T13:05:12Z",
  "basedOnEventId": "9b1c..."
}
```
`strategy` ∈ `model | popular | random`.

**Zasady:** każdy zapis odnawia TTL. API przy braku klucza zwraca fallback (ADR-0009) i nie czeka na ML.

## Rozważane alternatywy
- **Synchroniczny endpoint FastAPI** — dublowałby rolę Redisa i wiązał dostępność sklepu z ML.
- **Rekomendacje w PostgreSQL** — za wolne przy zapisie po każdym kliknięciu.

## Konsekwencje
- Pozytywne: odczyt O(1), pełne odcięcie API od wydajności ML.
- Negatywne: spójność ostateczna, dane w Redisie są ulotne (to akceptowalne, źródłem prawdy jest PostgreSQL).
- W kodzie: zmiana formatu wartości = podbicie `schemaVersion` po obu stronach w jednym zadaniu.
