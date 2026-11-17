Aircraft Maintenance Operations

Project Status

Current Sprint: Sprint 7 — Complete

Aircraft Maintenance Operations is a maintenance management system built with C#, .NET, ASP.NET Core, Entity Framework Core, MediatR, FluentValidation, Carter/Minimal APIs, and SQL Server.

The current application focuses on:

Maintenance Requests

Technicians

Work Orders

Aircraft

Personnel and employment status

Sprint 7 Milestone

Sprint 7 completed the core API workflows for Maintenance Requests, Technicians, and Work Orders.

Completed

Maintenance Request commands and queries

Maintenance Request API endpoints

Maintenance Request lifecycle operations

Technician commands and queries

Technician validation and handlers

Technician API endpoints

Work Order commands and queries

Work Order creation

Work Order API endpoints

Maintenance Request number generation

Work Order number generation

Shared EmploymentStatus model

End-to-end API endpoint testing

Domain Model

User
├── EmploymentStatus
├── EmployeeNumber
├── FirstName
├── LastName
├── Email
└── PhoneNumber

Pilot
├── Rank
└── LicenseNumber

Technician
├── CertificationLevel
└── YearsOfExperience

Employment status belongs to User because it describes the employee rather than the employee's profession.

Maintenance Request Lifecycle

Open
  ↓
InProgress
  ↓
AwaitingParts
  ↓
InProgress
  ↓
Completed
  ↓
Closed
  ↓
Archived

Work Order

Work Order status remains a separate concept from Maintenance Request status because the two represent different business processes.

API Workflow

Create Maintenance Request
          ↓
       Open
          ↓
POST /api/maintenance/{id}/start
          ↓
      InProgress
          ↓
Create Work Order
          ↓
   Work Order Created

A Work Order can only be created when the related Maintenance Request is in the required lifecycle state.

Architecture

src/
├── AircraftMaintenanceOperations.Domain
├── AircraftMaintenanceOperations.Application
├── AircraftMaintenanceOperations.Infrastructure
└── AircraftMaintenanceOperations.API

Domain

Entities, enums, domain behavior, and interfaces.

Application

Commands, queries, handlers, validators, DTOs/results, and MediatR behaviors.

Infrastructure

EF Core, SQL Server persistence, configurations, and infrastructure implementations.

API

Carter endpoint modules, HTTP routing, Swagger/OpenAPI, and request/response handling.

Technology Stack

C#

.NET

ASP.NET Core

Carter / Minimal APIs

MediatR

FluentValidation

Entity Framework Core

SQL Server

Docker

Git/GitHub

Testing Status

Sprint 7 included manual end-to-end API testing.

Testing uncovered and resolved issues involving:

JSON enum values

GUID request values

Maintenance Request lifecycle state

TPH inheritance

Shared employment status

EF Core database mappings

Required Work Order notes

Next: Sprint 8

Sprint 8 will focus on completing and hardening the Work Order workflow.

Planned areas:

Work Order PATCH/update

Work Order lifecycle validation

Automated tests

Number-generator concurrency hardening

Database/migration cleanup

Notes/documentation improvements

API and domain cleanup

Development Philosophy

The project is being developed incrementally. The goal is not simply to make endpoints compile; business rules should live in the domain, application workflows should be explicit, and API testing should expose missing use cases.

Sprint 7 reinforced that approach by uncovering missing lifecycle operations and inconsistencies between the application model and database model.