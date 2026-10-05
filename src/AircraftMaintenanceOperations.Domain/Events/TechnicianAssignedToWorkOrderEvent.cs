namespace AircraftMaintenanceOperations.Domain.Events;

public class TechnicianAssignedToWorkOrderEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid TechnicianId { get; init; }
    public Guid AssignedByUserId { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
