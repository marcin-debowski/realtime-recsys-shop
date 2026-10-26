# ADR-0017: Wykresy i tabele w panelu admina — ngx-echarts, spartan/ui, TanStack Table

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0012, ADR-0016

## Kontekst
Panel admina (ADR-0012) pokazuje KPI, graf powiązań produktów, heatmapy, lejek i tabelę modeli
(`model_runs`). Potrzebuje więc wykresów i tabel. ADR-0012 wybrał już ECharts jako bibliotekę wykresów,
ale nie określił, czym budujemy tabele. Po ADR-0016 nie ma w projekcie gotowej biblioteki
komponentów z rozbudowaną tabelą (odrzucone PrimeNG i Angular Material).

## Decyzja
**Wykresy:** Apache ECharts przez `ngx-echarts`, zgodnie z ADR-0012. Ten ADR niczego tu nie zmienia,
tylko potwierdza wybór w kontekście nowego stosu UI. Kolory i typografię wykresów bierzemy z tokenów
`@theme` (ADR-0016), żeby wykresy wyglądały spójnie z resztą panelu.

**Tabele:**
- Domyślnie: własny komponent tabeli spartan/ui (ADR-0016), stylowany Tailwindem. Wystarcza dla
  prostych list (np. `model_runs`, top produkty).
- Przy bardziej złożonych potrzebach (sortowanie po wielu kolumnach, filtrowanie, paginacja,
  grupowanie, zmiana widoczności kolumn) używamy TanStack Table (`@tanstack/angular-table`).
  To biblioteka headless: dostarcza logikę, a renderowanie zostaje w komponentach spartan/ui.
- Długie listy wierszy: virtual scroll z Angular CDK (ADR-0016).

## Rozważane alternatywy
- **PrimeNG Table / Angular Material Table** — odrzucone razem z całymi bibliotekami (ADR-0016).
- **AG Grid** — bardzo rozbudowany, ale ciężki, z własnym systemem stylów i częścią funkcji płatną.
- **Wyłącznie własna logika tabel** — dla prostych tabel tak, ale sortowanie, filtrowanie i paginacja
  pisane od zera to niepotrzebna praca, skoro TanStack Table daje je bez narzucania wyglądu.

## Konsekwencje
- Pozytywne: jeden styl wizualny w tabelach i wykresach; logika tabel z dojrzałej biblioteki bez utraty
  kontroli nad wyglądem; spójność z ADR-0012.
- Negatywne / koszty: TanStack Table wymaga napisania warstwy prezentacji (nagłówki, sortowanie w UI,
  paginator) i nauczenia się jego modelu; adapter Angular jest mniej popularny niż wersja dla Reacta.
- W kodzie: TanStack Table dodajemy dopiero, gdy prosta tabela spartan/ui przestaje wystarczać.
  Dane dla wykresów przychodzą z `GET /api/admin/stats/{widok}` już w formacie ECharts (ADR-0012).
