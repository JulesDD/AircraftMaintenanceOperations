Sprint 12 --- Work Order Lifecycle & Control
Sprint objective
Make the Work Order lifecycle reflect technician work, inspection,
holds, rework, and authorized supervisor closure.
Delivered functionality
- Work Order lifecycle transitions covering assignment, active work,
  conclusion, inspection, completion and archival.
- Technician start/InProgress and conclude actions.
- Supervisor inspection submission and pass/fail recording.
- Failed inspection returns the Work Order to the rework path;
  assigned technician can start rework and submit the Work Order for
  inspection again.
- Supervisor completion after successful inspection.
- Work Order archival after completion.
- Temporary OnHold status with saved PreviousStatus.
- Resume restores the saved eligible status (Assigned or
  InProgress).
- Hold reasons: Parts, Clarification, Revision, and Safety.
- Hold notes and inspection/rework notes are validated by
  domain/application rules.
- Technicians (when assigned) and MaintenanceSupervisors can place
  eligible Work Orders on hold, subject to handler
  authorization/business rules.
- Hold, resume, conclusion, inspection, rework and completion events
  are published through the Kafka event flow.
- Notification Worker sends the relevant supervisor/technician
  notifications. End-to-end email delivery has been manually
  confirmed.
- Authentication and authorization have been manually exercised,
  including expected 403 responses for unauthorized users.
Lifecycle
Open → Assigned → InProgress → Concluded → InspectionPending → InspectionPassed → Completed → Archived

Failed inspection path:

InspectionPending → InspectionFailed → InProgress (Rework) → Concluded → InspectionPending

Hold/resume path:

Assigned or InProgress → OnHold → saved previous status
API routes
The supplied WorkOrderEndpoints.cs defines these routes:
  Method            Route                                      Purpose           Authorization policy
  POST              /api/workorders/                         Create Work Order Supervisor

  GET               /api/workorders/                         List Work Orders  Supervisor

  GET               /api/workorders/{id}                     Get Work Order    WorkOrderAccess

  PATCH             /api/workorders/{id}                     Update Work Order WorkOrderAccess

  PATCH             /api/workorders/{id}/assign-technician   Assign technician Supervisor

  PATCH             /api/workorders/{id}/in-progress         Start work        Technician

  PATCH             /api/workorders/{id}/on-hold             Place on hold     WorkOrderHoldResume

  PATCH             /api/workorders/{id}/resume              Resume            WorkOrderHoldResume

  PATCH             /api/workorders/{id}/concluded           Conclude work     Technician

  PATCH             /api/workorders/{id}/inspection          Send to           Supervisor
inspection        

  PATCH             /api/workorders/{id}/inspection-result   Record pass/fail  Supervisor
outcome           

  PATCH             /api/workorders/{id}/rework              Start rework      Technician

  PATCH             /api/workorders/{id}/complete            Complete after    Supervisor
inspection        
PATCH             /api/workorders/{id}/archive             Archive completed Supervisor
Work Order
The endpoint policy names are not themselves domain roles. Handler-level
role checks must compare actual role names such as Admin and
MaintenanceSupervisor.
Kafka event flow
The diagnostic consumer output supplied during testing demonstrated
these events for one Work Order, in order:
1. WorkOrderCreatedEvent
2. TechnicianAssignedToWorkOrderEvent
3. WorkOrderInProgressEvent
4. WorkOrderConcludedEvent
5. WorkOrderSentToInspectionEvent
6. WorkOrderInspectionFailedEvent
7. WorkOrderReworkStartedEvent
8. WorkOrderConcludedEvent
9. WorkOrderSentToInspectionEvent
10. WorkOrderInspectionPassedEvent
11. WorkOrderCompletedEvent
These events were observed on work-order-events, keyed by WorkOrderId
in the supplied sample. Hold/resume events were also implemented and
tested separately.
Notifications
Manual end-to-end verification confirmed delivery of the relevant
emails/notifications. Implemented notification workflows include
technician assignment, Work Order conclusion to supervisors, hold
notifications to active supervisors, resume notification to the assigned
technician, inspection outcome notifications to the assigned technician,
and completion notification to the assigned technician.
Explicitly deferred
- WaitingForParts as a separate status and inventory-driven
  notification workflow. Current parts holds use OnHold with
  HoldReason.Parts.
- Dedicated technician check-in/check-out timestamps. This is a
  later-sprint enhancement; event occurrence timestamps are not
  represented as a substitute.
- Staff attendance check-in/check-out.
- Maximum daily work-time limits.
- Transactional Outbox, idempotent consumers, bounded retries and
  dead-letter handling.
- Reporting & Analytics (Sprint 13), Security & API Hardening (Sprint
  14), React Frontend (Sprint 15), and Microservices/DevOps/Cloud
  (Sprint 16).
Known technical follow-ups from source review
- Add automated tests for RecordInspectionCommandHandler,
  StartReworkWorkOrderCommandHandler, and
  ArchiveWorkOrderCommandHandler.
- Add domain tests for inspection pass/fail and rework transitions.
- Fix ArchiveWorkOrderCommandHandler to check the DomainResult
  returned by ArchiveWorkOrder before saving and returning success.
- In WorkOrderEndpoints.cs, the in-progress, inspection
  (send-to-inspection), and complete routes currently return HTTP
  200 unconditionally; align response mapping with handler result
  success.
- Run and record the full test suite locally.
Closeout status
The main functional workflows have been manually verified, including
failed inspection/rework/reinspection, successful inspection/completion,
archive, hold/resume, Kafka event publication/consumption, notifications
and unauthorized 403 responses.

Automated coverage exists for significant portions of the lifecycle but
has identified gaps; see Sprint12_TestCoverageReview.md. Do not mark
the automated test suite as fully verified until the remaining tests are
added or explicitly accepted as deferred and dotnet test is run
successfully.