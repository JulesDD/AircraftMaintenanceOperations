# Sprint 11 Retrospective --- Notification Service

**Outcome:** Core notification functionality implemented and verified.\
**Status:** Ready to close after documentation is committed.

## Sprint Goal

Introduce an independent Notification Worker that consumes work-order
events from Kafka and sends email notifications without coupling email
delivery to the API request lifecycle.

## Delivered

-   Created an independent .NET Worker Service for notification
    processing.
-   Connected the worker to Kafka topic `work-order-events` using
    consumer group `aircraft-maintenance-notification-worker`.
-   Added event-envelope handling and routing for supported work-order
    event types.
-   Implemented technician-assignment email notifications.
-   Implemented Work Order completion email notifications.
-   Corrected completion notification recipient resolution to target the
    technician assigned to the Work Order.
-   Kept email delivery behind an email-sender abstraction, with SMTP
    delivery implemented using MailKit.
-   Registered database access for the worker independently rather than
    registering the API's full infrastructure service set.
-   Used manual Kafka offset commits after successful event processing.
-   Verified the solution build and existing test suite.

## Verification Completed

### End-to-end delivery

-   Technician assignment notifications were triggered and received.
-   Work Order completion notifications were triggered and received.
-   The completion email recipient was checked and corrected to the
    assigned technician.
-   Restarting the worker with the same consumer group did not produce
    unexpected repeat notifications for previously committed events in
    the observed test.

### SMTP failure and recovery

Papercut was stopped and a fresh Work Order completion notification was
triggered. The worker logged an SMTP connection-refused exception. The
exception propagated through event processing and stopped the Worker
host under the configured `BackgroundServiceExceptionBehavior`.

Papercut and the Notification Worker were restarted, and the expected
emails were received.

**Result:** SMTP failure detection and recovery of the notification flow
were verified. To prove conclusively that the exact same original Kafka
record was redelivered, logs must show that record being consumed again
after restart. Email receipt alone does not prove that detail.

### Build and tests

The final build and existing test suite were confirmed green.

## What Went Well

-   The notification service is independently hosted rather than
    embedded in the API.
-   Event-driven processing separates notification delivery from the
    API's synchronous request path.
-   The email provider is abstracted, allowing future provider changes.
-   Manual commits avoid committing an event before its notification
    processing succeeds.
-   End-to-end tests uncovered and corrected a recipient-selection
    issue.
-   Failure testing exposed runtime behavior rather than relying only on
    the happy path.
-   Build and test verification remained green at sprint closeout.

## Challenges and Lessons Learned

### Recipient resolution must match the business workflow

A Work Order completion notification should go to the assigned
technician in the current workflow. Notification tests should verify
both successful delivery and recipient identity.

### Manual commits do not guarantee exactly-once delivery

If an email is sent successfully and the worker crashes before
committing the Kafka offset, the message can be processed again and the
recipient may receive a duplicate. Email delivery and Kafka offset
commits are not one atomic transaction.

### SMTP failure currently stops the worker

An unhandled exception from the email sender escapes event processing.
With the host configured to stop on an unhandled `BackgroundService`
exception, a temporary SMTP outage stops the Worker host. Restarting the
services restored processing in the tested scenario, but automated
recovery is not implemented.

### Database persistence and Kafka publishing are not atomic

If the API saves a business change and then fails to publish the event,
SQL Server and Kafka can become inconsistent. A transactional outbox is
a future reliability improvement.

## Known Limitations

-   No bounded retry policy or exponential backoff for transient SMTP
    failures.
-   No transactional outbox for reliable publication after database
    changes.
-   Duplicate email delivery remains possible if sending succeeds but
    committing the offset does not.
-   The observed restart recovery test did not by itself prove that the
    exact same Kafka record was redelivered.
-   SMS notifications were deferred.

## Follow-up Actions

  -----------------------------------------------------------------------------
  Priority                Action                  Reason
  ----------------------- ----------------------- -----------------------------
  High                    Add bounded retries and Avoid stopping the worker for
                          backoff for transient   a temporary provider outage
                          SMTP errors             

  High                    Add tests for failed    Protect message-processing
                          delivery and offset     guarantees
                          behavior                

  Medium                  Consider a              Reduce the
                          transactional outbox    database-save/Kafka-publish
                                                  consistency gap

  Medium                  Consider idempotency or Reduce duplicate emails after
                          deduplication           retries or restarts

  Medium                  Add structured logs     Make recovery and redelivery
                          with event ID, event    easier to verify
                          type, topic, partition, 
                          and offset              

  Deferred                Add SMS delivery        Email-first scope was
                                                  intentional
  -----------------------------------------------------------------------------

## Sprint Assessment

**The core Sprint 11 goal was achieved.** The worker consumes Kafka
events, resolves recipients, sends email, and has been tested through
successful delivery and an SMTP outage/restart scenario. Remaining work
is reliability hardening, not a blocker to documenting the current
implementation.

## Closing Note

Sprint 11 moved the platform from planned Kafka and notification
capabilities to an implemented, independently hosted email notification
flow. The key lesson is that reliable event-driven delivery requires
attention not only to the happy path, but also to retries, offset
commits, idempotency, and the consistency boundary between the database
and the message broker.
