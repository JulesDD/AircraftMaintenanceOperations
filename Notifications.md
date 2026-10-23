# Notification Service

## Purpose

The Notification Service sends email notifications for selected Work
Order events. It runs as an independent .NET Worker Service, separate
from the ASP.NET Core API, and consumes events asynchronously from
Apache Kafka.

## Architecture

``` text
API command handler
    |
    | persists business change
    | publishes work-order event
    v
Kafka topic: work-order-events
    |
    v
Notification Worker
    |
    +--> Deserialize KafkaEventEnvelope
    |
    +--> Route supported event type
    |
    +--> Resolve Work Order and recipient from database
    |
    +--> Build notification email
    |
    v
IEmailSender
    |
    v
SMTP server (Papercut in local development)
```

The API publishes events; the Notification Worker consumes them. The
worker has its own host and database registration so it does not need to
register the API's complete infrastructure service set.

## Implemented Workflows

### Technician assigned to Work Order

1.  The application publishes `TechnicianAssignedToWorkOrderEvent`.
2.  The event includes `EventId`, `WorkOrderId`, `TechnicianId`,
    `AssignedByUserId`, and `OccurredAt`.
3.  The worker routes the event using the Kafka envelope's event type.
4.  The worker resolves the technician and prepares an email.
5.  The email sender sends the message through SMTP.

### Work Order completed

1.  The application publishes `WorkOrderCompletedEvent` when the
    completion workflow is processed.
2.  The event includes `EventId`, `WorkOrderId`, `AircraftId`,
    `CompletedByUserId`, and `OccurredAt`.
3.  The worker loads the Work Order and resolves its assigned
    technician.
4.  The worker sends the completion email to the assigned technician.

The current recipient rule is the **assigned technician**, not the user
who assigned the Work Order.

## Kafka Processing Behavior

-   **Topic:** `work-order-events`
-   **Consumer group:** `aircraft-maintenance-notification-worker`
-   **Offset reset policy:** `Earliest`
-   **Auto-commit:** Disabled
-   **Commit behavior:** A Kafka record is committed after event
    processing succeeds.
-   Supported events are routed by `EventType` in `KafkaEventEnvelope`.
-   Malformed envelopes and unsupported event types are logged and
    skipped/committed according to the current consumer implementation.
-   SMTP send exceptions propagate out of event processing, so the
    commit statement is not reached for that processing attempt.

Manual commits reduce the risk of losing a notification because it was
committed before processing. They do not create exactly-once email
delivery.

## Local Development Configuration

The worker configuration should include equivalent settings to the
following. Keep secrets and environment-specific values out of source
control; use user secrets or environment variables where appropriate.

``` json
{
  "ConnectionStrings": {
    "AircraftMaintenanceConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AircraftMaintenanceDb;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=false"
  },
  "Kafka": {
    "BootstrapServers": "localhost:9092",
    "Topic": "work-order-events",
    "ConsumerGroupId": "aircraft-maintenance-notification-worker"
  },
  "Smtp": {
    "Host": "localhost",
    "Port": 25,
    "FromAddress": "notifications@aircraftmaintenance.local"
  }
}
```

These names reflect the settings used by the current implementation as
documented during development. Confirm them against the worker's actual
configuration classes and `appsettings` before copying the sample into a
new environment.

### Local prerequisites

-   SQL Server LocalDB is installed and the application database is
    available.
-   Kafka is running and reachable at `localhost:9092`.
-   The `work-order-events` topic is available (or topic auto-creation
    is configured as intended).
-   Papercut SMTP is running and listening on `localhost:25`.
-   The database contains the Work Order and technician records needed
    to resolve the notification recipient.

## Running and Verifying Locally

1.  Start SQL Server LocalDB and ensure the application database is
    current.
2.  Start Kafka and verify the broker is reachable.
3.  Start Papercut and confirm its SMTP listener is available on port
    25.
4.  Start `AircraftMaintenanceOperations.NotificationWorker`.
5.  Trigger a technician assignment through the API and verify the
    assignment email in Papercut.
6.  Trigger the Work Order completion workflow and verify the completion
    email is sent to the assigned technician.
7.  Review worker logs for event handling and the Kafka commit after
    successful processing.

## SMTP Failure Test

The failure scenario tested during Sprint 11 was:

1.  Stop Papercut.
2.  Trigger a fresh Work Order completion event.
3.  Confirm the worker logs an SMTP connection failure.
4.  Observe that the unhandled exception currently stops the Worker
    host.
5.  Restart Papercut and the Notification Worker.
6.  Confirm expected email delivery after restart.

The test confirms failure detection and recovery of the notification
flow. To conclusively prove that the exact same Kafka record was
redelivered, correlate the event ID and topic/partition/offset in logs
before and after restart. Email receipt alone does not establish that
the record was redelivered rather than another event being processed.

## Error Handling and Reliability

### Current behavior

The consumer uses manual commits. The commit statement follows
successful event processing, so an SMTP exception prevents that
processing attempt from reaching the commit.

The host currently stops when an unhandled exception escapes the
`BackgroundService`. This behavior was observed during the SMTP outage
test.

### At-least-once implications

The design favors not committing a notification event before processing
completes, but it cannot guarantee exactly-once email delivery:

-   If sending fails before success, the event is not committed by that
    attempt.
-   If sending succeeds but the worker crashes before committing the
    offset, the event can be processed again and the email can be
    duplicated.
-   If a business change is committed to SQL Server but Kafka
    publication fails afterward, the event may never reach the worker
    because a transactional outbox is not currently implemented.

### Future improvements

-   Add bounded retry attempts with exponential backoff and cancellation
    support for transient SMTP failures.
-   Define a deliberate policy for persistent failures and poison
    messages.
-   Add structured logs containing `EventId`, event type, topic,
    partition, and offset.
-   Add idempotency or deduplication where appropriate.
-   Consider a transactional outbox for database-to-Kafka publication.
-   Add automated integration tests for redelivery, retry exhaustion,
    and duplicate-send scenarios.

## Scope

Email is the first delivery channel. SMS notifications are deferred.
This document describes the current implementation and observed
behavior, not future capabilities that have not yet been built.
