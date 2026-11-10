Sprint 12 Retrospective --- Work Order Lifecycle & Control

Sprint outcome: Core Work Order lifecycle functionality implemented
and manually verified. Final closeout remains subject to the focused
code/test items listed below.

What went well
- The Work Order lifecycle now models technician execution,
  conclusion, inspection, rework, supervisor completion and archival.
- The failed-inspection path was exercised end to end: inspection
  failed, technician started rework, work was concluded again, the
  Work Order was reinspected, passed and completed.
- Hold/resume behaviour was implemented and manually tested, including
  restoring the previous workflow status.
- Kafka event publication and diagnostic consumption were verified
  using a real lifecycle sequence.
- Notification Worker emails were received and confirmed.
- Authentication and authorization were tested with authorized and
  unauthorized users; expected 403 responses were observed.
- Existing automated tests already cover substantial domain and
  handler behaviour, particularly hold/resume and completion.

Challenges and lessons learned
- Authorization policy names and role names must not be confused. A
  policy named Supervisor does not mean the authenticated role is
  literally Supervisor; handlers must check actual roles.
- EF Core model changes and enum evolution require care because
  changing enum member ordering can alter persisted numeric values.
  Migration compatibility should be reviewed whenever enum values
  change.
- Kafka publisher routing and key mappings must be kept aligned with
  the event contracts and diagnostic/notification consumers.
- Local infrastructure lifecycle matters: API, Kafka consumers,
  Notification Worker and SMTP test server must be started/stopped
  consistently when diagnosing delivery problems.
- Database persistence followed by Kafka publication leaves a
  reliability gap if publishing fails after the database commit. This
  remains a known follow-up rather than an excuse to expand Sprint 12.

What could be improved
- Add automated handler/domain tests for the newer inspection-result
  and rework paths, plus archive.
- Ensure every handler checks and propagates its domain result before
  saving or reporting success.
- Ensure every endpoint maps unsuccessful results to an appropriate
  HTTP response rather than returning 200 unconditionally.
- Keep diagnostic consumer output, event contracts, notification
  handling and documentation synchronized as new event types are
  added.

Decisions and scope control
- Keep OnHold as the single temporary hold status. Parts-related
  holds use HoldReason.Parts; a separate WaitingForParts state and
  inventory notification workflow are deferred.
- Dedicated technician check-in/check-out timestamps are deferred to a
  later sprint.
- Staff attendance tracking and maximum daily work-time limits remain
  separate deferred capabilities.
- Transactional Outbox, idempotency, retry/backoff and dead-letter
  handling remain future reliability work.
- Do not pull Sprint 13 reporting, Sprint 14 security hardening,
  Sprint 15 React frontend, or Sprint 16 deployment/microservices work
  into Sprint 12.

Action items before formal closeout
1. Fix ArchiveWorkOrderCommandHandler so a failed domain transition
   is not saved or reported as successful.
2. Add focused automated tests for inspection outcome, rework and
   archive.
3. Correct unconditional success responses in the InProgress,
   send-to-inspection and completion endpoints.
4. Run dotnet test locally and record the actual result.
5. Update the main README and API/event documentation with the final
   Sprint 12 behaviour.

Retrospective conclusion
Sprint 12 delivered its central objective: a controlled Work Order
lifecycle with technician execution, inspection, rework, holds,
supervisor closure, Kafka events and stakeholder notifications. The most
valuable functional proof was the complete
failed-inspection/rework/reinspection path. The remaining work is
targeted engineering cleanup and documentation---not a reason to expand
the sprint with deferred inventory, attendance, reporting or
production-reliability features.