# ADR-0001: Dokumentujemy decyzje w formie ADR

- **Status:** Accepted
- **Data:** 2026-10-21

## Kontekst
Projekt łączy pięć technologii w trzech językach i jest rozwijany z pomocą Claude Code.
Agent nie pamięta wcześniejszych sesji, a decyzje rozproszone po czacie giną.
Przy obronie pracy trzeba też uzasadnić każdy wybór.

## Decyzja
Każdą istotną decyzję architektoniczną zapisujemy jako ADR w `docs/adr/` (format Michaela Nygarda,
po polsku). `CLAUDE.md` nakazuje agentowi czytać powiązany ADR przed zmianą i nie łamać
zaakceptowanych ADR-ów bez nowego ADR-a. Nowe ADR-y tworzy komenda `/new-adr`.

## Konsekwencje
- Pozytywne: agent i promotor widzą uzasadnienia; ADR-y to gotowy materiał do rozdziału „Projekt systemu”.
- Negatywne: trzeba pilnować aktualności indeksu w `docs/adr/README.md`.
