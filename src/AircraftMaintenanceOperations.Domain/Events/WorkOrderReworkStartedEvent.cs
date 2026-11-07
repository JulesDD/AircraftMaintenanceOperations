namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderReworkStartedEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid ReworkedByUserId { get; init; }
    public string ReworkNotes { get; init; }
    public Guid ReissueWorkToUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
