namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.InspectionCommand;

public class InspectionCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<InspectionCommand, InspectionCommandResult>
{
    public async Task<InspectionCommandResult> Handle(InspectionCommand command, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var isSupervisor = CurrentUser.Roles.Contains("Supervisor") || CurrentUser.Roles.Contains("Admin");
        
        
        var workOrder = await DbContext.WorkOrders.FindAsync(command.WorkOrderId);
        if(workOrder is null) return new InspectionCommandResult(false);
        if (!isSupervisor && workOrder.AssignedTechnicianId != domainUserId) throw new ForbiddenException("You are not authorized to work on this work order.");

        var inspection = workOrder.Inspection(command.LaborNotes);
        if(!inspection.IsSuccess) return new InspectionCommandResult(false);

        await DbContext.SaveChangesAsync(cancellationToken);
        return new InspectionCommandResult(true);
    }
}
