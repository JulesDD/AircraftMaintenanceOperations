namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CreateWorkOrderCommand;

public class CreateWorkOrderHandler(IAircraftMaintenanceDbContext DbContext, INumberGenerator NumberGenerator, IEventPublisher EventPublisher, ICurrentUserService CurrentUser) : ICommandHandler<CreateWorkOrderCommand, CreatedWorkOrderResult>
{
    public async Task<CreatedWorkOrderResult> Handle(CreateWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var workOrderNumber = await NumberGenerator.GenerateWorkOrderNumberAsync();

        if (await DbContext.WorkOrders.AnyAsync(x => x.MaintenanceRequestId == command.MaintenanceRequestId, cancellationToken)) throw new InvalidOperationException($"Work order for maintenance request with ID {command.MaintenanceRequestId} already exists.");
        if (await DbContext.WorkOrders.AnyAsync(x => x.AircraftId == command.AircraftId && x.WorkOrderStatus != WorkOrderStatus.Completed && x.WorkOrderStatus != WorkOrderStatus.Archived, cancellationToken)) throw new InvalidOperationException($"Aircraft with ID {command.AircraftId} already has an open work order.");
        var request = await DbContext.MaintenanceRequests.FirstOrDefaultAsync(x => x.Id == command.MaintenanceRequestId, cancellationToken);
        if (request is null) throw new InvalidOperationException("Maintenance request not found.");
        if (request.MaintenanceRequestStatus != MaintenanceRequestStatus.InProgress) throw new InvalidOperationException("Work orders can only be created from InProgress requests.");
        var workOrder = WorkOrder.Create
        (
            workOrderNumber,
            command.MaintenanceRequestId,
            command.AircraftId,
            command.WorkOrderPriority,
            command.EstimatedCompletionDate,
            command.LaborNotes
        );

        DbContext.WorkOrders.Add(workOrder);
        await DbContext.SaveChangesAsync(cancellationToken);

        var createdByUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var workOrderCreatedEvent = new WorkOrderCreatedEvent
        {
            EventId = Guid.NewGuid(),
            MaintenanceRequestId = workOrder.MaintenanceRequestId,
            WorkOrderId = workOrder.Id,
            AircraftId = workOrder.AircraftId,
            CreatedByUserId = createdByUserId,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(workOrderCreatedEvent, cancellationToken);
        return new CreatedWorkOrderResult(workOrder.Id);
    }
}
