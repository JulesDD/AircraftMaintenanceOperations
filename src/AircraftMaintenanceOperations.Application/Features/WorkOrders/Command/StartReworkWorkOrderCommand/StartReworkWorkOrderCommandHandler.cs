using AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectionCommand;

namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.ReworkWorkOrderCommand;

public class StartReworkWorkOrderCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher)
    : ICommandHandler<StartReworkWorkOrderCommand, StartReworkWorkOrderCommandResult>
{
    public async Task<StartReworkWorkOrderCommandResult> Handle(StartReworkWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        var workOrder = await DbContext.WorkOrders.FindAsync([command.WorkOrderId], cancellationToken); ;
        if (workOrder is null) return new StartReworkWorkOrderCommandResult(false, "Work order not found.");
        if (workOrder.AssignedTechnicianId != domainUserId) throw new ForbiddenException("You are not authorized to work on this work order.");

        var reworkResult = workOrder.StartRework(command.ReworkNotes);
        if (!reworkResult.IsSuccess) return new StartReworkWorkOrderCommandResult(false, reworkResult.ErrorMessage);

        await DbContext.SaveChangesAsync(cancellationToken);

        var reworkEvent = new WorkOrderReworkStartedEvent
        {
            EventId = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            ReworkedByUserId = domainUserId,
            ReworkNotes = command.ReworkNotes!,
            ReissueWorkToUserId = domainUserId,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(reworkEvent, cancellationToken);
        return new StartReworkWorkOrderCommandResult(true, "Rework started successfully.");
    }
}
