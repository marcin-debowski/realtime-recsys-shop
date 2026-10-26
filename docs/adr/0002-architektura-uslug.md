# ADR-0002: Podział na usługi i granice odpowiedzialności

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0003, ADR-0005

## Kontekst
System ma część transakcyjną (sklep), strumieniową (clickstream) i obliczeniową (ML).
Te części mają różne wymagania: sklep musi odpowiadać szybko, ML może być wolniejszy,
zapis logów musi być niezawodny. ML najwygodniej pisać w Pythonie, resztę backendu w C#.

## Decyzja
Pięć procesów z jasno rozdzielonymi rolami:

1. **Shop.Api (ASP.NET Core)** — jedyny punkt wejścia dla przeglądarki. Katalog, koszyk, ingest
   eventów, odczyt rekomendacji, statystyki, auth admina.
2. **Shop.EventWriter (.NET Worker)** — konsument Kafki zapisujący eventy do PostgreSQL.
3. **ML Service (Python, FastAPI)** — konsument Kafki, stan sesji, inferencja, zapis do Redisa.
   HTTP tylko do `GET /health`, `GET /model` i `POST /admin/reload` (sieć wewnętrzna).
4. **Trening (Python, skrypt)** — uruchamiany wsadowo, poza ścieżką zapytania.
5. **Frontend (Angular)** — sklep, tracker, dashboard.

Komunikacja: przeglądarka → API przez REST; API → ML wyłącznie asynchronicznie przez Kafkę;
ML → API wyłącznie przez Redis. API nie wywołuje ML synchronicznie.

## Rozważane alternatywy
- **Monolit w C# z ML.NET** — prostszy, ale traci się ekosystem Pythona i sens części „inżynieria danych”.
- **Synchroniczne wywołanie FastAPI z C#** przy każdym żądaniu — prostsze, ale awaria lub wolny ML blokuje sklep.

## Konsekwencje
- Pozytywne: awaria ML nie psuje sklepu (fallback z ADR-0009); każdy serwis można skalować i testować osobno.
- Negatywne: spójność ostateczna rekomendacji; więcej kontenerów do uruchomienia.
- W kodzie: przeglądarka nigdy nie rozmawia bezpośrednio z ML Service ani z Redisem.
