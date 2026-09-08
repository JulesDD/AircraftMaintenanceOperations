Sprint 8 Retrospective

Sprint Objective

Finish authorization verification and deliver the core Inventory
Management workflow, including transactions, adjustments, auditability,
authorization, testing, and sprint closeout.

Outcome

Sprint 8 development objectives were achieved. The remaining work is
final documentation/closeout rather than unfinished core functionality.

What We Completed

-Authorization

-Authentication tested.

-Authorized access tested.

-Unauthorized/forbidden access tested.

-Inventory Management authorization verified.

-Role-specific authorization paths tested.

-Inventory

-Create Inventory Part

-Receive Inventory

-Issue Inventory

-Adjust Inventory

-Get All Inventory

-Get Inventory By ID

-Get Inventory Transactions

-Low Stock / Needs Restock

-Transaction Auditability

Inventory transactions now capture the important audit context: Inventory Part -> Work Order when applicable -> Performing domain user ->
Transaction date -> Transaction type -> Quantity before -> Quantity after ->
Quantity -> Reason

Work Order

Technician assignment → Assigned

Assigned / WaitingForParts → InProgress

Labor notes captured

InProgress transition tested

What Went Well

Incremental implementation. Each operation was implemented and
tested before moving on.

Domain driven state changes. Inventory quantities are changed
through domain operations rather than directly in endpoints.

Auditability by default. Stock-changing operations create
transaction records as part of the same business operation.

Security was tested with the features. Authentication and
authorization were not left until the end.

Review caught a real transaction-history bug. The handler
initially lacked the InventoryPartId filter; it was corrected
before completion.

Adjustment semantics were clarified. Signed adjustment input
changes stock, while the transaction records the adjustment
magnitude.

Scope discipline. Reserve Inventory was deferred instead of
introducing an incomplete reservation model.

What Could Be Improved

Endpoint command reconstruction can be simplified later.

API response metadata and descriptions can receive a cleanup pass.

CurrentUserService has technical debt around synchronous data
access.

Global exception mapping can eventually distinguish
validation/business errors from generic 500 responses.

Inventory creation semantics could eventually enforce that stock
enters through Receive rather than initial creation quantity, if
that becomes a business rule.

Key Architectural Decisions

Reserve Inventory Deferred

Reservation is not being implemented without a real requirement. A
proper design would likely distinguish: QuantityOnHand, ReservedQuantity, AvailableQuantity and potentially introduce an InventoryReservation entity associated
with a Work Order.