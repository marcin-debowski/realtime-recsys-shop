# ADR-0004: PostgreSQL jako jedyna baza danych

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0003, ADR-0012

## Kontekst
Potrzebujemy danych relacyjnych (produkty, zamówienia) i półstrukturalnych logów clickstreamu.
Pierwotnie rozważane były PostgreSQL i MongoDB.

## Decyzja
Tylko PostgreSQL. Eventy w tabeli z kolumną `JSONB` na payload.

**Główne tabele:**
- `categories`, `products` (z atrybutami: kategoria, marka, kolor, cena, tagi),
- `orders`, `order_items` (zamówienie bez konta, powiązane z `session_id`),
- `sessions` (`session_id`, `started_at`, kontekst z ADR-0007, `variant`),
- `events` — partycjonowana po dniu (`ts`):
  `event_id uuid PK`, `session_id`, `type`, `ts timestamptz`, `server_ts`, `product_id null`,
  `payload jsonb`, `schema_version`,
- `model_runs` — wyniki treningów i metryki (ADR-0015).

**Indeksy:** `events(session_id, ts)`, `events(type, ts)`, `events(product_id)`.

**Idempotencja:** EventWriter robi `INSERT ... ON CONFLICT (event_id) DO NOTHING`.

**Dostęp:** EF Core dla tabel domenowych; tabelę `events` zapisujemy przez Dapper/`COPY` w paczkach.
Widoki analityczne w czystym SQL w `infra/postgres/` (ADR-0012).

## Rozważane alternatywy
- **MongoDB na logi** — naturalny dla JSON, ale drugi silnik, druga technologia, brak joinów z produktami.
- **TimescaleDB** — rozszerzenie Postgresa; możliwe później bez zmiany kontraktów.

## Konsekwencje
- Pozytywne: jedna baza, joiny eventów z produktami w SQL, mniej kontenerów.
- Negatywne: przy dużym ruchu tabela `events` rośnie szybko (partycjonowanie to łagodzi).
