namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CompletedWorkOrder;

public record CompletedWorkOrderCommand
(
    Guid WorkOrderId,
    string LaborNotes,
    decimal LaborHours
) : IRequest<CompletedWorkOrderResult>;

public record CompletedWorkOrderResult(bool IsSuccess, string? ErrorMessage = null);
