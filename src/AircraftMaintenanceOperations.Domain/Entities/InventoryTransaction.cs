namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid InventoryPartId { get; private set; }
    public InventoryPart InventoryPart { get; private set; } = null!;
    
    public InventoryTransactionType TransactionType { get; private set; }
    
    public int Quantity { get; private set; }
    public int QuantityBefore { get; private set; }
    public int QuantityAfter { get; private set; }
    
    public Guid? WorkOrderId { get; private set; }
    public WorkOrder? WorkOrder { get; private set; }
    
    public Guid PerformedByUserId { get; private set; }
    
    public DateTime TransactionDate { get; private set; }
    
    public string? Reason { get; private set; }

    private InventoryTransaction() { }
    private InventoryTransaction(
        Guid inventoryPartId,
        InventoryTransactionType transactionType,
        int quantity,
        int quantityBefore,
        int quantityAfter,
        Guid? workOrderId,
        Guid performedByUserId,
        DateTime transactionDate,
        string? reason)
    {
        InventoryPartId = inventoryPartId;
        TransactionType = transactionType;
        Quantity = quantity;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        WorkOrderId = workOrderId;
        PerformedByUserId = performedByUserId;
        TransactionDate = transactionDate;
        Reason = reason;
    }

    public static InventoryTransaction Create(
        Guid inventoryPartId,
        InventoryTransactionType transactionType,
        int quantity,
        int quantityBefore,
        int quantityAfter,
        Guid performedByUserId,
        Guid? workOrderId = null,
        string? reason = null)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Transaction quantity must be greater than zero.");

        return new InventoryTransaction(
            inventoryPartId,
            transactionType,
            quantity,
            quantityBefore,
            quantityAfter,
            workOrderId,
            performedByUserId,
            DateTime.UtcNow,
            reason);
    }
}