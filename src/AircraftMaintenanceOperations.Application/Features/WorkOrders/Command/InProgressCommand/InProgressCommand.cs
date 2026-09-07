namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InProgressCommand;

public record InProgressCommand(Guid WorkOrderId, Guid TechnicianId, string LaborNotes) : ICommand<InProgressCommandResult>;
public record InProgressCommandResult(bool IsInProgress);
