namespace AircraftMaintenanceOperations.Domain.Events;

public class WorkOrderOnHoldEvent
{
    public Guid EventId { get; init; }
    public Guid WorkOrderId { get; init; }
    public Guid OnHoldByUserId { get; init; }
    public string HoldNotes { get; init; }
    public HoldReason HoldReason { get; init; }
    public WorkOrderStatus PreviousStatus { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
}
