namespace AircraftMaintenanceOperations.Application.Features.DTOs;

public record InventoryTransactionDto
(
    Guid InventoryPartId,
    Guid? WorkOrderId,
    Guid PerformedByUserId,
    string Reason,
    DateTime TransactionDate,
    InventoryTransactionType TransactionType,
    int QuantityBefore,
    int QuantityAfter,
    int Quantity
);
