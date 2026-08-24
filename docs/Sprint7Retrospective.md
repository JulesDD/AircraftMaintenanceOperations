SPRINT 7 RETROSPECTIVE

A diagram showing the sprint's progress and completion of user stories can be included here.

Sprint 7
│
├── SQL yearly counters
├── Number generation
├── Domain entities
├── Value Object → removed
├── Work Order lifecycle
├── Archive instead of delete
├── CQRS Commands
├── CQRS Queries
├── PATCH design
├── Technician domain
├── Technician statuses
├── DomainResult
├── API endpoints
├── Dependency Injection
├── Maintenance → Work Order workflow
└── Concurrency

Status

Sprint 7: Complete

Sprint 7 became a larger integration sprint than originally expected, but it finished with the core Maintenance Request, Technician, and Work Order API workflows implemented and every API endpoint tested.

Completed

Maintenance Request commands, queries, handlers, validators, and endpoints

Technician commands, queries, handlers, validators, and endpoints

Work Order commands, queries, handlers, validators, and endpoints

Work Order creation flow

Maintenance Request Open -> InProgress transition

Shared EmploymentStatus moved to User

Work Order number generation

Maintenance Request number generation

End-to-end API testing

Important Design Decisions

Shared Employment Status

Pilot and Technician status were unified under User as EmploymentStatus.

User
└── EmploymentStatus

Pilot
├── Rank
└── LicenseNumber

Technician
├── CertificationLevel
└── YearsOfExperience

This avoided conflicting TPH mappings in the shared Users table.

Separate Maintenance Request and Work Order Status

These remain separate because they describe different business lifecycles.

A Maintenance Request being InProgress does not mean a Work Order has the same lifecycle state.

Explicit Maintenance Request Start Operation

Integration testing exposed a missing workflow: Work Orders could only be created from an InProgress Maintenance Request, but the API had no way to start one.

That resulted in:

POST /api/maintenance/{id}/start

Required Work Order Documentation

The database exposed that LaborNotes was required when creating a Work Order. This reinforced the decision that Work Order activity should be documented.

What Went Well

End-to-end testing exposed real business gaps.

Domain behavior remained separate from HTTP concerns.

TPH inheritance was corrected rather than worked around.

Number-generator retry logic was simplified instead of keeping incomplete concurrency code.

The API reached a usable, testable state.

Lessons Learned

Test workflows earlier, not just individual handlers.

Review EF migrations before applying them.

Put shared concepts at the correct domain level.

Keep lifecycle rules in the domain.

Do not add complexity merely because it looks robust.

Technical Debt for Sprint 8

Work Order PATCH/update command, handler, validation, and endpoint

Tighten lifecycle transition rules

Add automated domain/application/integration tests

Properly design number-generator concurrency handling

Review Maintenance Request request-number uniqueness

Review RequestNumber database length

Consider renaming LaborNotes to WorkOrderNotes

Design MaintenanceNotes later as part of a broader notes model

General API/domain cleanup

Final Assessment

Sprint 7 is complete. The most important result was not simply that the endpoints work; integration testing now exposes meaningful domain rules instead of basic plumbing problems.

Sprint 8 should focus on finishing and hardening rather than expanding scope.