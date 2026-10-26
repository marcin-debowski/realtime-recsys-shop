# Roadmapa

Każdy etap kończy się działającym systemem. Kolejny etap zaczynamy dopiero, gdy poprzedni
spełnia definicję „zrobione”.

## Etap 0 — Szkielet i infrastruktura
- Monorepo w strukturze z `CLAUDE.md`, `.env.example`, `.gitignore`.
- `infra/docker-compose.yml`: Kafka (KRaft), PostgreSQL, Redis, opcjonalnie Kafka UI (ADR-0014).
- Puste projekty: Angular workspace, solucja .NET, pakiet Pythona z `pyproject.toml`.

**Zrobione, gdy:** `docker compose up` stawia infrastrukturę, a każdy projekt się buduje i ma jeden przechodzący test.

## Etap 1 — Sklep i zbieranie danych
- Katalog produktów (seed ~200 produktów streetwear w kilku kategoriach), karta produktu, filtry, koszyk.
- Banner zgody (ADR-0007).
- Biblioteka trackera z batchingiem i `sendBeacon` (ADR-0006).
- `POST /api/events` z walidacją schematu.
- Zapis eventów do PostgreSQL (na tym etapie dopuszczalny zapis bezpośrednio z API).

**Zrobione, gdy:** kliknięcia w sklepie pojawiają się w tabeli `events` z poprawnym kontekstem.

## Etap 2 — Kafka i symulator
- API publikuje do `clickstream.events.v1`. EventWriter konsumuje i zapisuje (ADR-0003, ADR-0004).
- Bezpośredni zapis z API usunięty.
- Symulator ruchu z personami (ADR-0010).
- Test replay: przestawienie offsetu odtwarza dane do pustej tabeli.

**Zrobione, gdy:** symulator generuje 500 sesji, wszystkie lądują w bazie bez duplikatów.

## Etap 3 — Rekomendacje na żywo (MVP systemu)
- ML Service konsumuje eventy, trzyma stan sesji w Redisie (ADR-0005).
- Baseline: popularność kontekstowa (ADR-0009) + session-kNN (ADR-0008).
- `GET /api/recommendations` czyta Redis z fallbackiem.
- Sklep pokazuje sekcję „Dla Ciebie”. Tracker loguje `rec_impression` i `rec_click`.
- Grupa kontrolna (ADR-0011).

**Zrobione, gdy:** rekomendacje zmieniają się po kliknięciach, a nowa sesja dostaje popularne produkty z kontekstu.

## Etap 4 — Dashboard
- Logowanie admina (ADR-0013).
- Widoki zmaterializowane i endpointy statystyk (ADR-0012).
- Graf powiązań produktów, heatmapa CTR rekomendacji (godzina × kategoria, model vs kontrola), KPI.

**Zrobione, gdy:** dashboard pokazuje dane z symulatora i odświeża się bez ręcznej pracy.

## Etap 5 — Lepsze modele i ewaluacja
- Trening nocny, wersjonowanie, hot-reload (ADR-0015).
- Ewaluacja offline na publicznym datasecie i danych z symulatora (ADR-0011).
- Opcjonalnie GRU4Rec / SASRec i re-ranking kontekstowy (ADR-0008).

**Zrobione, gdy:** jest tabela porównawcza modeli (Recall@K, MRR@K, NDCG@K) i model można podmienić bez restartu.
