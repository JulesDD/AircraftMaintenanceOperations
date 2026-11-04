namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.OnHoldWorkOrderCommand;

public record OnHoldWorkOrderCommand(Guid WorkOrderId, HoldReason HoldReason, string HoldNotes) : ICommand<OnHoldWorkOrderResult>;
public record OnHoldWorkOrderResult(bool IsOnHold, string Message);
