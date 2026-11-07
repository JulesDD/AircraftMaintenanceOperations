namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.RecordInspectionCommand;

public record RecordInspectionCommand(Guid WorkOrderId, bool IsInspected, string InspectionNotes) : ICommand<RecordInspectionCommandResult>;
public record RecordInspectionCommandResult(bool IsRecorded, string Message);