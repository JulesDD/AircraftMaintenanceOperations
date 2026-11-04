namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderResumedEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid ResumedByUserId { get; init; }
    public string ResumeNotes { get; init; }
    public WorkOrderStatus RestoredStatus { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
