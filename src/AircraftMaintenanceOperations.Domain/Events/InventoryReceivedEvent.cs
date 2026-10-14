namespace AircraftMaintenanceOperations.Domain.Events;

public class InventoryReceivedEvent
{
    public Guid EventId { get; init; }
    public Guid ReceivedByUserId { get; init; }
    public Guid InventoryPartId { get; init; }
    public int Quantity { get; init; }
    public string? Reason { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
