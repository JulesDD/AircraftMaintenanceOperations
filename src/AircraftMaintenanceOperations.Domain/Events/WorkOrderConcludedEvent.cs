namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderConcludedEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public string LaborNotes { get; init; }
    public Guid ConcludedByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
