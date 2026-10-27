namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectWorkOrderCommand;

public record InspectWorkOrderCommand(Guid WorkOrderId, string LaborNotes) : ICommand<InspectWorkOrderResult>;
public record InspectWorkOrderResult(bool IsInspected);
