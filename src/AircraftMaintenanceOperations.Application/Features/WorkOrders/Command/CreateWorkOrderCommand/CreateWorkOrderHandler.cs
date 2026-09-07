using AircraftMaintenanceOperations.Domain.Interfaces;

namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.CreateWorkOrderCommand;

public class CreateWorkOrderHandler(IAircraftMaintenanceDbContext dbContext, INumberGenerator numberGenerator) : ICommandHandler<CreateWorkOrderCommand, CreatedWorkOrderResult>
{
    public async Task<CreatedWorkOrderResult> Handle(CreateWorkOrderCommand command, CancellationToken cancellationToken)
    {
        var workOrderNumber = await numberGenerator.GenerateWorkOrderNumberAsync();

        if (await dbContext.WorkOrders.AnyAsync(x => x.MaintenanceRequestId == command.MaintenanceRequestId, cancellationToken)) throw new InvalidOperationException($"Work order for maintenance request with ID {command.MaintenanceRequestId} already exists.");
        if(await dbContext.WorkOrders.AnyAsync(x => x.AircraftId == command.AircraftId && x.WorkOrderStatus != WorkOrderStatus.Completed && x.WorkOrderStatus != WorkOrderStatus.Archived, cancellationToken)) throw new InvalidOperationException($"Aircraft with ID {command.AircraftId} already has an open work order.");
        var request = await dbContext.MaintenanceRequests.FirstOrDefaultAsync(x => x.Id == command.MaintenanceRequestId, cancellationToken);
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

        dbContext.WorkOrders.Add(workOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreatedWorkOrderResult(workOrder.Id);
    }
}
