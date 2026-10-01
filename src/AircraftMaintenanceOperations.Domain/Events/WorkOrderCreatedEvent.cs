namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderCreatedEvent
{
    public Guid EventId { get; init; }
    public Guid MaintenanceRequestId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid AircraftId { get; init; }
    public Guid CreatedByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}