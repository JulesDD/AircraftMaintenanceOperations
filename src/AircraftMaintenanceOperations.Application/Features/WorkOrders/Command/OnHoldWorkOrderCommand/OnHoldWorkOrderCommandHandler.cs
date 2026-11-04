namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.OnHoldWorkOrderCommand;

public class OnHoldWorkOrderCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher) 
    : ICommandHandler<OnHoldWorkOrderCommand,OnHoldWorkOrderResult>
{
    public async Task<OnHoldWorkOrderResult> Handle(OnHoldWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var workOrder = await DbContext.WorkOrders.FirstOrDefaultAsync(w => w.Id == command.WorkOrderId, cancellationToken);
        if (workOrder is null) return new OnHoldWorkOrderResult(false, "Work order not found.");

        var user = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        var isSupervisor = CurrentUser.Roles.Contains("MaintenanceSupervisor", StringComparer.OrdinalIgnoreCase);
        var isAssignedTechnician = workOrder.AssignedTechnicianId == user;
        if (!isSupervisor && !isAssignedTechnician) return new OnHoldWorkOrderResult(false, "You are not authorized to place this work order on hold.");

        var result = workOrder.OnHold(command.HoldReason, command.HoldNotes);
        if (!result.IsSuccess) return new OnHoldWorkOrderResult(false, result.ErrorMessage);



        await DbContext.SaveChangesAsync(cancellationToken);

        var workOrderOnHoldEvent = new WorkOrderOnHoldEvent
        {
            EventId = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            OnHoldByUserId = user,
            HoldNotes = command.HoldNotes,
            HoldReason = command.HoldReason!,
            PreviousStatus = workOrder.PreviousStatus!.Value,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(workOrderOnHoldEvent, cancellationToken);
        return new OnHoldWorkOrderResult(true, "Work order placed on hold successfully");
    }
}
