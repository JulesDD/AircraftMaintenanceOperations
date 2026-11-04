namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ResumeWorkOrderCommand;

public record ResumeWorkOrderCommand(Guid WorkOrderId, string ResumeNotes) : ICommand<ResumeWorkOrderResult>;
public record ResumeWorkOrderResult(bool IsResumed, string Message);