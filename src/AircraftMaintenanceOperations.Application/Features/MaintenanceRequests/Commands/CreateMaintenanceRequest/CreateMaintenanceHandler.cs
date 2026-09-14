namespace AircraftMaintenanceOperations.Application.Features.MaintenanceRequests.Commands.CreateMaintenanceRequest;

public class CreateMaintenanceHandler(IAircraftMaintenanceDbContext DbContext, INumberGenerator NumberGenerator, ICurrentUserService CurrentUserService) : ICommandHandler<CreateMaintenanceCommand, CreatedMaintenanceResult>
{
    public async Task<CreatedMaintenanceResult> Handle(CreateMaintenanceCommand command, CancellationToken cancellationToken)
    {
        var requestNumber = await NumberGenerator.GenerateMaintenanceRequestNumberAsync();
        var domainUserId = await CurrentUserService.GetDomainUserIdAsync(cancellationToken);

        if (!await DbContext.Aircrafts.AnyAsync(x => x.Id == command.AircraftId, cancellationToken)) throw new InvalidOperationException("The specified aircraft does not exist.");
        
        var maintenanceRequest = MaintenanceRequest.Create(
            requestNumber,
            command.Title,
            command.AircraftId,
            command.Description,
            domainUserId,
            command.DueDate
        );

        DbContext.MaintenanceRequests.Add(maintenanceRequest);
        await DbContext.SaveChangesAsync(cancellationToken);

        return new CreatedMaintenanceResult(maintenanceRequest.Id); 
    }
}
