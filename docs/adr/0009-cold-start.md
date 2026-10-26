# ADR-0009: Cold start — popularność kontekstowa

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0005, ADR-0007, ADR-0008

## Kontekst
Nowa sesja nie ma historii, więc model sesyjny nie ma wejścia. Dodatkowo ML może nie zdążyć
policzyć rekomendacji przed pierwszym renderem. Dane kontekstowe muszą realnie wpływać na wynik.

## Decyzja
Trening nocny liczy ranking popularności dla każdego **klucza kontekstu**:
`contextKey = {deviceType}:{daypart}:{referrerType}` (np. `mobile:evening:social`).

Popularność = ważona suma interakcji z ostatnich 14 dni (te same wagi co w ADR-0008),
z wygładzaniem: gdy kontekst ma < 30 sesji, łączymy go z rankingiem ogólnym.

Wynik w Redisie: `popular:{contextKey}` i `popular:global` (ADR-0005).

**Kolejność fallbacku w API (`GET /api/recommendations`):**
1. `recs:{sessionId}` istnieje → zwróć.
2. Sesja ma kontekst → `popular:{contextKey}`.
3. `popular:global`.
4. Redis niedostępny → zapytanie SQL o najczęściej kupowane produkty (cache w pamięci API 10 min).

W odpowiedzi pole `strategy` mówi, która ścieżka zadziałała (logowane w `rec_impression`).

## Konsekwencje
- Pozytywne: sklep zawsze coś pokazuje; kontekst ma mierzalny wpływ (porównanie `popular:global` vs kontekstowy).
- Negatywne: przy małej liczbie danych rankingi kontekstowe są niestabilne (stąd wygładzanie).
