# ✈️ Aircraft Maintenance Operations Platform

A backend-focused aircraft maintenance operations platform built with
ASP.NET Core, .NET, Clean Architecture, CQRS, Domain-Driven Design
(DDD), Entity Framework Core, SQL Server, and Apache Kafka. The platform
manages aircraft, pilots, technicians, maintenance requests, work
orders, and inventory, with asynchronous email notifications handled by
an independent worker service.

The project is developed incrementally through agile sprints to
demonstrate enterprise-style backend engineering, testing, and
architectural decision-making. Its long-term direction is to evolve into
a shared aviation maintenance collaboration platform connecting
aircraft operators and Approved Maintenance Organizations (AMOs).

## 🚀 Project Status

| Module | Status |
|---------|:------:|
| Aircraft Management | ✅ Complete |
| Pilot Management | ✅ Complete |
| Technician Management | ✅ Complete |
| Work Orders | ✅ Complete |
| Maintenance Requests | ✅ Complete |
| Inventory Management and Audit History | ✅ Complete |
| Authentication & Authorization | ✅ Complete |
| Employee / User / Role / Profile Foundation | ✅ Complete |
| Inventory Hardening | ✅ Complete |
| Technical Debt Cleanup | ✅ Complete |
| Kafka Messaging | ✅ Implemented |
| Notification Worker — Email | ✅ Implemented and end-to-end tested |
| SMTP Failure / Recovery Verification | ✅ Tested |
| Work Order Lifecycle & Control Improvements | 🔄 In progress — Sprint 12 |
| Reporting and Analytics | 📅 Planned |
| Security & API Hardening | 📅 Planned — Sprint 14 |
| React Frontend & Platform Integration | 📅 Planned |
| Microservices, DevOps & Cloud | 📅 Planned |

**Current milestone: Sprint 12 — Work Order Lifecycle & Control.**
The focus is on formalizing Work Order execution, conclusion,
inspection, rework, supervisor closure, and hold/resume behavior.

Immediate repository and credential security remediation is being
handled as a focused risk-reduction task alongside development. It
should not be confused with the broader security hardening planned for
Sprint 14.

## 🎯 Product Vision

The long-term vision is a **shared aviation maintenance collaboration
platform** where aircraft operators and AMOs coordinate maintenance
while retaining control of their own records, workflows, and information.

The practical starting point is a strong AMO maintenance-management
workspace. The signature product experience to validate is controlled
operator-to-AMO collaboration—not simply another Work Order system.

### Core product promises

- **Coordinate:** Connect defect submission, maintenance requests, AMO
  responses, Work Orders, progress updates, inspections, and completion
  documentation.
- **Control:** Preserve organization ownership, permission boundaries,
  record provenance, and auditability.
- **Connect:** Exchange only the maintenance information explicitly
  authorized for another organization.

This is a product-positioning hypothesis to validate with aviation
customers, not a claim that the product is first or unique in the market.

## 🛠️ Technology Stack

- C# and .NET
- ASP.NET Core Web API
- Entity Framework Core and SQL Server
- MediatR and CQRS
- Carter and Minimal APIs
- FluentValidation and Mapster
- Swagger / OpenAPI
- ASP.NET Identity and JWT authentication
- Apache Kafka
- MailKit / SMTP
- Docker (development and deployment work is evolving)

Check the target frameworks in the solution files for the exact
framework version used by each project.

## 🏗️ Architecture

The API follows Clean Architecture, with a separate Notification Worker
for asynchronous event processing.

```text
src/
├── AircraftMaintenanceOperations.Domain
├── AircraftMaintenanceOperations.Application
├── AircraftMaintenanceOperations.Infrastructure
├── AircraftMaintenanceOperations.API
└── AircraftMaintenanceOperations.NotificationWorker
```

### Domain

Contains domain entities, enums, behavior, and domain-facing
abstractions. The user foundation includes `User` with specialized
`Pilot` and `Technician` profiles.

### Application

Contains commands, queries, handlers, validators, DTOs/results, and
MediatR pipeline behaviors.

### Infrastructure

Contains EF Core persistence, SQL Server configuration, entity
configurations, Identity integration, and infrastructure
implementations. Kafka event publishing and the shared Kafka event
envelope are part of the messaging infrastructure.

### API

Contains Carter endpoint modules, HTTP routing, authorization, OpenAPI
metadata, and request/response handling.

### Notification Worker

Runs independently from the API. It consumes work-order events from
Kafka, resolves notification recipients using persisted application
data, builds notification messages, and sends email through SMTP. Email
delivery is behind an abstraction so the delivery implementation can be
changed without coupling notification workflows directly to a provider.

## 🔐 Authentication & Authorization

ASP.NET Identity provides authentication infrastructure.
`ApplicationUser` maps the authenticated Identity account to the domain
user through `DomainUserId`. Authorization is currently role-based,
with resource-level checks where required.

Current roles:

- `Admin`
- `MaintenanceSupervisor`
- `Technician`
- `InventoryClerk`
- `Pilot`
- `OperationsManager`

JWT role claims represent the user's authorization role at token
creation time.

### Planned multi-organization authorization

The future hybrid model will introduce organization-scoped memberships
and permissions. A user may belong to multiple organizations and hold
different roles in each. Access to another organization's resources
will require resource ownership or an active collaboration and
applicable sharing permission.

## 📦 Inventory

Inventory capabilities include Inventory Part creation and lookup,
receiving, issuing and adjusting stock, inventory transaction history,
and low-stock/restock queries. Stock-changing operations create audit
transactions recording the relevant inventory part, Work Order when
applicable, performing domain user, transaction type, quantity
information, and reason/context.

Reserve Inventory remains deferred until a concrete business requirement
exists for allocating and reserving stock.

## 📨 Event Messaging & Notifications

The Notification Worker consumes the `work-order-events` Kafka topic
using the consumer group
`aircraft-maintenance-notification-worker`.

Implemented workflows:

- **Technician assigned:** sends an email to the assigned technician.
- **Work Order completed:** sends an email to the assigned technician
  when the completion event is processed.

The consumer uses manual Kafka offset commits and commits after event
processing succeeds. If email delivery fails before the commit, that
processing attempt does not commit the message.

**Known limitations:** an unhandled SMTP exception currently stops the
Worker host; bounded retries and backoff are future improvements. Email
delivery is not exactly-once: a crash after sending but before
committing can result in a duplicate email. The
database-save-to-Kafka-publish gap is not yet protected by a
transactional outbox.

A Transactional Outbox and stronger event reliability are planned
production-readiness improvements.

See [`docs/Notifications.md`](docs/Notifications.md) for configuration,
processing behavior, testing, and limitations.

## 🛩️ Hybrid AMO & Aircraft Operator Model

The long-term product will support organizations that operate as an
Aircraft Operator, an AMO, or both.

The current architectural decisions are:

1. **Organization capabilities:** An organization can act as an
   Aircraft Operator, an AMO, or both.
2. **Aircraft record control:** The designated aircraft operator
   controls the authoritative aircraft record. Legal ownership and
   operational record control are not assumed to be identical.
3. **Maintenance record sharing:** Sharing is explicit and controlled.
   A collaboration does not automatically expose every record.

### Proposed ownership boundaries

| Domain | Ownership |
|---|---|
| User identity | Platform-level identity; memberships are organization-scoped |
| Aircraft profile | Controlling aircraft operator |
| Maintenance request | Requesting organization |
| Work Order | Performing AMO |
| Inventory and internal labour | Performing AMO |
| Collaboration | Participating organizations, within defined scope |
| Shared records | Available only through explicit grants and authorization |

The AMO will be able to access aircraft information required for an
authorized maintenance engagement without receiving unrestricted edit
access to the operator's aircraft record. The operator will receive an
approved progress view, not automatic access to internal AMO notes,
labour details, inventory, or private documents.

These are target architecture decisions; the multi-organization model
is not yet represented as a completed implementation.

## ✅ Validation & Business Rules

Validation is split between application-level input validation and
domain behavior.

**FluentValidation** handles input concerns such as required fields,
string lengths, numeric ranges, dates, and email formatting. The
**Domain Layer** owns business rules and state transitions such as
employment lifecycle changes, role changes, aircraft and Work Order
behavior, inventory quantity changes, and maintenance lifecycle
behavior.

## 🧪 Testing

The project uses unit, handler, integration, and regression tests.
Verified coverage includes employee/user creation and updates, role
changes, specialized profile persistence, authorization after role
changes, authenticated inventory operations and audit behavior,
Inventory/Work Order interactions, Kafka-driven notification workflows,
assignment and completion email delivery, and SMTP failure/recovery
behavior.

The solution build and existing test suite were green at Sprint 11
closeout. Sprint 12 changes continue to require their own build and
test verification.

## 🧩 Design Patterns & Practices

- Clean Architecture
- CQRS and MediatR
- Domain-Driven Design
- Dependency Injection
- Factory Methods
- Domain Result Pattern
- Validation Pipeline Behavior
- Entity Framework Core
- Table-per-hierarchy (TPH) inheritance for specialized user profiles
- Event-driven integration through Kafka
- Provider abstraction for email delivery
- Modular monolith as the intended near-term application structure

## 🗺️ Roadmap

The original Sprints 8–16 remain the core development roadmap.

| Sprint | Focus |
|---|---|
| 8 | Authorization / inventory transactions |
| 9 | Employee, role, and inventory foundation |
| 10 | Kafka and event-driven architecture |
| 11 | Notification Service |
| 12 | Work Order Lifecycle & Control |
| 13 | Reporting & Analytics |
| 14 | Security & API Hardening |
| 15 | React Frontend & Platform Integration |
| 16 | Microservices, DevOps & Cloud |

### Proposed post-Sprint 16 plan

These are proposed future milestones, subject to customer validation
and technical review—not current implementation commitments.

| Phase / Sprint | Focus |
|---|---|
| Pre-Sprint 17 | Customer discovery, IP ownership, repository/licensing decisions, target-domain review |
| 17 | Organization and Membership Foundation |
| 18 | Tenant Context and Authorization |
| 19 | Operator-Controlled Aircraft Records |
| 20 | Maintenance Provider Relationships and Collaboration |
| 21 | Controlled Sharing and Audit |
| 22 | Operator-to-AMO End-to-End Workflow |
| 23 | Operator Portal and Pilot Experience |
| 24 | Reliability and Production Readiness |
| 25 | Integrations and Evidence-Led Expansion |

The intended initial tenant approach is a shared SQL database and schema
with organization-scoped ownership. Authorization must verify
organization membership, permissions, resource ownership, collaboration,
and applicable sharing grants. EF Core query filters are a supporting
safeguard, not the security boundary.

### Commercial and validation direction

- **Initial target market:** Independent AMOs and the aircraft
  operators they serve.
- **Product hypothesis:** Make operator-to-AMO collaboration a
  first-class workflow while preserving each organization's control.
- **Validation:** Conduct approximately 15–20 discovery interviews and
  identify 2–3 credible design partners or pilot prospects.
- **Business model hypothesis:** B2B subscriptions, onboarding,
  migration, and optional integrations/services.
- **Deployment hypothesis:** Shared cloud SaaS by default; dedicated
  private cloud when justified; customer-hosted deployment by exception.
- **Funding:** Bootstrap and validate first; investigate Alberta and
  federal programs only after confirming current eligibility.

### Longer-term opportunities

Potential later modules include component lifecycle and traceability,
maintenance planning and utilization, electronic technical records,
QR/barcode workflows, staff attendance, fleet/flight tracking, and
AI-assisted search or analytics. These should be prioritized only when
customer demand, data governance, and operational requirements justify
them.

## 📚 Project Documentation

- [`docs/Notifications.md`](docs/Notifications.md) — Notification
  architecture, configuration, processing behavior, testing, and known
  limitations.
- [`docs/retrospectives/Sprint-11-Retrospective.md`](docs/retrospectives/Sprint-11-Retrospective.md)
  — Sprint 11 outcomes, lessons, and follow-up actions.
- `Airport_Operations_Platform_Updated_Roadmap_and_Commercial_Vision_v4.docx`
  — Updated strategic direction, architecture decisions, and proposed
  sprint plan (maintained separately from this repository unless added).

## 🔒 Security, IP & Project Guardrails

- Rotate exposed JWT signing material and any credentials that may have
  been exposed or reused.
- Remove hardcoded development credentials and restrict development
  seeding to the Development environment.
- Review Git history, secret handling, `.gitignore`, repository
  visibility, and the intended licence.
- Clarify IP ownership and contributor agreements before bringing in a
  co-founder, contractor, or external developer.
- Do not assume regulatory compliance or certification.
- Do not treat ADS-B as authoritative maintenance hours or cycles
  without validation against approved operational records.
- Avoid premature microservices, AI features, and large enterprise
  feature sets before customer demand is established.

## 👨‍💻 Author

**Jules Douglas**  
Backend Software Developer

This repository documents an ongoing journey building enterprise-style
backend applications while applying Clean Architecture, CQRS,
Domain-Driven Design, API security, asynchronous messaging, testing, and
incremental sprint-based development.

## ⚠️ Disclaimer

This is an evolving software development project. It is not represented
as Transport Canada-approved, certified, or suitable for making
independent airworthiness or return-to-service decisions. Regulatory,
privacy, security, and operational requirements must be assessed with
qualified professionals before any production use.
