using AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.OnHoldWorkOrderCommand;

namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ResumeWorkOrderCommand;

public class ResumeWorkOrderCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher) 
    : ICommandHandler<ResumeWorkOrderCommand, ResumeWorkOrderResult>
{
    public async Task<ResumeWorkOrderResult> Handle(ResumeWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var workOrder = await DbContext.WorkOrders.FirstOrDefaultAsync(w => w.Id == command.WorkOrderId, cancellationToken);
        if (workOrder is null) return new ResumeWorkOrderResult(false, "Work order not found.");


        var user = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        var isSupervisor = CurrentUser.Roles.Contains("MaintenanceSupervisor", StringComparer.OrdinalIgnoreCase);
        var isAssignedTechnician = workOrder.AssignedTechnicianId == user;
        if (!isSupervisor && !isAssignedTechnician) return new ResumeWorkOrderResult(false, "You are not authorized to resume this work order.");

        var result = workOrder.ResumeFromHold(command.ResumeNotes);
        if (!result.IsSuccess) return new ResumeWorkOrderResult(false, result.ErrorMessage);
        
        var resumedByUserId = user;
        await DbContext.SaveChangesAsync(cancellationToken);
        
        var workOrderResumedEvent = new WorkOrderResumedEvent
        {
            EventId = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            ResumedByUserId = resumedByUserId,
            ResumeNotes = command.ResumeNotes!,
            RestoredStatus = workOrder.WorkOrderStatus,
            OccurredAt = DateTimeOffset.UtcNow
        };
        
        await EventPublisher.PublishAsync(workOrderResumedEvent, cancellationToken);
        
        return new ResumeWorkOrderResult(true, "Work order resumed successfully");
    }
}
