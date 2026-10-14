namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderInProgressEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid StartedByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
