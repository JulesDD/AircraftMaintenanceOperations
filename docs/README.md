# ✈️ Airport Maintenance Operations API

A modern **ASP.NET Core (.NET 8)** backend application built using **Clean Architecture**, **CQRS**, and **Domain-Driven Design (DDD)** principles to manage aircraft, pilots, and maintenance operations.

This project is being developed incrementally using agile sprints to simulate real-world enterprise software development practices.

---

## 🚀 Project Status

| Module | Status |
|---------|:------:|
| Pilot Management | ✅ Complete |
| Aircraft Management | ✅ Complete |
| Maintenance Requests | ✅ Complete |
| Work Orders | ✅ Complete|
| Technician Management | 🚧 In Progress |
| Inventory Management | 🚧 In Progress |
| Maintenance Scheduling | 📅 Planned |
| Authentication & Authorization | 🚧 In Progress |
| Kafka Messaging | 📅 Planned |
| Unit Testing | 📅 Planned |
| Docker Deployment | 📅 Planned |

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
- Kafka

---

# 🧩 Design Patterns

This project demonstrates several enterprise development patterns:

- Clean Architecture
- CQRS (Command Query Responsibility Segregation)
- Domain-Driven Design (DDD)
- Repository Pattern
- Dependency Injection
- Factory Methods
- Domain Result Pattern
- Validation Pipeline Behaviour

---

# ✅ Validation Strategy

Validation is intentionally split into two layers.

### FluentValidation

Used for:

- Required fields
- Length validation
- Date validation
- Input formatting

### Domain Layer

Responsible for business rules such as:

- Aircraft assignment rules
- Flight hour validation
- Operational status changes
- Aggregate behaviour

This separation keeps business logic inside the domain where it belongs.

---

# 📖 Learning Objectives

This project has strengthened my understanding of:

- Clean Architecture
- CQRS
- Domain-Driven Design
- MediatR
- Entity Framework Core
- RESTful API Design
- Minimal APIs
- FluentValidation
- Separation of Concerns

---

# 🚀 Upcoming Work

Planned features include:

- Maintenance Requests
- Maintenance Scheduling
- Maintenance History
- JWT Authentication
- Role-Based Authorization
- Unit Testing
- Integration Testing
- Docker Improvements
- Azure Deployment
- CI/CD Pipeline
- Kafka Messaging

---

# 👨‍💻 Author

**Jules Douglas**

Backend Software Developer

This repository documents my journey developing enterprise-style backend applications while applying Clean Architecture, CQRS, and Domain-Driven Design principles through sprint-based development. Ideas and retrospectives are found in the .md files.
