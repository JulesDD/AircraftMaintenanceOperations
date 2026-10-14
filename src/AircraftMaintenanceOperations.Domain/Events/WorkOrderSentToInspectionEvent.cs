namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderSentToInspectionEvent
{
    public Guid EventId {get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid SentByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
