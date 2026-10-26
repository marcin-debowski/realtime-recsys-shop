# CLAUDE.md — Kontekstowy system rekomendacji e-commerce (Clickstream)

Ten plik jest czytany przez Claude Code na starcie każdej sesji. Zawiera to, co trzeba
wiedzieć, żeby pracować w tym repozytorium. Szczegóły decyzji są w `docs/adr/`.

## O projekcie

Sklep internetowy (odzież streetwear), który personalizuje ofertę **w czasie rzeczywistym**
na podstawie bieżącej sesji użytkownika (clickstream) i danych kontekstowych (urządzenie,
pora dnia, źródło wejścia). Używamy wyłącznie First-Party Data: bez third-party cookies,
bez wymuszania logowania.

Model jest trenowany wsadowo (raz na dobę). Na produkcji jest „zamrożony”, a rekomendacje
zmieniają się, bo z każdym kliknięciem zmienia się wektor wejściowy (historia sesji).

Projekt dyplomowy. Dashboard administratora pokrywa dodatkowo zaliczenie z wizualizacji danych.

## Mapa dokumentacji

| Plik | Po co |
|---|---|
| `docs/architecture.md` | Diagram komponentów, przepływ danych, kontrakty między serwisami |
| `docs/roadmap.md` | Etapy prac i definicja „zrobione” dla każdego etapu |
| `docs/adr/README.md` | Indeks wszystkich ADR-ów i szablon |
| `docs/adr/NNNN-*.md` | Pojedyncze decyzje architektoniczne |
| `docs/examples/` | Przykładowe payloady kontraktów (używane w testach wszystkich języków) |

## Stos

- **Frontend:** Angular + TypeScript (sklep, panel admina, biblioteka trackera)
  - Tailwind CSS v4 (czysty CSS, bez SCSS), komponenty spartan/ui, Angular CDK, bez Angular Material i PrimeNG (ADR-0016)
  - Wykresy: ECharts przez `ngx-echarts`; tabele: spartan/ui + TanStack Table (ADR-0012, ADR-0017)
  - Ikony `@lucide/angular`, przejścia przez View Transitions API w routerze (ADR-0018)
- **Backend:** C# ASP.NET Core (Web API) + .NET Worker Service (zapis eventów do bazy)
- **Szyna zdarzeń:** Apache Kafka w trybie KRaft
- **ML:** Python + FastAPI (serwis inferencji, konsument Kafki), osobne skrypty treningowe
- **Cache rekomendacji:** Redis
- **Baza:** PostgreSQL (eventy w `JSONB`)
- **Uruchamianie:** Docker Compose

## Struktura repozytorium (docelowa)

```
/frontend              Angular workspace
  /projects/shop       aplikacja sklepu (z lazy-loaded /admin)
  /projects/tracker    biblioteka trackera (bez zależności od reszty aplikacji)
/backend
  /src/Shop.Api        Web API: produkty, koszyk, ingest eventów, rekomendacje, auth admina
  /src/Shop.Core       domena, DTO, kontrakty eventów
  /src/Shop.Infrastructure  EF Core, Kafka producer, Redis
  /src/Shop.EventWriter     Worker: Kafka -> PostgreSQL
  /tests
/ml
  /service             FastAPI + konsument Kafki + inferencja
  /training            pipeline treningu i ewaluacji offline
  /simulator           generator ruchu (persony)
  /models              artefakty modeli (git-ignored)
/infra
  docker-compose.yml
  /postgres            skrypty init, migracje SQL dla widoków
/docs
```

## Zasady pracy dla Claude

1. **Przed zmianą w danym obszarze przeczytaj powiązany ADR.** Indeks jest w `docs/adr/README.md`.
2. **Nie łam zaakceptowanych ADR-ów po cichu.** Jeśli zadanie wymaga innego rozwiązania,
   zatrzymaj się, opisz konflikt i zaproponuj nowy ADR, który zastępuje stary
   (status `Superseded by ADR-XXXX`). Nowe ADR-y twórz komendą `/new-adr`.
3. **Kontrakty są święte.** Schemat eventu (ADR-0006), klucze Redis (ADR-0005) i topiki Kafki
   (ADR-0003) to granice między serwisami w różnych językach. Każda zmiana wymaga podbicia
   `schemaVersion` i aktualizacji wszystkich stron kontraktu w tym samym zadaniu.
4. **Prywatność:** nie dodawaj zbierania danych spoza listy w ADR-0007. Nie twórz trwałych
   identyfikatorów urządzenia. Nie zapisuj pełnego User-Agenta ani dokładnej rozdzielczości.
5. **Pracuj etapami z `docs/roadmap.md`.** Nie implementuj funkcji z późniejszego etapu,
   dopóki bieżący nie spełnia definicji „zrobione”.
6. Po zmianie, która wpływa na architekturę, zaktualizuj `docs/architecture.md`.
7. W razie wątpliwości co do wymagań pytaj, zamiast zgadywać.

## Konwencje

- Dokumentacja i ADR-y po polsku. Kod, identyfikatory, komentarze w kodzie i commity po angielsku.
- Commity: Conventional Commits (`feat(tracker): ...`, `fix(ml): ...`).
- C#: nullable enabled, `async` wszędzie, gdzie jest I/O, konfiguracja przez `IOptions<T>`.
- TypeScript: `strict: true`, standalone components, signals.
- Python: 3.12+, type hints, `ruff` + `pytest`, zależności w `pyproject.toml`.
- Czas zawsze w UTC (ISO 8601) w eventach i bazie. Lokalna godzina użytkownika to osobne pole kontekstowe.
- Konfiguracja i sekrety przez zmienne środowiskowe (`.env`, nigdy w repo, jest `.env.example`).

## Komendy

Wszystkie komendy z katalogu głównego repozytorium. Pierwsze uruchomienie: skopiuj
`infra/.env.example` do `infra/.env`.

**Infrastruktura (Docker Compose, ADR-0014):**

```bash
docker compose -f infra/docker-compose.yml up -d                       # Kafka, PostgreSQL, Redis (development)
docker compose -f infra/docker-compose.yml --profile app up -d --build # + API, EventWriter, sklep (demo)
docker compose -f infra/docker-compose.yml --profile tools up -d       # + Kafka UI
docker compose -f infra/docker-compose.yml ps                          # stan kontenerów
docker compose -f infra/docker-compose.yml down                        # stop (dane zostają; -v usuwa dane)
```

| Serwis | Adres z hosta | Adres z kontenerów |
|---|---|---|
| Kafka | `localhost:9092` | `kafka:29092` |
| PostgreSQL | `localhost:5433` (5432 bywa zajęty przez lokalny Postgres) | `postgres:5432` |
| Redis | `localhost:6379` | `redis:6379` |
| Sklep (profil `app`, nginx, `/api` → API) | `localhost:8000` | — |
| API (profil `app`) | `localhost:5080` | `api:8080` |
| Kafka UI (profil `tools`) | `localhost:8080` | — |

**Frontend (`frontend/`):**

```bash
cd frontend && npm start              # dev server na localhost:4200
cd frontend && npm test               # testy (Vitest)
cd frontend && npm run build          # build produkcyjny do dist/shop
cd frontend && npx prettier --write projects/shop/src
```

**Backend (`backend/`):**

```bash
cd backend && dotnet build
cd backend && dotnet test
cd backend && dotnet run --project src/Shop.Api
```

**ML (`ml/`, Python w `.venv`):**

```bash
cd ml && python -m venv .venv && .venv\Scripts\activate && pip install -e ".[dev]"   # pierwsze uruchomienie
cd ml && .venv\Scripts\activate       # każda nowa sesja terminala
pytest                                # testy
ruff check . && ruff format .         # lint i formatowanie
```

> Docelowo (późniejsze etapy): `python -m simulator --sessions 500` (etap 2),
> `python -m training.train --dataset retailrocket` (etap 3–5).
