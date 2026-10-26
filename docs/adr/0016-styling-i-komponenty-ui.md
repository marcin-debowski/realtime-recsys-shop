# ADR-0016: Styling i komponenty UI — Tailwind CSS v4, spartan/ui, Angular CDK

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0006, ADR-0017, ADR-0018

## Kontekst
Sklep (odzież streetwear) potrzebuje własnego, wyrazistego wyglądu, a nie estetyki Material Design.
Panel admina potrzebuje formularzy, tabel, dialogów i menu. Nie chcemy pisać od zera zachowań
takich jak overlay, focus trap czy obsługa klawiatury, ale chcemy pełnej kontroli nad wyglądem.
Projekt dyplomowy nie może zależeć od biblioteki, której licencja lub cykl wydań mogą się nagle zmienić.

## Decyzja
**Styling:**
- Tailwind CSS v4 przez plugin PostCSS (`@tailwindcss/postcss`, plik `.postcssrc.json` w workspace).
- Style wyłącznie w czystym CSS (`styles.css`, `*.component.css`), bez SCSS. Tailwind v4 sam działa
  jako preprocesor (`@import "tailwindcss"`, `@theme`, zagnieżdżanie) i nie współpracuje z Sassem.
- Tokeny projektu (kolory, typografia, promienie, odstępy) definiujemy w `@theme` w jednym miejscu.
- Prettier z `prettier-plugin-tailwindcss` sortuje klasy.

**Komponenty UI:** spartan/ui, czyli podejście jak w shadcn. Komponenty są generowane przez CLI spartana,
kopiowane do repozytorium i stylowane Tailwindem. Kod komponentów należy do nas, więc nie zależymy
od licencji ani cyklu wydań zewnętrznego dostawcy. To **jedyna** biblioteka komponentów w projekcie.

**Zachowania i dostępność:** Angular CDK, bez Angular Material:
- `overlay` (dialogi, menu, dropdowny),
- `scrolling` (virtual scroll dla list produktów),
- `a11y` (focus trap, `LiveAnnouncer`, nawigacja klawiaturą),
- `drag-drop`.

## Rozważane alternatywy
- **PrimeNG** — od wersji 22 (zgodnej z Angularem 22) przeszedł na zamkniętą, komercyjną licencję
  PrimeUI z kluczem licencyjnym. Repozytorium na GitHubie zostało zarchiwizowane w czerwcu 2026.
  Darmowa licencja Community wymaga weryfikacji uprawnień. Pozostanie na ostatniej wersji MIT (v21)
  oznaczałoby brak dalszych darmowych aktualizacji.
- **Optimus UI** (społecznościowy fork PrimeNG na licencji MIT) — w momencie decyzji nie miał jeszcze
  pierwszego wydania. Zbyt ryzykowne dla projektu dyplomowego.
- **Angular Material** — konflikty stylów z Tailwindem i konieczność ciągłego nadpisywania styli.
  Estetyka Material Design nie pasuje do sklepu streetwearowego.
- **SCSS + Tailwind** — niewspierane w Tailwind v4.

## Konsekwencje
- Pozytywne: pełna kontrola nad wyglądem; brak zależności od licencji zewnętrznego dostawcy UI;
  dostępność oparta na sprawdzonym CDK; jeden system stylów w całej aplikacji.
- Negatywne / koszty: API spartan/ui jest wciąż młode, więc przy aktualizacjach mogą pojawić się zmiany
  łamiące kompatybilność, a skopiowane komponenty aktualizujemy ręcznie. Złożone komponenty
  (np. zaawansowany datepicker, tabela z wieloma funkcjami) wymagają więcej pracy niż w gotowych bibliotekach.
- W kodzie:
  - zakaz instalowania PrimeNG i Angular Material (`@angular/material`) oraz używania SCSS/Sass
    (`inlineStyleLanguage: "css"` w `angular.json`);
  - nowe komponenty UI dodajemy przez CLI spartana i modyfikujemy w repozytorium, nie przez nową bibliotekę;
  - biblioteka trackera (`frontend/projects/tracker`) nie zależy od Tailwinda, spartan/ui ani CDK (ADR-0006).
