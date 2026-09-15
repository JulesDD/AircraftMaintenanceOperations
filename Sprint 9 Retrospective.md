# Sprint 9 Retrospective

## Sprint Objective

Establish the Employee/User/Role/Profile foundation identified during Sprint 8 and harden the completed Inventory domain without introducing unnecessary new inventory functionality.

## Outcome

Sprint 9 objectives were completed.

The sprint established a clearer separation between authentication infrastructure and the domain User model, introduced specialized Pilot and Technician profiles, refined role-based authorization, hardened Inventory behavior and auditability, expanded integration/regression coverage, and completed a targeted technical-debt cleanup.

Reserve Inventory remains intentionally deferred because no concrete business requirement currently requires reservation semantics.

## What We Completed

### Employee / User Foundation

- Separated the domain User concept from ASP.NET Identity authentication infrastructure.
- Established the `ApplicationUser.DomainUserId` relationship between Identity and the domain user.
- Moved employment lifecycle behavior to the User model.
- Preserved domain employee information independently of the current authorization role.

### Role Model

Current roles:

- Admin
- MaintenanceSupervisor
- Technician
- InventoryClerk
- Pilot
- OperationsManager

Role changes and promotions are handled through the domain User model and synchronized with the Identity role assignment.

### Specialized Profiles

Technician and Pilot are specialized User profiles with their own qualification data.

Technician-specific information includes:

- Certification level
- Years of experience

Pilot-specific information includes:

- Rank
- License number

The domain model uses TPH inheritance so specialized profile information remains part of the employee's domain history rather than being replaced when an authorization role changes.

### Authorization Refinement

Authorization was reviewed and hardened across:

- Maintenance Requests
- Maintenance Tasks / History
- Work Orders
- Aircraft
- Pilots
- Technicians
- Inventory
- Administrative role changes

Resource-level ownership checks were added where role membership alone was not sufficient, including Pilot access to requested maintenance records and Technician access to assigned Work Orders.

### Inventory Hardening

Inventory functionality from Sprint 8 was reviewed and hardened rather than expanded with unnecessary operations.

Verified behavior includes:

- Receive Inventory
- Issue Inventory
- Adjust Inventory
- Inventory transaction history
- Low-stock / restock queries
- Authenticated performing-user audit information
- Quantity-before / quantity-after audit values
- Work Order association for usage transactions
- Prevention of invalid stock states

### Integration and Regression Testing

The following scenarios were completed and verified:

1. Employee/User creation and update
2. Role change
3. Promotion
4. Specialized profile persistence
5. Authorization after role change
6. Authenticated Inventory operation
7. Inventory audit behavior
8. Inventory / Work Order interaction
9. Full Sprint 8 regression

## Technical Debt Completed

Sprint 9.8 was intentionally limited to targeted cleanup.

### Completed

- Removed synchronous database access from `CurrentUserService` where domain-user resolution is required.
- Added global exception/error mapping.
- Reduced endpoint command/request duplication by binding request bodies directly to application commands where appropriate.
- Reviewed DTO consistency.
- Reviewed nullable result contracts.
- Added `AsNoTracking()` to appropriate read-only EF Core queries.
- Removed confirmed unused interfaces, abstractions, and code/usings.
- Cleaned API descriptions and naming inconsistencies.

### Deliberately Not Expanded

Update result wrappers that only communicate success/failure were reviewed but not broadly redesigned. They are consistent enough for the current API contract and are better treated as future technical debt than as a Sprint 9 scope expansion.

## What Went Well

### Incremental Refactoring

Each technical-debt item was audited before code was changed. Changes were followed by a build and test cycle before moving to the next item.

### Authorization Became More Explicit

The sprint moved beyond simple role checks by introducing resource-level ownership checks where appropriate.

### Domain and Identity Responsibilities Are Clearer

The domain User now represents the employee and employment lifecycle, while ASP.NET Identity remains responsible for authentication and authorization infrastructure.

### Inventory Auditability Was Preserved

Stock-changing operations continue to create transaction records with the context needed to understand what happened and why.

### Scope Discipline

Reserve Inventory was not implemented merely because it appeared on the roadmap. The feature was deferred until a real requirement exists for allocation and reservation semantics.

### Testing Discipline

Builds and tests remained green after the Sprint 9 technical-debt and cleanup work.

## What Could Be Improved

- Some update command result contracts remain simple success/failure wrappers.
- Direct DTO projection can be considered later for additional query optimization where profiling or scale justifies it.
- API documentation can continue to evolve as new modules are added.
- The roadmap and older sprint documentation required synchronization with the project's actual current state.

These items do not block Sprint 9 completion.

## Key Architectural Decisions

### Domain User vs Identity User

The domain `User` owns employee identity and employment information. `ApplicationUser` owns authentication infrastructure and maps to the domain user through `DomainUserId`.

### Role vs Specialized Profile

Authorization role and professional qualification are separate concepts. A current role determines access, while Pilot and Technician profiles retain profession-specific information.

### Resource-Level Authorization

Role authorization is supplemented with ownership or assignment checks where a resource should only be visible or actionable by the appropriate user.

### Reserve Inventory Deferred

Reservation is not being implemented without a concrete business requirement. A future reservation design would need to distinguish physical stock from reserved/available stock and associate reservations with Work Orders.

## Sprint 9 Definition of Done

- [x] Employee/User foundation is clearly separated from authentication infrastructure.
- [x] Current authorization role is distinct from specialized profile/qualification data.
- [x] Role changes/promotions preserve employee and profile information.
- [x] Technician and Pilot profiles have a clear domain model.
- [x] Authorization policies reflect the finalized model.
- [x] Inventory validation and audit behavior are hardened.
- [x] Integration and regression tests pass.
- [x] Targeted technical debt is addressed.
- [x] Documentation reflects the resulting domain model.
- [x] Reserve Inventory remains deferred without a business requirement.

## Sprint 9 Closeout

**Sprint 9 — Complete**

The project is ready to move from domain foundation and hardening work into the next planned development stage.
