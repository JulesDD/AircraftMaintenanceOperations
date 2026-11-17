# Sprint 10 — Kafka Implementation Summary

## Project
**AircraftMaintenanceOperations**

## Sprint objective

Introduce Kafka-based event publishing and consumption, integrate it with the existing .NET application architecture, and validate message processing and error handling.

## 1. Infrastructure

- Configured Kafka using Docker Compose and KRaft mode.
- Added Kafka UI for inspecting topics and messages.
- Configured the Kafka broker at `localhost:9092`.
- Configured Kafka UI at `localhost:8080`.
- Created the `work-order-events` and `inventory-events` topics.
- Configured `work-order-events` with three partitions and replication factor one.
- Created an isolated `consumer-error-tests` topic for manual error-handling tests.

Kafka CLI scripts for the current Apache Kafka image are located under `/opt/kafka/bin/`.

## 2. Event publishing architecture

Introduced the `IEventPublisher` abstraction:

```csharp
IEventPublisher.PublishAsync<T>(
    T @event,
    CancellationToken cancellationToken)
```

The Kafka publishing implementation lives in Infrastructure, while event contracts remain part of the application/domain design.

The publisher serializes events into an envelope containing:

- `EventType`: identifies the event type.
- `Payload`: contains the event data.

### Topic routing

| Event category | Kafka topic | Message key |
|---|---|---|
| Work-order events | `work-order-events` | `WorkOrderId` |
| Inventory issued for a work order | `inventory-events` | `WorkOrderId` |
| Inventory received | `inventory-events` | `InventoryPartId` |
| Inventory adjusted | `inventory-events` | `InventoryPartId` |

Using `IEventPublisher` keeps application handlers from depending directly on a particular messaging implementation.

## 3. Events verified

### Work-order events
- `WorkOrderCreated`
- `TechnicianAssignedToWorkOrder`
- `WorkOrderInProgressEvent`
- `WorkOrderCompletedEvent`
- `WorkOrderSentToInspectionEvent`

### Inventory events
- `InventoryReceivedEvent`
- `InventoryAdjustedEvent`
- `InventoryIssuedForWorkOrderEvent`

Publishing was manually verified using Kafka UI and/or the consumer test program.

## 4. Consumer implementation

Two consumers were implemented in `AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers`.

| Consumer | Topic | Consumer group |
|---|---|---|
| `WorkOrderEventConsumer` | `work-order-events` | `workOrder-group` |
| `InventoryEventConsumer` | `inventory-events` | `inventory-group` |

The consumers deserialize a common `KafkaEventEnvelope`, inspect `EventType`, and deserialize the payload into the corresponding event class.

The implementation includes:
- Null-envelope validation.
- Missing or blank `EventType` detection.
- JSON exception handling.
- Kafka consume-error handling.
- Unsupported-event logging.
- Cancellation and graceful shutdown.

Both consumers use `AutoOffsetReset.Earliest`.

## 5. Error-handling tests

| Test scenario | Expected behavior | Result |
|---|---|---|
| Valid inventory event | Deserialize and process the payload | Passed |
| Missing `EventType` | Log invalid/legacy envelope and skip it | Passed |
| Malformed JSON | Catch and log `JsonException` | Passed |
| Unknown valid event type | Log from the switch `default` branch | Passed |
| Graceful shutdown | Close both consumers cleanly | Passed |
| Build after restoring subscriptions | Compile successfully | Passed |

The error-handling tests used the isolated `consumer-error-tests` topic. The normal subscriptions were restored afterward.

**Testing note:** These were manual tests, not automated unit or integration tests. The malformed-JSON test verified that the exception was caught and logged. Continued processing of a subsequent valid message in the same run was not separately verified.

## 6. Key lessons from implementation

- Kafka topics contain records; an event is an individual record describing something that happened.
- Partitions allow a topic's records to be distributed and processed in parallel.
- Offsets identify positions within partitions.
- Message keys associate records with a business identifier and influence partition assignment.
- Serialization and event contracts must be consistent between producers and consumers.
- Consumer groups allow multiple consumers to share partition assignments.
- Isolated test topics help prevent deliberately invalid messages from polluting normal event topics.

## 7. Follow-up work

### Durable failed-message handling

The current consumers log and skip invalid envelopes and catch malformed JSON. This prevents those handled exceptions from immediately crashing the loop, but does not provide durable recovery by itself.

With automatic offset commits, a skipped message may not be replayed. Consider a dead-letter topic, durable failure records, and a replay procedure.

### Database/Kafka consistency

A database update and Kafka publish are separate operations. If one succeeds and the other fails, database state and published events can become inconsistent.

Consider the **Outbox Pattern** as a future improvement.

### Automated testing

Add automated tests for:
- Valid envelope deserialization.
- Missing `EventType`.
- Malformed JSON.
- Unsupported event types.
- Continued processing after a bad message.
- Event publishing and failure paths.

## 8. Sprint outcome

The sprint established the core Kafka publishing and consumption path for AircraftMaintenanceOperations, including topic routing, event envelopes, consumer error handling, and graceful shutdown.

The solution built successfully after restoring the normal topic subscriptions.

The next stage should focus on reliability, automated testing, and durable handling of failed messages.
