# ADR-0018: Ikony i przejścia między widokami — @lucide/angular, View Transitions API

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0016

## Kontekst
Sklep potrzebuje spójnego zestawu ikon (koszyk, wyszukiwarka, filtry, nawigacja) pasującego
do komponentów spartan/ui (ADR-0016). Chcemy też płynnych przejść między widokami,
np. z listy produktów do karty produktu, bez ręcznie pisanych animacji i bez dodatkowej biblioteki.

## Decyzja
**Ikony:** `@lucide/angular`. Importujemy tylko ikony faktycznie używane, żeby tree-shaking
usunął resztę z bundla. Lucide to ten sam zestaw ikon, którego domyślnie używa spartan/ui.

**Przejścia:** View Transitions API przez wbudowaną obsługę w routerze Angulara
(`provideRouter(routes, withViewTransitions())`). Elementy wspólne dla dwóch widoków
(np. zdjęcie produktu na liście i na karcie) dostają ten sam `view-transition-name` w CSS.
Przy `prefers-reduced-motion: reduce` animacje są wyłączone.

## Rozważane alternatywy
- **Inne zestawy ikon (Heroicons, Font Awesome, Material Icons)** — Material Icons odpada stylistycznie
  (ADR-0016), Font Awesome jest cięższy, a Heroicons nie ma oficjalnego pakietu dla Angulara.
- **Angular Animations (`@angular/animations`)** — więcej kodu dla przejść między trasami, a pakiet
  jest stopniowo wypierany przez natywne animacje CSS.
- **Biblioteki animacji (GSAP, Motion)** — dodatkowa zależność do efektu, który przeglądarka daje natywnie.

## Konsekwencje
- Pozytywne: spójne ikony z komponentami spartan/ui; płynne przejścia praktycznie bez kodu.
- Negatywne / koszty: przeglądarki bez obsługi View Transitions API przełączają widok bez animacji
  (graceful degradation, funkcjonalność się nie zmienia).
- W kodzie: przejścia definiujemy w CSS (`::view-transition-*`), nie w TypeScripcie. Nie dodajemy
  `@angular/animations` ani innej biblioteki animacji bez nowego ADR-a.
