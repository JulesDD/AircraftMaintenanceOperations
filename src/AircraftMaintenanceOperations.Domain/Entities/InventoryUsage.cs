namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryUsage : BaseEntity
{
    public Guid InventoryPartId { get; set; }
    public InventoryPart InventoryPart { get; set; } = null!;
    public Guid WorkOrderId { get; set; }
    public WorkOrder WorkOrder { get; set; } = null!;
    public int QuantityUsed { get; set; }
    public DateTime DateUsed { get; set; }
}