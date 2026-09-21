namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderCompletedEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid AircraftId { get; init; }
    public Guid CompletedByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}