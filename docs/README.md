# ✈️ Aircraft Maintenance Operations Platform

A backend-focused aircraft maintenance operations platform built with
ASP.NET Core, .NET, Clean Architecture, CQRS, Domain-Driven Design
(DDD), Entity Framework Core, SQL Server, and Apache Kafka. The platform
manages aircraft, pilots, technicians, maintenance requests, work
orders, and inventory, with asynchronous email notifications handled by
an independent worker service.

The project is developed incrementally through agile sprints to
demonstrate enterprise-style backend engineering, testing, and
architectural decision-making.

## 🚀 Project Status

  Module / Capability                                              Status
  ------------------------------------------------ --------------------------------------
  Aircraft Management                                           ✅ Complete
  Pilot Management                                              ✅ Complete
  Technician Management                                         ✅ Complete
  Maintenance Requests                                          ✅ Complete
  Work Orders                                                   ✅ Complete
  Inventory Management and Audit History                        ✅ Complete
  Authentication and Role-Based Authorization                   ✅ Complete
  Employee / User / Role / Profile Foundation                   ✅ Complete
  Inventory Hardening and Technical-Debt Cleanup                ✅ Complete
  Kafka Event Messaging                                        ✅ Implemented
  Notification Worker --- Email                     ✅ Implemented and end-to-end tested
  SMTP Failure / Recovery Verification                           ✅ Tested
  Work Order Lifecycle & Control Improvements                     ⏭️ Next
  Reporting and Analytics                                        📅 Planned
  Security and API Hardening                                     📅 Planned
  React Frontend                                                 📅 Planned
  Deployment / CI/CD / Cloud                                     📅 Planned

**Current milestone: Sprint 11 --- Notification Service implemented and
verified; documentation and retrospective closeout.**

## 🛠️ Technology Stack

-   C# and .NET
-   ASP.NET Core Web API
-   Entity Framework Core and SQL Server
-   MediatR and CQRS
-   Carter and Minimal APIs
-   FluentValidation and Mapster
-   Swagger / OpenAPI
-   ASP.NET Identity and JWT authentication
-   Apache Kafka
-   MailKit / SMTP
-   Docker (development and deployment work is evolving)

Check the target frameworks in the solution files for the exact
framework version used by each project.

## 🏗️ Architecture

The API follows Clean Architecture, with a separate Notification Worker
for asynchronous event processing.

``` text
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
user through `DomainUserId`. Authorization is role-based, with
resource-level checks where required.

Current roles:

-   `Admin`
-   `MaintenanceSupervisor`
-   `Technician`
-   `InventoryClerk`
-   `Pilot`
-   `OperationsManager`

JWT role claims represent the user's authorization role at token
creation time.

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
using the consumer group `aircraft-maintenance-notification-worker`.

Implemented workflows:

-   **Technician assigned:** sends an email to the assigned technician.
-   **Work Order completed:** sends an email to the assigned technician
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

See [`docs/Notifications.md`](docs/Notifications.md) for configuration,
processing behavior, testing, and limitations.

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
closeout.

## 🧩 Design Patterns & Practices

-   Clean Architecture
-   CQRS and MediatR
-   Domain-Driven Design
-   Dependency Injection
-   Factory Methods
-   Domain Result Pattern
-   Validation Pipeline Behavior
-   Entity Framework Core
-   Table-per-hierarchy (TPH) inheritance for specialized user profiles
-   Event-driven integration through Kafka
-   Provider abstraction for email delivery

## 🗺️ Roadmap

The next planned milestone is **Sprint 12: Work Order Lifecycle &
Control**. Planned areas include explicit technician check-in/check-out,
a clearer inspection and supervisor-completion workflow, and controlled
hold/resume behavior.

Later roadmap areas include reporting and analytics, security and API
hardening, a React frontend, deployment improvements, Azure deployment,
and CI/CD. See `docs/Roadmap.md` and the revised sprint roadmap for the
current plan.

## 📚 Project Documentation

-   [`docs/Notifications.md`](docs/Notifications.md) --- Notification
    architecture, configuration, processing behavior, testing, and known
    limitations.
-   [`docs/retrospectives/Sprint-11-Retrospective.md`](docs/retrospectives/Sprint-11-Retrospective.md)
    --- Sprint 11 outcomes, lessons, and follow-up actions.

## 👨‍💻 Author

**Jules Douglas**\
Backend Software Developer

This repository documents an ongoing journey building enterprise-style
backend applications while applying Clean Architecture, CQRS,
Domain-Driven Design, API security, asynchronous messaging, testing, and
incremental sprint-based development.
