# Architecture Decision Records

ADR opisuje jedną decyzję: kontekst, wybór, konsekwencje. ADR-ów nie edytujemy merytorycznie
po zaakceptowaniu. Zmiana decyzji to nowy ADR, a stary dostaje status `Superseded by ADR-XXXX`.

Statusy: `Proposed` (do potwierdzenia), `Accepted`, `Deprecated`, `Superseded by ADR-XXXX`.

## Indeks

| Nr | Tytuł | Status | Obszar |
|---|---|---|---|
| [0001](0001-uzywamy-adr.md) | Dokumentujemy decyzje w formie ADR | Accepted | proces |
| [0002](0002-architektura-uslug.md) | Podział na usługi i granice odpowiedzialności | Accepted | architektura |
| [0003](0003-kafka-szyna-zdarzen.md) | Apache Kafka (KRaft) jako szyna zdarzeń | Accepted | infrastruktura |
| [0004](0004-postgresql-jedyna-baza.md) | PostgreSQL jako jedyna baza danych | Accepted | dane |
| [0005](0005-redis-bufor-rekomendacji.md) | Redis jako bufor rekomendacji i stanu sesji | Accepted | dane |
| [0006](0006-schemat-eventu-i-tracker.md) | Schemat eventu i działanie trackera | Accepted | kontrakt |
| [0007](0007-dane-kontekstowe-i-zgoda.md) | Dane kontekstowe zamiast fingerprintingu, zgoda użytkownika | Accepted | prywatność |
| [0008](0008-strategia-ml.md) | Strategia ML: trening wsadowy, inferencja na żywo, drabinka modeli | Accepted | ML |
| [0009](0009-cold-start.md) | Cold start: popularność kontekstowa | Accepted | ML |
| [0010](0010-dane-treningowe.md) | Dane treningowe: publiczny dataset i symulator | Accepted | ML |
| [0011](0011-ewaluacja-i-eksperyment.md) | Ewaluacja offline i grupa kontrolna | Accepted | ML / analityka |
| [0012](0012-dashboard.md) | Dashboard: ECharts i agregaty w widokach zmaterializowanych | Accepted | frontend / analityka |
| [0013](0013-autoryzacja-admina.md) | Autoryzacja panelu admina przez JWT | Accepted | bezpieczeństwo |
| [0014](0014-docker-compose.md) | Docker Compose jako środowisko uruchomieniowe | Accepted | infrastruktura |
| [0015](0015-wersjonowanie-modeli.md) | Wersjonowanie i hot-reload modeli | Accepted | ML |
| [0016](0016-styling-i-komponenty-ui.md) | Styling i komponenty UI: Tailwind CSS v4, spartan/ui, Angular CDK | Accepted | frontend |
| [0017](0017-wykresy-i-tabele-panelu-admina.md) | Wykresy i tabele w panelu admina: ngx-echarts, spartan/ui, TanStack Table | Accepted | frontend / analityka |
| [0018](0018-ikony-i-przejscia-widokow.md) | Ikony i przejścia między widokami: @lucide/angular, View Transitions API | Accepted | frontend |

## Szablon

Nowy ADR: komenda `/new-adr` w Claude Code albo kopia poniższego.

```markdown
# ADR-NNNN: Tytuł

- **Status:** Proposed
- **Data:** RRRR-MM-DD
- **Powiązane:** ADR-XXXX

## Kontekst
Jaki problem rozwiązujemy i jakie mamy ograniczenia.

## Decyzja
Co wybieramy. Konkretnie, w trybie oznajmującym.

## Rozważane alternatywy
- **Opcja A** — dlaczego nie.

## Konsekwencje
- Pozytywne:
- Negatywne / koszty:
- Co trzeba zrobić w kodzie:
```
