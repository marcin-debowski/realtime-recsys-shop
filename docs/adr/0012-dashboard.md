# ADR-0012: Dashboard — ECharts i agregaty w widokach zmaterializowanych

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0004, ADR-0011, ADR-0013

## Kontekst
Dashboard musi pokryć zaliczenie z wizualizacji danych: graf powiązań produktów i heatmapy
skuteczności. Rozważane były D3.js i ECharts. Liczenie agregatów na żywo z tabeli `events` będzie wolne.

## Decyzja
**Biblioteka:** Apache ECharts przez `ngx-echarts` dla wszystkich wykresów (typy `graph` z układem
`force`, `heatmap`, `line`, `bar`). D3.js tylko jeśli wymagania przedmiotu jawnie tego wymagają.

**Widoki dashboardu (`/admin`, lazy-loaded, guard JWT):**
1. **KPI** — sesje, CTR rekomendacji (model vs kontrola), konwersja, średnia długość sesji (ostatnie 24 h / 7 dni).
2. **Graf powiązań produktów** — węzły: produkty (rozmiar = liczba wyświetleń, kolor = kategoria);
   krawędzie: współwystępowanie w sesjach (grubość = liczba sesji). Top 100 par powyżej progu,
   filtr kategorii, kliknięcie węzła pokazuje sąsiadów.
3. **Heatmapa CTR** — oś X: godzina (0–23), oś Y: kategoria; przełącznik model / kontrola / różnica.
4. **Heatmapa kontekstu** — `deviceType × daypart` → CTR lub konwersja.
5. **Lejek** — product_view → add_to_cart → purchase, z podziałem na źródło wejścia.
6. **Modele** — tabela `model_runs` i wykres metryk offline w czasie.

**Agregaty:** widoki zmaterializowane w PostgreSQL (`infra/postgres/views/`), np.
`mv_coview_pairs`, `mv_rec_ctr_hour_category`, `mv_context_ctr`, `mv_funnel_daily`.
Odświeżanie `REFRESH MATERIALIZED VIEW CONCURRENTLY` co 5 min przez `BackgroundService` w API.
API wystawia `GET /api/admin/stats/{widok}` z gotowymi danymi pod format ECharts.

## Rozważane alternatywy
- **D3.js** — pełna kontrola, ale kilkukrotnie więcej kodu na każdy wykres.
- **Agregujący konsument Kafki** — ciekawszy strumieniowo, ale więcej kodu; możliwy później dla KPI na żywo.
- **Grafana / Metabase** — gotowe, ale nie byłoby to częścią aplikacji.

## Konsekwencje
- Pozytywne: jeden styl wykresów, szybkie odpowiedzi API, czysty SQL łatwo opisać w pracy.
- Negatywne: dane opóźnione do 5 min (dla dashboardu wystarczy).
