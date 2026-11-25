namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public Guid InventoryPartId { get; private set; }
    public InventoryPart InventoryPart { get; private set; } = null!;
    public InventoryTransactionType TransactionType { get; private set; }
    public InventoryCondition Condition { get; private set; }
    public int Quantity { get; private set; }
    public int QuantityBefore { get; private set; }
    public int QuantityAfter { get; private set; }
    public Guid InventoryLocationId { get; private set; }
    public Guid? WorkOrderId { get; private set; }
    public WorkOrder? WorkOrder { get; private set; }
    public Guid PerformedByUserId { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public string? Reason { get; private set; }

    private InventoryTransaction() { }
    private InventoryTransaction(
        Guid inventoryPartId,
        InventoryTransactionType transactionType,
        InventoryCondition condition,
        int quantity,
        int quantityBefore,
        int quantityAfter,
        Guid locationId,
        Guid? workOrderId,
        Guid performedByUserId,
        DateTime transactionDate,
        string reason)
    {
        InventoryPartId = inventoryPartId;
        TransactionType = transactionType;
        Condition = condition;
        Quantity = quantity;
        QuantityBefore = quantityBefore;
        QuantityAfter = quantityAfter;
        InventoryLocationId = locationId;
        WorkOrderId = workOrderId;
        PerformedByUserId = performedByUserId;
        TransactionDate = transactionDate;
        Reason = reason;
    }

    public static InventoryTransaction Create(
        Guid inventoryPartId,
        InventoryTransactionType transactionType,
        InventoryCondition condition,
        int quantity,
        int quantityBefore,
        int quantityAfter,
        Guid locationId,
        Guid? workOrderId,
        Guid performedByUserId,
        DateTime transactionDate,
        string reason)
    {
        if (quantity <= 0) throw new ArgumentException("Transaction quantity must be greater than zero.");
        if (inventoryPartId == Guid.Empty) throw new ArgumentException("Inventory part ID cannot be empty.");
        if (locationId == Guid.Empty) throw new ArgumentException("Inventory location ID cannot be empty.");
        if (performedByUserId == Guid.Empty) throw new ArgumentException("User ID cannot be empty.");
        if (!Enum.IsDefined(transactionType)) throw new ArgumentException(nameof(transactionType));
        if (!Enum.IsDefined(condition)) throw new ArgumentException(nameof(condition));
        if (quantityBefore <= 0 || quantityAfter <= 0) throw new ArgumentException("Quantity values must be greater than zero.");
        if (quantityAfter < quantityBefore) throw new ArgumentException("The quantity after must be greater than the value before.");



        return new InventoryTransaction(
            inventoryPartId,
            transactionType,
            condition,
            quantity,
            quantityBefore,
            quantityAfter,
            locationId,
            workOrderId,
            performedByUserId,
            transactionDate,
            reason);
    }
}