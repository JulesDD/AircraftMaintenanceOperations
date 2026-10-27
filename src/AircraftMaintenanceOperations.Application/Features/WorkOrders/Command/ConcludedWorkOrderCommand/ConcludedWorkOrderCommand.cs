namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ConcludedWorkOrderCommand;

public record ConcludedWorkOrderCommand(Guid WorkOrderId, string LaborNotes) : ICommand<ConcludedWorkOrderResult>;
public record ConcludedWorkOrderResult(bool IsConcluded, string Message = "");