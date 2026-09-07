namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.AssignTechnicianCommand;

public class AssignTechnicianCommandHandler(IAircraftMaintenanceDbContext dbContext) : ICommandHandler<AssignTechnicianCommand, AssignTechnicianResult>
{
    public async Task<AssignTechnicianResult> Handle(AssignTechnicianCommand command, CancellationToken cancellationToken)
    {
        var technician = await dbContext.Technicians.FindAsync([command.TechnicianId], cancellationToken);
        var workOrder = await dbContext.WorkOrders.FindAsync([command.WorkOrderId], cancellationToken);

        if(technician is null || workOrder is null) return new AssignTechnicianResult(false);

        var assignTech = workOrder.AssignTechnician(technician, command.LaborNotes);

        if (!assignTech.IsSuccess) return new AssignTechnicianResult(false, assignTech.ErrorMessage);
        await dbContext.SaveChangesAsync(cancellationToken);
        return new AssignTechnicianResult(true);

    }
}
