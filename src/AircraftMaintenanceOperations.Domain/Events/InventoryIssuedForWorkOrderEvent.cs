namespace AircraftMaintenanceOperations.Domain.Events;

public class InventoryIssuedForWorkOrderEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid IssuedByUserId { get; init; }
    public Guid InventoryPartId { get; init; }
    public int Quantity { get; init; }
    public string? Reason { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
