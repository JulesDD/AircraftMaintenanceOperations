namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.AssignTechnicianCommand;

public class AssignTechnicianCommandHandler(IAircraftMaintenanceDbContext DbContext, IEventPublisher EventPublisher, ICurrentUserService CurrentUser) : ICommandHandler<AssignTechnicianCommand, AssignTechnicianResult>
{
    public async Task<AssignTechnicianResult> Handle(AssignTechnicianCommand command, CancellationToken cancellationToken)
    {
        var technician = await DbContext.Technicians.FindAsync([command.TechnicianId], cancellationToken);
        var workOrder = await DbContext.WorkOrders.FindAsync([command.WorkOrderId], cancellationToken);
        if (technician is null || workOrder is null) return new AssignTechnicianResult(false, "Technician or work order not found.");

        var assignedByUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);
        var assignTech = workOrder.AssignTechnician(technician, command.LaborNotes, assignedByUserId);
        

        if (!assignTech.IsSuccess) return new AssignTechnicianResult(false, assignTech.ErrorMessage);
        await DbContext.SaveChangesAsync(cancellationToken);

        var technicianAssignedEvent = new TechnicianAssignedToWorkOrderEvent
        {
            EventId = Guid.NewGuid(),
            WorkOrderId = workOrder.Id,
            TechnicianId = technician.Id,
            AssignedByUserId = assignedByUserId,
            OccurredAt = DateTimeOffset.UtcNow
        };

        await EventPublisher.PublishAsync(technicianAssignedEvent, cancellationToken);
        return new AssignTechnicianResult(true);

    }
}
