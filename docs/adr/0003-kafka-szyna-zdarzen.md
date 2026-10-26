# ADR-0003: Apache Kafka (KRaft) jako szyna zdarzeń

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0002, ADR-0004, ADR-0006

## Kontekst
Każdy event clickstreamu ma trafić niezależnie do trwałego magazynu i do ML. Chcemy też móc
odtworzyć historię (replay), np. po zmianie modelu albo błędzie w zapisie.

## Decyzja
Apache Kafka w trybie KRaft (bez ZooKeepera), jeden broker w środowisku deweloperskim.

**Topiki:**

| Topic | Partycje | Klucz | Retencja | Zawartość |
|---|---|---|---|---|
| `clickstream.events.v1` | 3 | `sessionId` | 7 dni | pojedyncze eventy wg ADR-0006 |
| `clickstream.events.dlq` | 1 | `sessionId` | 7 dni | eventy, których konsument nie przetworzył, z polem `error` |

**Grupy konsumentów:**
- `event-writer` (Shop.EventWriter) — zapis do PostgreSQL,
- `ml-inference` (ML Service) — aktualizacja rekomendacji.

**Zasady:**
- Klucz `sessionId` gwarantuje kolejność eventów w ramach sesji (ta sama partycja).
- Producent: `acks=all`, idempotencja włączona.
- Konsumenci: at-least-once (commit offsetu po przetworzeniu); duplikaty eliminuje `eventId`.
- Wiadomość w JSON. Wersja schematu w nazwie topicu (`v1`) i w polu `schemaVersion`.
- Replay: reset offsetu grupy (`kafka-consumer-groups --reset-offsets --to-datetime ...`).

**Biblioteki:** C# `Confluent.Kafka`, Python `confluent-kafka`.

## Rozważane alternatywy
- **RabbitMQ** — prostszy, ale wiadomość znika po odbiorze, brak replay.
- **Redis Streams** — wystarczyłby, ale słabiej uzasadnia część strumieniową pracy.
- **Redpanda** — zgodna z API Kafki i lżejsza; dopuszczalny zamiennik w compose bez zmian w kodzie.
- **Schema Registry + Avro** — za duży narzut na projekt; zostajemy przy JSON z walidacją.

## Konsekwencje
- Pozytywne: niezależni konsumenci, replay, naturalna kolejność per sesja.
- Negatywne: Kafka jest najcięższym kontenerem; trzeba obsłużyć duplikaty.
- W kodzie: nazwy topiców i grup tylko z konfiguracji (`Kafka__Topic`, `KAFKA_TOPIC`), nie na sztywno.
