# ADR-0006: Schemat eventu i działanie trackera

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0003, ADR-0007, ADR-0011

## Kontekst
Tracker w Angularze, API w C#, EventWriter w C# i ML w Pythonie muszą rozumieć te same dane.
Tracker nie może spowalniać strony ani gubić ostatnich eventów przy zamknięciu karty.

## Decyzja

### Paczka z przeglądarki (`POST /api/events`)
```json
{
  "schemaVersion": 1,
  "sessionId": "c0f7c6a2-...",
  "sentAt": "2026-10-21T13:05:10.120Z",
  "context": { "...": "pola z ADR-0007, wysyłane tylko w pierwszej paczce sesji" },
  "events": [
    {
      "eventId": "9b1c...",
      "type": "product_view",
      "ts": "2026-10-21T13:05:02.004Z",
      "page": "/p/oversized-hoodie-black",
      "productId": 123,
      "payload": {}
    }
  ]
}
```
API odpowiada `202 Accepted`. Limit: 50 eventów i 64 KB na paczkę.

### Pojedynczy event w Kafce (`clickstream.events.v1`)
Pola eventu z paczki + `sessionId`, `schemaVersion`, `serverTs` (czas API, UTC) i `variant` (ADR-0011).

### Typy eventów i payload

| `type` | `payload` |
|---|---|
| `page_view` | `{ "pageType": "home|category|product|cart|search" }` |
| `product_view` | `{ "listPosition": 3, "fromSource": "recs|category|search|direct" }` |
| `product_dwell` | `{ "visibleMs": 18500 }` — wysyłany przy opuszczeniu karty produktu |
| `scroll_depth` | `{ "maxPercent": 75 }` — progi 25/50/75/100 |
| `image_interaction` | `{ "action": "zoom|next|prev", "imageIndex": 2 }` |
| `filter_applied` | `{ "filters": { "size": ["L"], "color": ["black"], "priceMax": 300 } }` |
| `search` | `{ "query": "hoodie", "resultsCount": 14 }` |
| `add_to_cart` / `remove_from_cart` | `{ "quantity": 1, "size": "L" }` |
| `purchase` | `{ "orderId": 77, "productIds": [123, 88] }` |
| `rec_impression` | `{ "slot": "home_for_you", "productIds": [..], "modelVersion": "...", "strategy": "model" }` |
| `rec_click` | `{ "slot": "home_for_you", "position": 2, "modelVersion": "...", "strategy": "model" }` |

### Zachowanie trackera
- Biblioteka `frontend/projects/tracker`, bez zależności od komponentów sklepu.
- Działa tylko po udzieleniu zgody (ADR-0007). Bez zgody: brak wysyłki, brak `sessionId`.
- `sessionId`: UUID w `sessionStorage`; nowy po 30 min bezczynności.
- `eventId`: `crypto.randomUUID()`.
- Bufor w pamięci, flush co 5 s **albo** przy 20 eventach (throttling).
- Scroll i zmiany filtrów: debounce 500 ms.
- Dwell: liczony tylko gdy karta jest widoczna (Page Visibility API).
- Widoczność rekomendacji i zdjęć: `IntersectionObserver` (impression = min. 50% widoczności przez 1 s).
- Przy `visibilitychange → hidden` i `pagehide`: flush przez `navigator.sendBeacon`.
- Błędy sieci: jedna ponowna próba, potem porzucenie paczki (nie blokujemy UI).

### Walidacja w API
Sprawdzenie `schemaVersion`, typów, długości pól, `ts` nie dalej niż ±24 h od `serverTs`.
Rate limit na `sessionId` i IP (wbudowany `RateLimiter` ASP.NET Core). Prosty filtr botów po User-Agencie
(UA używany tylko do decyzji, nie jest zapisywany).

## Rozważane alternatywy
- **Wysyłka każdego eventu osobno** — dużo żądań, gorsza wydajność strony.
- **Gotowe narzędzia (Snowplow, PostHog)** — odbierają projektowi autorską część.

## Konsekwencje
- Pozytywne: jeden kontrakt dla trzech języków, deduplikacja dzięki `eventId`, atrybucja rekomendacji.
- Negatywne: zmiany schematu trzeba robić równocześnie w TS, C# i Pythonie.
- W kodzie: typy kontraktu trzymamy w jednym miejscu na język (`tracker/src/schema.ts`,
  `Shop.Core/Events/`, `ml/service/schema.py`), z testem na przykładowym JSON z `docs/examples/`.
