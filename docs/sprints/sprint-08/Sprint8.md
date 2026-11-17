# Sprint 8 — Entity & DTO Implementation

## Sprint Goal

Complete the remaining domain model for the application by creating all required entities and DTOs for:

* Inventory
* Maintenance Scheduling
* Maintenance records
* Supervisors
* Administration
* Users / roles
* Locations
* Equipment / assets
* Supporting relationships and status models

**Primary objective:** Have the complete entity + DTO layer modeled before implementing the next round of business logic and endpoints.

---

# Phase 1 — Sprint 7 Carry-Forward

## Review Existing Work

* [ ] Review all entities currently implemented
* [ ] Review all DTOs currently implemented
* [ ] Review all endpoints completed in Sprint 7
* [ ] Confirm every Sprint 7 endpoint has been tested
* [ ] Identify any entities that were created as temporary placeholders
* [ ] Identify duplicate models or DTOs
* [ ] Identify properties that need to be renamed or standardized
* [ ] Review existing entity relationships
* [ ] Review existing foreign keys
* [ ] Review existing enums/status values
* [ ] Review existing validation rules

## Clean-Up

* [ ] Remove obsolete properties
* [ ] Remove obsolete DTOs
* [ ] Standardize naming
* [ ] Standardize ID conventions
* [ ] Standardize timestamps
* [ ] Standardize nullable vs required properties
* [ ] Standardize navigation properties
* [ ] Confirm entities follow the project's existing architecture

---

# Phase 2 — Core Asset / Equipment Entities

These entities should provide the foundation for inventory and maintenance.

## Equipment / Asset

* [ ] Create `Equipment` entity
* [ ] Add equipment identifier
* [ ] Add name
* [ ] Add description
* [ ] Add equipment type
* [ ] Add manufacturer
* [ ] Add model
* [ ] Add serial number
* [ ] Add purchase information
* [ ] Add installation date
* [ ] Add current location
* [ ] Add current status
* [ ] Add active/inactive state
* [ ] Add maintenance relationship
* [ ] Add inventory relationship where appropriate

## Equipment Type

* [ ] Create `EquipmentType` entity
* [ ] Add name
* [ ] Add description
* [ ] Add category
* [ ] Add active status

## Location

* [ ] Create/complete `Location` entity
* [ ] Add name
* [ ] Add description
* [ ] Add location type
* [ ] Add address/details as required
* [ ] Add active status
* [ ] Establish equipment relationship
* [ ] Establish inventory relationship

---

# Phase 3 — Inventory Entities

Sprint 8 should finish the inventory domain rather than leaving it partially modeled.

## Inventory Item

* [ ] Create/complete `InventoryItem`
* [ ] Add item number/SKU
* [ ] Add name
* [ ] Add description
* [ ] Add category
* [ ] Add quantity
* [ ] Add minimum quantity
* [ ] Add maximum quantity
* [ ] Add unit of measure
* [ ] Add storage location
* [ ] Add active status
* [ ] Add reorder information

## Inventory Category

* [ ] Create `InventoryCategory`
* [ ] Add name
* [ ] Add description
* [ ] Add active status

## Inventory Transaction

* [ ] Create `InventoryTransaction`
* [ ] Add transaction type
* [ ] Add quantity
* [ ] Add date/time
* [ ] Add inventory item relationship
* [ ] Add user relationship
* [ ] Add reason/reference
* [ ] Add related maintenance work order where applicable

## Inventory Adjustment

* [ ] Create `InventoryAdjustment`
* [ ] Add adjustment type
* [ ] Add quantity before adjustment
* [ ] Add quantity after adjustment
* [ ] Add reason
* [ ] Add user
* [ ] Add timestamp

## Supplier / Vendor

* [ ] Create/complete `Supplier`
* [ ] Add supplier name
* [ ] Add contact information
* [ ] Add address
* [ ] Add active status
* [ ] Establish inventory relationship

---

# Phase 4 — Maintenance Scheduling

This is the major new Sprint 8 domain.

## Maintenance Schedule

* [ ] Create `MaintenanceSchedule`
* [ ] Add schedule name
* [ ] Add description
* [ ] Add equipment relationship
* [ ] Add maintenance type
* [ ] Add frequency
* [ ] Add frequency unit
* [ ] Add start date
* [ ] Add next scheduled date
* [ ] Add last completed date
* [ ] Add estimated duration
* [ ] Add priority
* [ ] Add active status
* [ ] Add assigned supervisor
* [ ] Add required parts/inventory relationship

## Maintenance Task

* [ ] Create `MaintenanceTask`
* [ ] Add task name
* [ ] Add description
* [ ] Add instructions
* [ ] Add estimated duration
* [ ] Add priority
* [ ] Add required skill/type
* [ ] Add completion requirements
* [ ] Establish schedule relationship

## Maintenance Type

* [ ] Create `MaintenanceType`
* [ ] Add name
* [ ] Add description
* [ ] Add type/category
* [ ] Add active status

Examples:

* Preventative
* Corrective
* Emergency
* Inspection
* Scheduled
* Unscheduled

## Maintenance Work Order

* [ ] Create `MaintenanceWorkOrder`
* [ ] Add work order number
* [ ] Add equipment
* [ ] Add maintenance schedule
* [ ] Add maintenance type
* [ ] Add description
* [ ] Add priority
* [ ] Add status
* [ ] Add scheduled date
* [ ] Add start date
* [ ] Add completion date
* [ ] Add estimated duration
* [ ] Add actual duration
* [ ] Add assigned supervisor
* [ ] Add assigned technician/employee
* [ ] Add notes
* [ ] Add completion notes

## Maintenance Status

* [ ] Create maintenance status enum
* [ ] Define statuses
* [ ] Ensure status transitions are logically supported

Possible states:

`Scheduled → Assigned → InProgress → Completed`

Additional states:

`Cancelled`

`OnHold`

`Overdue`

---

# Phase 5 — Maintenance Parts / Inventory Integration

Maintenance and inventory should connect cleanly.

## Maintenance Part

* [ ] Create `MaintenancePart`
* [ ] Link maintenance work order
* [ ] Link inventory item
* [ ] Add required quantity
* [ ] Add used quantity
* [ ] Add returned quantity
* [ ] Add part status

## Maintenance Material Usage

* [ ] Create `MaintenanceMaterialUsage`
* [ ] Add inventory item
* [ ] Add work order
* [ ] Add quantity used
* [ ] Add date/time
* [ ] Add employee/user
* [ ] Add inventory transaction relationship

## Validation

* [ ] Prevent maintenance from consuming unavailable inventory
* [ ] Validate required parts
* [ ] Validate quantities
* [ ] Determine whether unused parts can be returned
* [ ] Determine how inventory transactions are recorded

---

# Phase 6 — Supervisor Domain

Supervisors should be represented as an actual domain role rather than simply adding random supervisor properties throughout the system.

## Supervisor

* [ ] Create/complete `Supervisor`
* [ ] Link supervisor to user
* [ ] Add employee identifier
* [ ] Add department
* [ ] Add active status
* [ ] Add assigned location
* [ ] Add responsibilities where required

## Supervisor Relationships

* [ ] Supervisor → Maintenance Schedule
* [ ] Supervisor → Maintenance Work Order
* [ ] Supervisor → Equipment
* [ ] Supervisor → Employees/Technicians
* [ ] Supervisor → Inventory activity where appropriate

## Supervisor DTOs

* [ ] Create `SupervisorDto`
* [ ] Create `SupervisorSummaryDto`
* [ ] Create `CreateSupervisorDto`
* [ ] Create `UpdateSupervisorDto`
* [ ] Create `SupervisorDetailsDto`

---

# Phase 7 — Administration Domain

Administration should have its own domain representation.

## Administrator

* [ ] Create/complete `Administrator`
* [ ] Link administrator to user
* [ ] Add employee identifier
* [ ] Add department
* [ ] Add active status

## Role

* [ ] Create `Role` entity if not already provided by the authentication system
* [ ] Define administrator role
* [ ] Define supervisor role
* [ ] Define technician/employee role
* [ ] Define standard user role

## Permission

* [ ] Create `Permission` entity if required by the architecture
* [ ] Add permission name
* [ ] Add description
* [ ] Add permission category

## Role Permission

* [ ] Create `RolePermission`
* [ ] Link role
* [ ] Link permission

## Administration Relationships

* [ ] Administrator → Users
* [ ] Administrator → Inventory
* [ ] Administrator → Equipment
* [ ] Administrator → Maintenance schedules
* [ ] Administrator → Maintenance work orders
* [ ] Administrator → Locations
* [ ] Administrator → Reports/settings where applicable

---

# Phase 8 — User / Employee Entities

Before completing Admin/Supervisor DTOs, make sure the underlying user model is clean.

## User

* [ ] Review existing `User`
* [ ] Confirm user ID
* [ ] Confirm username/email
* [ ] Confirm first/last name
* [ ] Confirm active status
* [ ] Confirm role relationship
* [ ] Confirm created/updated timestamps

## Employee

* [ ] Create/complete `Employee`
* [ ] Add employee number
* [ ] Add user relationship
* [ ] Add department
* [ ] Add job title
* [ ] Add active status

## Technician

* [ ] Create `Technician` if required by the domain
* [ ] Link technician to employee
* [ ] Add skill information if required
* [ ] Add certification information if required
* [ ] Add active status

---

# Phase 9 — DTO Layer

Every new entity should have a deliberate DTO strategy.

## Standard DTOs

For major entities:

* [ ] `XDto`
* [ ] `XSummaryDto`
* [ ] `XDetailsDto`
* [ ] `CreateXDto`
* [ ] `UpdateXDto`

Do **not** automatically create unnecessary DTOs for every tiny lookup entity.

---

# Maintenance DTOs

* [ ] `MaintenanceScheduleDto`

* [ ] `MaintenanceScheduleSummaryDto`

* [ ] `MaintenanceScheduleDetailsDto`

* [ ] `CreateMaintenanceScheduleDto`

* [ ] `UpdateMaintenanceScheduleDto`

* [ ] `MaintenanceTaskDto`

* [ ] `CreateMaintenanceTaskDto`

* [ ] `UpdateMaintenanceTaskDto`

* [ ] `MaintenanceTypeDto`

* [ ] `CreateMaintenanceTypeDto`

* [ ] `UpdateMaintenanceTypeDto`

* [ ] `MaintenanceWorkOrderDto`

* [ ] `MaintenanceWorkOrderSummaryDto`

* [ ] `MaintenanceWorkOrderDetailsDto`

* [ ] `CreateMaintenanceWorkOrderDto`

* [ ] `UpdateMaintenanceWorkOrderDto`

* [ ] `MaintenancePartDto`

* [ ] `CreateMaintenancePartDto`

* [ ] `UpdateMaintenancePartDto`

* [ ] `MaintenanceMaterialUsageDto`

* [ ] `CreateMaintenanceMaterialUsageDto`

---

# Inventory DTOs

* [ ] `InventoryItemDto`

* [ ] `InventoryItemSummaryDto`

* [ ] `InventoryItemDetailsDto`

* [ ] `CreateInventoryItemDto`

* [ ] `UpdateInventoryItemDto`

* [ ] `InventoryCategoryDto`

* [ ] `CreateInventoryCategoryDto`

* [ ] `UpdateInventoryCategoryDto`

* [ ] `InventoryTransactionDto`

* [ ] `CreateInventoryTransactionDto`

* [ ] `InventoryAdjustmentDto`

* [ ] `CreateInventoryAdjustmentDto`

* [ ] `SupplierDto`

* [ ] `CreateSupplierDto`

* [ ] `UpdateSupplierDto`

---

# Equipment DTOs

* [ ] `EquipmentDto`

* [ ] `EquipmentSummaryDto`

* [ ] `EquipmentDetailsDto`

* [ ] `CreateEquipmentDto`

* [ ] `UpdateEquipmentDto`

* [ ] `EquipmentTypeDto`

* [ ] `CreateEquipmentTypeDto`

* [ ] `UpdateEquipmentTypeDto`

* [ ] `LocationDto`

* [ ] `CreateLocationDto`

* [ ] `UpdateLocationDto`

---

# Administration DTOs

* [ ] `AdministratorDto`

* [ ] `AdministratorSummaryDto`

* [ ] `CreateAdministratorDto`

* [ ] `UpdateAdministratorDto`

* [ ] `RoleDto`

* [ ] `CreateRoleDto`

* [ ] `UpdateRoleDto`

* [ ] `PermissionDto`

* [ ] `CreatePermissionDto`

* [ ] `UpdatePermissionDto`

* [ ] `RolePermissionDto`

---

# Supervisor / Employee DTOs

* [ ] `SupervisorDto`

* [ ] `SupervisorSummaryDto`

* [ ] `SupervisorDetailsDto`

* [ ] `CreateSupervisorDto`

* [ ] `UpdateSupervisorDto`

* [ ] `EmployeeDto`

* [ ] `EmployeeSummaryDto`

* [ ] `CreateEmployeeDto`

* [ ] `UpdateEmployeeDto`

* [ ] `TechnicianDto`

* [ ] `TechnicianSummaryDto`

* [ ] `CreateTechnicianDto`

* [ ] `UpdateTechnicianDto`

---

# Phase 10 — Relationships & Database Configuration

Once the entities exist:

* [ ] Configure primary keys
* [ ] Configure foreign keys
* [ ] Configure one-to-one relationships
* [ ] Configure one-to-many relationships
* [ ] Configure many-to-many relationships
* [ ] Configure delete behavior
* [ ] Configure required relationships
* [ ] Configure optional relationships
* [ ] Configure indexes
* [ ] Configure unique constraints
* [ ] Configure string lengths
* [ ] Configure decimal precision
* [ ] Configure timestamps
* [ ] Configure enums
* [ ] Configure default values

## Important Relationship Review

Verify these relationships explicitly:

* [ ] Equipment → EquipmentType
* [ ] Equipment → Location
* [ ] Equipment → MaintenanceSchedule
* [ ] MaintenanceSchedule → MaintenanceTask
* [ ] MaintenanceSchedule → MaintenanceWorkOrder
* [ ] MaintenanceWorkOrder → Equipment
* [ ] MaintenanceWorkOrder → Supervisor
* [ ] MaintenanceWorkOrder → Technician
* [ ] MaintenanceWorkOrder → MaintenancePart
* [ ] MaintenancePart → InventoryItem
* [ ] InventoryItem → InventoryCategory
* [ ] InventoryItem → Location
* [ ] InventoryTransaction → InventoryItem
* [ ] InventoryTransaction → User
* [ ] Supervisor → User/Employee
* [ ] Administrator → User/Employee
* [ ] Technician → User/Employee

---

# Phase 11 — Mapping

* [ ] Review existing AutoMapper/manual mapping strategy
* [ ] Create entity → DTO mappings
* [ ] Create DTO → entity mappings
* [ ] Create create/update mappings
* [ ] Handle nested relationships
* [ ] Prevent sensitive properties from being exposed
* [ ] Prevent navigation-property loops
* [ ] Verify summary DTOs don't load unnecessary data

---

# Phase 12 — Validation

For every Create/Update DTO:

* [ ] Required fields
* [ ] String lengths
* [ ] Numeric ranges
* [ ] Date validation
* [ ] Enum validation
* [ ] Foreign-key validation
* [ ] Duplicate detection where necessary
* [ ] Business-rule validation

Special attention:

* [ ] Maintenance schedule cannot have an invalid frequency
* [ ] Scheduled date cannot conflict with required rules
* [ ] Equipment must exist before scheduling maintenance
* [ ] Inventory item must exist before assigning a maintenance part
* [ ] Inventory quantities cannot become invalid
* [ ] Supervisor must be active before assignment
* [ ] Technician must be active before assignment

---

# Phase 13 — Database Migration

After the entity model is stable:

* [ ] Review generated migration
* [ ] Check every new table
* [ ] Check every foreign key
* [ ] Check indexes
* [ ] Check column lengths
* [ ] Check nullable columns
* [ ] Check cascade behavior
* [ ] Check enum storage
* [ ] Check default values
* [ ] Run migration against development database
* [ ] Verify database schema
* [ ] Verify no unintended schema changes
* [ ] Seed lookup/reference data where appropriate

---

# Phase 14 — DTO Compilation & Integration Check

* [ ] Build entire solution
* [ ] Resolve entity configuration errors
* [ ] Resolve mapping errors
* [ ] Resolve nullable-reference warnings
* [ ] Resolve circular reference issues
* [ ] Resolve EF Core relationship errors
* [ ] Verify migrations
* [ ] Verify application starts successfully

---

# Phase 15 — Sprint 8 Testing

Entity-level testing:

* [ ] Create each major entity
* [ ] Update each major entity
* [ ] Verify required fields
* [ ] Verify invalid data is rejected
* [ ] Verify relationships
* [ ] Verify foreign keys
* [ ] Verify DTO mapping
* [ ] Verify nested DTOs
* [ ] Verify database persistence

Maintenance-specific testing:

* [ ] Create maintenance schedule
* [ ] Assign equipment
* [ ] Assign supervisor
* [ ] Generate/associate work order
* [ ] Assign technician
* [ ] Associate required parts
* [ ] Consume inventory
* [ ] Complete maintenance
* [ ] Verify maintenance history
* [ ] Verify inventory transaction

---

# Sprint 8 Definition of Done

Sprint 8 is complete when:

* [ ] All required domain entities exist
* [ ] Inventory entities are complete
* [ ] Maintenance scheduling entities are complete
* [ ] Maintenance work-order entities are complete
* [ ] Admin entities are complete
* [ ] Supervisor entities are complete
* [ ] Employee/technician entities are complete
* [ ] Equipment/location entities are complete
* [ ] DTOs exist for all major entities
* [ ] Create/Update DTOs exist where required
* [ ] Entity relationships are configured
* [ ] DTO mappings are configured
* [ ] Validation is implemented
* [ ] EF Core migration succeeds
* [ ] Database schema is verified
* [ ] Solution builds cleanly
* [ ] Existing Sprint 7 functionality still works
* [ ] New entities can be persisted successfully
* [ ] No unnecessary entities/DTOs remain
* [ ] Sprint 8 retrospective is documented

---

# Recommended Sprint 8 Order

### Day/Stage 1 — Foundation

* [ ] Review Sprint 7
* [ ] Review current entities
* [ ] Review current DTOs
* [ ] Establish final domain model

### Stage 2 — Core Domain

* [ ] Equipment
* [ ] Equipment Type
* [ ] Location
* [ ] Employee
* [ ] User/Role relationships

### Stage 3 — Inventory

* [ ] Inventory Item
* [ ] Inventory Category
* [ ] Supplier
* [ ] Inventory Transaction
* [ ] Inventory Adjustment

### Stage 4 — Maintenance

* [ ] Maintenance Type
* [ ] Maintenance Schedule
* [ ] Maintenance Task
* [ ] Maintenance Work Order
* [ ] Maintenance Part
* [ ] Material Usage

### Stage 5 — People / Administration

* [ ] Supervisor
* [ ] Administrator
* [ ] Technician
* [ ] Role
* [ ] Permission

### Stage 6 — DTOs

* [ ] Create DTOs
* [ ] Update DTOs
* [ ] Read DTOs
* [ ] Summary DTOs
* [ ] Details DTOs

### Stage 7 — Integration

* [ ] Entity configurations
* [ ] Relationships
* [ ] Mappings
* [ ] Validation
* [ ] Migration

### Stage 8 — Verification

* [ ] Build
* [ ] Database test
* [ ] Entity tests
* [ ] Mapping tests
* [ ] Regression test Sprint 7 functionality

---

# Sprint 8 Deliverable

At the end of Sprint 8, we should have a **complete domain model**, not necessarily a complete feature set.

The architectural progression should be:

**Entities → Relationships → DTOs → Mapping → Validation → Database**

Then Sprint 9 can focus on:

**Services → Business Logic → Endpoints → Authorization → Workflows → Testing**

This keeps us from repeating the Sprint 7 situation where we're building endpoints while parts of the domain model are still moving underneath them.