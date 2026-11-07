namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Command.RecordInspectionCommand;

public class RecordInspectionCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser, IEventPublisher EventPublisher)
    : ICommandHandler<RecordInspectionCommand, RecordInspectionCommandResult>
{
    public Task<RecordInspectionCommandResult> Handle(RecordInspectionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
