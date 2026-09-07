namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InProgressCommand;

public class InProgressCommandHandler(IAircraftMaintenanceDbContext DbContext) : ICommandHandler<InProgressCommand, InProgressCommandResult>
{
    public async Task<InProgressCommandResult> Handle(InProgressCommand command, CancellationToken cancellationToken)
    {
        var workOrder = await DbContext.WorkOrders.FindAsync(command.WorkOrderId);
        if(workOrder is null) return new InProgressCommandResult(false);

        var inProgress = workOrder.InProgress(command.LaborNotes);
        if (!inProgress.IsSuccess) return new InProgressCommandResult(false);

        await DbContext.SaveChangesAsync(cancellationToken);
        return new InProgressCommandResult(true);
    }
}
