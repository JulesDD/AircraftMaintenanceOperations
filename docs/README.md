# ✈️ Aircraft Maintenance Operations API

A modern **ASP.NET Core (.NET 8)** backend application built using **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles to manage aircraft, pilots, technicians, maintenance requests, work orders, and inventory operations.

The project is developed incrementally using agile sprints to simulate real-world enterprise backend development practices.

---

## 🚀 Project Status

| Module | Status |
|---------|:------:|
| Pilot Management | ✅ Complete |
| Aircraft Management | ✅ Complete |
| Maintenance Requests | ✅ Complete |
| Work Orders | ✅ Complete |
| Technician Management | ✅ Complete |
| Inventory Management | ✅ Complete |
| Authentication & Authorization | ✅ Complete |
| Employee / User / Role / Profile Foundation | ✅ Complete |
| Inventory Hardening | ✅ Complete |
| Sprint 9 Technical Debt Cleanup | ✅ Complete |
| Kafka Messaging | 📅 Planned |
| Notifications | 📅 Planned |
| Reporting and Analytics | 📅 Planned |
| Security & API Hardening | 📅 Planned |
| React Frontend | 📅 Planned |
| Docker Deployment | 📅 Planned |

**Current Sprint: Sprint 9 — Complete**

---

# 🛠 Technologies

- ASP.NET Core (.NET 8)
- C#
- Entity Framework Core
- SQL Server
- MediatR
- Carter
- FluentValidation
- Mapster
- Swagger / OpenAPI
- Minimal APIs
- Docker
- Kafka (planned)

---

# 🏗 Architecture

The solution follows a layered Clean Architecture structure:

```text
src/
├── AircraftMaintenanceOperations.Domain
├── AircraftMaintenanceOperations.Application
├── AircraftMaintenanceOperations.Infrastructure
└── AircraftMaintenanceOperations.API
```

### Domain

Contains entities, enums, domain behavior, and domain-facing abstractions.

The current domain foundation includes:

```text
User
├── EmployeeNumber
├── FirstName
├── LastName
├── Email
├── PhoneNumber
├── Role
└── EmploymentStatus

Pilot : User
├── Rank
└── LicenseNumber

Technician : User
├── CertificationLevel
└── YearsOfExperience
```

`User` owns employment lifecycle information. Specialized Pilot and Technician profiles contain profession-specific qualification data.

### Application

Contains commands, queries, handlers, validators, DTOs/results, and MediatR pipeline behaviors.

### Infrastructure

Contains EF Core, SQL Server persistence, entity configurations, Identity integration, and infrastructure service implementations.

### API

Contains Carter endpoint modules, HTTP routing, authorization, Swagger/OpenAPI metadata, and request/response handling.

---

# 🔐 Authentication & Authorization

ASP.NET Identity is used for authentication infrastructure.

`ApplicationUser` maps the authenticated Identity account to the domain user through `DomainUserId`.

Authorization is role-based, with resource-level checks where required.

Current roles:

- Admin
- MaintenanceSupervisor
- Technician
- InventoryClerk
- Pilot
- OperationsManager

Examples of authorization responsibilities include:

- Maintenance Request access
- Maintenance history/task access
- Work Order access and technician assignment checks
- Inventory Management
- Administrative role changes

JWT role claims represent the user's current authorization role at token creation time.

---

# 📦 Inventory

Inventory Management includes:

- Inventory Part creation
- Receive Inventory
- Issue Inventory
- Adjust Inventory
- Inventory lookup
- Inventory transaction history
- Low-stock / restock queries

Stock-changing operations create audit transactions containing the relevant inventory part, Work Order when applicable, performing domain user, transaction type, quantity information, and reason/context.

Reserve Inventory is intentionally deferred until a concrete business requirement exists for allocating and reserving stock.

---

# ✅ Validation & Business Rules

Validation is split between application-level input validation and domain behavior.

### FluentValidation

Used for input concerns such as:

- Required fields
- String lengths
- Numeric ranges
- Dates
- Email formatting

### Domain Layer

Responsible for business rules and state transitions such as:

- Employment lifecycle changes
- Role changes
- Aircraft/work-order behavior
- Inventory quantity changes
- Maintenance lifecycle behavior

This keeps business behavior out of API endpoints where possible.

---

# 🧪 Testing

Sprint 9 included integration and regression verification covering:

- Employee/User creation and update
- Role changes and promotions
- Specialized profile persistence
- Authorization after role changes
- Authenticated Inventory operations
- Inventory audit behavior
- Inventory / Work Order interaction
- Sprint 8 regression coverage

Builds and tests remained green through the Sprint 9 hardening and technical-debt cleanup work.

---

# 🧩 Design Patterns

The project demonstrates:

- Clean Architecture
- CQRS
- Domain-Driven Design
- Dependency Injection
- Factory Methods
- Domain Result Pattern
- Validation Pipeline Behavior
- Entity Framework Core
- TPH inheritance for specialized User profiles

---

# 🚀 Roadmap

Completed milestones include the core Aircraft, Pilot, Technician, Maintenance Request, Work Order, Inventory, authentication, authorization, and Sprint 9 domain-foundation work.

Planned future areas include:

- Kafka messaging
- Notifications
- Reporting and analytics
- Security and API hardening
- React frontend
- Docker deployment improvements
- Azure deployment
- CI/CD

See `docs/Roadmap.md` for the current roadmap.

---

# 👨‍💻 Author

**Jules Douglas**

Backend Software Developer

This repository documents an ongoing journey building enterprise-style backend applications while applying Clean Architecture, CQRS, Domain-Driven Design, API security, testing, and incremental sprint-based development.
