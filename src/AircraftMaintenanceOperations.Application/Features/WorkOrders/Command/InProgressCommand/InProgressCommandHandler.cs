namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InProgressCommand;

public class InProgressCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<InProgressCommand, InProgressCommandResult>
{
    public async Task<InProgressCommandResult> Handle(InProgressCommand command, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var isSupervisor = CurrentUser.Roles.Contains("Admin") || CurrentUser.Roles.Contains("MaintenanceSupervisor");
        
        var workOrder = await DbContext.WorkOrders.FindAsync(command.WorkOrderId);
        if(workOrder is null) return new InProgressCommandResult(false);
        if (!isSupervisor && workOrder.AssignedTechnicianId != domainUserId) throw new ForbiddenException("You are not authorized to start this work order.");

        var inProgress = workOrder.InProgress(command.LaborNotes);
        if (!inProgress.IsSuccess) return new InProgressCommandResult(false);

        await DbContext.SaveChangesAsync(cancellationToken);
        return new InProgressCommandResult(true);
    }
}
