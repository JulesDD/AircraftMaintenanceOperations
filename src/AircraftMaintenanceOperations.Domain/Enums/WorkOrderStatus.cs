namespace AircraftMaintenanceOperations.Domain.Enums;

public enum WorkOrderStatus
{
    Open,
    Assigned,
    InProgress,
    Concluded,
    OnHold,
    InspectionPending,
    InspectionPassed,
    InspectionFailed,
    Completed,
    Archived
}
