namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CreateWorkOrderCommand;

public record CreateWorkOrderCommand(
    Guid MaintenanceRequestId,
    Guid AircraftId,
    MaintenancePriority WorkOrderPriority,
    DateTime EstimatedCompletionDate,
    string LaborNotes
    ) : ICommand<CreatedWorkOrderResult>;

public record CreatedWorkOrderResult(Guid Id);
