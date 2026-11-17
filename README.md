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

| Module / Milestone | Status |
|---|:---:|
| Aircraft, Pilot and Technician Management | ✅ Complete |
| Work Orders and Maintenance Requests | ✅ Complete |
| Inventory Management Foundation | ✅ Complete |
| Authentication and Authorization Foundation | ✅ Complete |
| Kafka Messaging and Notification Worker | ✅ Implemented |
| Sprint 13 — Security Remediation & API Hardening | ✅ Complete |
| Sprint 14 — Inventory Management, Material Requests & Notifications | 🔄 Next |
| Reporting & Analytics | 📅 Planned — Sprint 15 |
| Workforce & Labour Management | 📅 Planned — Sprint 16 |
| React Frontend & Platform Integration | 📅 Planned — Sprint 20 |

**Current milestone: Sprint 14 — Inventory Management, Material Requests & Notifications.**

The next milestone expands inventory workflows with Work Order material requests, Inventory Clerk approval, multi-location stock availability, reservations, dispatch and receipt tracking, shortage handling, and notifications.

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

See [`docs/notifications/Notifications.md`](docs/notifications/Notifications.md) for configuration,
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

The current development roadmap is organized around the following milestones:

| Sprint / Gate | Focus |
|---|---|
| 13 | Security Remediation & API Hardening — Complete |
| 14 | Inventory Management, Material Requests & Notifications |
| 15 | Reporting & Analytics |
| 16 | Workforce & Labour Management |
| 17 | Maintenance Request Lifecycle & Pilot Communication |
| 18 | Organization & Tenant Foundation |
| 19 | Operator-Controlled Aircraft Records & Utilization |
| Pre-Sprint 20 | Security & Authorization Readiness Gate |
| 20 | React Frontend & Platform Integration |
| 21 | Reliability, DevOps & Cloud |

### Pre-Sprint 20 security readiness gate

Before beginning the React frontend, the backend will undergo a focused authorization and security review. This includes role and resource access validation, technician assignment boundaries, tenant isolation, positive and negative API authorization tests, and frontend authentication integration readiness.

Backend authorization remains the security authority. The frontend must not be relied upon to enforce access control.

The gate must be satisfied before Sprint 20 begins.


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

Project documentation is organized under the `docs/` directory:

- [Architecture Decisions](docs/architecture/ArchitectureDecisions.md)
- [Notification Architecture and Configuration](docs/notifications/Notifications.md)
- [Security Configuration](docs/security/SECURITY_CONFIGURATION.md)
- [Historical Roadmap](docs/roadmap/Roadmap.md)
- [Sprint Plans and Retrospectives](docs/sprints/)

See [docs/README.md](docs/README.md) for the documentation index.

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
