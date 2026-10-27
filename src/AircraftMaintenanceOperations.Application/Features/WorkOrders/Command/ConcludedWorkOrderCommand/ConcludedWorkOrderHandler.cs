namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ConcludedWorkOrderCommand;

public class ConcludedWorkOrderHandler(IAircraftMaintenanceDbContext DbContext, IEventPublisher EventPublisher, ICurrentUserService CurrentUser) : ICommandHandler<ConcludedWorkOrderCommand, ConcludedWorkOrderResult>
{
    public async Task<ConcludedWorkOrderResult> Handle(ConcludedWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var workOrder = await DbContext.WorkOrders.FindAsync(new object[] { command.WorkOrderId }, cancellationToken);
        if (workOrder is null) return new ConcludedWorkOrderResult(false, "Work order not found.");

        var user = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        var isAuthorized = workOrder.AssignedTechnicianId == user;
        if (!isAuthorized) return new ConcludedWorkOrderResult(false, "You are not authorized to conclude this work order.");

        var isConcluded = workOrder.Concluded(command.LaborNotes);
        if(!isConcluded.IsSuccess) return new ConcludedWorkOrderResult(false, isConcluded.ErrorMessage);

        await DbContext.SaveChangesAsync(cancellationToken);

        var concludedWorkOrderEvent = new WorkOrderConcludedEvent
        { 
            EventId = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            ConcludedByUserId = user,
            LaborNotes = workOrder.LaborNotes,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(concludedWorkOrderEvent, cancellationToken);
        return new ConcludedWorkOrderResult(true, "Work order concluded successfully. You will be notified when it is inspected and approved.");
    }
}