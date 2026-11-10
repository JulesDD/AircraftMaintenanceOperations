namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.RecordInspectionCommand;

public class RecordInspectionCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher)
    : ICommandHandler<RecordInspectionCommand, RecordInspectionCommandResult>
{
    public async Task<RecordInspectionCommandResult> Handle(RecordInspectionCommand request, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        
        var workOrder = await DbContext.WorkOrders.FindAsync(request.WorkOrderId, cancellationToken);
        if (workOrder is null) return new RecordInspectionCommandResult(false, "Work order not found." );

        if (!CurrentUser.Roles.Contains("Supervisor")) throw new ForbiddenException("You are not authorized to record inspection for this work order.");

        var inspectionResult = workOrder.InspectWorkOrderResult(request.IsInspected, request.InspectionNotes);
        if (!inspectionResult.IsSuccess)
        {
            return new RecordInspectionCommandResult(false, inspectionResult.ErrorMessage);
        }
        await DbContext.SaveChangesAsync(cancellationToken);

        if (request.IsInspected)
        {
            var inspectionEvent = new WorkOrderInspectionPassedEvent
            {
                EventId = Guid.NewGuid(),
                WorkOrderId = workOrder.Id,
                InspectedByUserId = domainUserId,
                InspectionNotes = workOrder.LaborNotes!,
                OccurredAt = DateTimeOffset.UtcNow
            };
            await EventPublisher.PublishAsync(inspectionEvent, cancellationToken);
            return new RecordInspectionCommandResult(true, "Inspection recorded successfully.");
        } else
        {
            var failedEvent = new WorkOrderInspectionFailedEvent
            {
                EventId = Guid.NewGuid(),
                WorkOrderId = workOrder.Id,
                InspectedByUserId = domainUserId,
                InspectionNotes = workOrder.LaborNotes!,
                OccurredAt = DateTimeOffset.UtcNow
            };
            await EventPublisher.PublishAsync(failedEvent, cancellationToken);
            return new RecordInspectionCommandResult(true, "Inspection failed.");
        }
    }
}
