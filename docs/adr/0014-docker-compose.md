# ADR-0014: Docker Compose jako środowisko uruchomieniowe

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0003

## Kontekst
Siedem procesów i trzy języki. Środowisko musi dać się postawić jedną komendą na laptopie
autora, promotora i komisji.

## Decyzja
`infra/docker-compose.yml` z profilami:

| Serwis | Obraz / build | Profil |
|---|---|---|
| `kafka` | `apache/kafka` (KRaft, jeden węzeł) | domyślny |
| `kafka-init` | jednorazowe utworzenie topiców z ADR-0003 | domyślny |
| `postgres` | `postgres` + skrypty z `infra/postgres/` | domyślny |
| `redis` | `redis` | domyślny |
| `kafka-ui` | `provectuslabs/kafka-ui` | `tools` |
| `api` | build `backend/` (Shop.Api) | `app` |
| `event-writer` | build `backend/` (Shop.EventWriter) | `app` |
| `ml-service` | build `ml/` | `app` |
| `frontend` | build `frontend/` (nginx ze statycznym buildem) | `app` |

- Podczas developmentu: `docker compose up -d` (tylko infrastruktura), serwisy uruchamiane lokalnie z IDE.
- Demo / obrona: `docker compose --profile app --profile tools up -d`.
- Wersje obrazów przypięte w `.env` (bez `latest`).
- Healthchecki dla Kafki, Postgresa i Redisa; serwisy aplikacji startują z `depends_on: condition: service_healthy`.
- Trening nocny: kontener `ml-trainer` uruchamiany ręcznie (`docker compose run`) lub przez cron hosta;
  w demo wystarczy ręczne wywołanie.

## Konsekwencje
- Pozytywne: odtwarzalne środowisko, łatwe demo.
- Negatywne: Kafka + Postgres + Redis + 4 serwisy potrzebują ok. 4–6 GB RAM.
