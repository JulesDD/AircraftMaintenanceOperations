namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectionCommand;

public record InspectionCommand(Guid WorkOrderId, string InspectionNotes) : ICommand<InspectionCommandResult>;
public record InspectionCommandResult(bool IsInspected);
