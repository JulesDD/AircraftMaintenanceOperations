Sprint 9 --- Domain Foundation & Inventory Hardening

Objective

Establish the Employee/User/Role/Profile foundation identified during
Sprint 8 and harden the completed Inventory domain.

The original roadmap called Sprint 9 Domain Foundation & Inventory
Completion, with emphasis on Employee/User separation, specialized
Technician/Pilot profiles, role changes/promotions, authorization
refinement, deferred inventory work, stronger validation/integration
tests, and documentation. fileciteturn53file0L45-L57

Because Sprint 8 inventory work is now complete, Sprint 9 is updated to
Domain Foundation & Inventory Hardening.

9.1 Employee / User Foundation

Review the current ApplicationUser ↔ domain-user relationship.

Define the domain Employee/User concept separately from
authentication infrastructure.

Establish clear ownership of identity and employment
information.

Preserve historical employee information when roles change.

9.2 Role Model

Current roles: - Admin - MaintenanceSupervisor - Technician -
InventoryClerk - Pilot - OperationsManager

Tasks: - [ ] Separate current authorization role from specialized
profile/qualification data. - [ ] Define how current roles are
represented. - [ ] Define role changes and promotions. - [ ]
Preserve historical profile information after role changes. - [ ]
Define rules for promotion/demotion/role transitions.

9.3 Specialized Profiles

Technician

Identify Technician-specific domain properties.

Separate technician qualifications from authorization role.

Preserve technician history after role changes.

Pilot

Identify Pilot-specific domain properties.

Separate pilot qualifications from authorization role.

Preserve pilot history after role changes.

Profile Lifecycle

Define when specialized profiles are created.

Define what happens when an employee changes roles.

Ensure profile history is retained where required.

Avoid coupling profile existence directly to current
authorization role.

9.4 Authorization Refinement

Review authorization policies against the finalized domain
model.

Keep current-role authorization separate from
qualifications/profile data.

Review Inventory Management policy.

Review Maintenance Task authorization.

Review Work Order access and assignment authorization.

Review administrative actions.

Test authorization after role changes.

9.5 Inventory Hardening

Sprint 8 inventory features are complete. Sprint 9 should harden them
rather than introduce unnecessary new operations.

Validation

Review inventory validators.

Review quantity boundaries.

Review zero/negative handling.

Review adjustment rules.

Review Receive and Issue business rules.

Confirm stock cannot become negative.

Audit Integrity

Verify every stock-changing operation creates the correct
transaction.

Verify QuantityBefore and QuantityAfter.

Verify PerformedByUserId comes from authenticated context.

Verify Usage transactions have the correct Work Order.

Verify adjustment reasons.

Query Integrity

Review Get All Inventory.

Review Get Inventory By ID.

Review transaction-history filtering and ordering.

Review Low Stock / Needs Restock behavior.

Add integration coverage for representative scenarios.

9.6 Reserve Inventory --- Decision Gate

Reserve Inventory remains deferred.

Do not implement it unless a concrete business requirement emerges
for: - allocating parts to Work Orders before issue, - preventing
competing Work Orders from using allocated stock, - tracking reserved
quantity, - releasing reservations, - distinguishing available stock
from physical stock.

If required, design the reservation model before implementing the
operation.

9.7 Integration Testing

Employee/User creation and update tests.

Role-change tests.

Promotion scenario tests.

Specialized profile persistence tests.

Authorization-after-role-change tests.

Inventory authenticated-operation tests.

Inventory audit tests.

Inventory/Work Order interaction tests.

Full Sprint 8 regression tests.

9.8 Targeted Technical Debt

Only address technical debt that supports Sprint 9:

Review synchronous database access in CurrentUserService.

Review global exception/error mapping.

Review endpoint command/request duplication.

Review DTO consistency.

Review nullable result contracts.

Review EF Core query performance and AsNoTracking() usage.

Remove unused code/usings.

Correct remaining API descriptions/naming inconsistencies.

Avoid broad refactors that do not support the sprint objective.

9.9 Documentation

Document Employee/User/Role/Profile architecture.

Document promotion and role-transition scenarios.

Document current authorization policies.

Document inventory audit behavior.

Update README architecture sections.

Update API feature documentation.

Create Sprint 9 retrospective at sprint close.

Sprint 9 Completion Criteria

Sprint 9 is complete when:

Employee/User foundation is clearly separated from
authentication infrastructure.

Current authorization role is distinct from specialized
profile/qualification data.

Role changes/promotions preserve historical profile information.

Technician and Pilot profiles have a clear domain model where
required.

Authorization policies reflect the finalized model.

Inventory validation and audit behavior are hardened.

Integration/regression tests pass.

Documentation reflects the resulting domain model.

Reservation has not been introduced without a real business
requirement.