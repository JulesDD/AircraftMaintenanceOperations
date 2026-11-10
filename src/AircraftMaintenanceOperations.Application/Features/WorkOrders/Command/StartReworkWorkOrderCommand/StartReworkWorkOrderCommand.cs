namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ReworkWorkOrderCommand;

public record StartReworkWorkOrderCommand(Guid WorkOrderId, string ReworkNotes) : ICommand<StartReworkWorkOrderCommandResult>;
public record StartReworkWorkOrderCommandResult(bool IsReworkStarted, string Message);
