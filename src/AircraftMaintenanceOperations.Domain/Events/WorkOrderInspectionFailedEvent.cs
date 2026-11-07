namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderInspectionFailedEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid InspectedByUserId { get; init; }
    public string InspectionNotes { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
