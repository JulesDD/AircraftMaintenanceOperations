namespace AircraftMaintenanceOperations.Application.Features.MaintenanceRequests.Queries.GetMaintenanceQueryById;

public record GetMaintenanceQueryByIdHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUserService) : IQueryHandler<GetMaintenanceQueryById, GetMaintenanceQueryByIdResult>
{
    public async Task<GetMaintenanceQueryByIdResult> Handle(GetMaintenanceQueryById query, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUserService.GetDomainUserIdAsync(cancellationToken);

        var mQuery = await DbContext.MaintenanceRequests
            .Where(mq => mq.Id == query.MaintenanceRequestId)
            .FirstOrDefaultAsync(cancellationToken);

        var isPilot = CurrentUserService.Roles.Contains("Pilot");
        if (isPilot && mQuery is not null && mQuery.RequestedBy != domainUserId) throw new ForbiddenException("You are not authorized to access this maintenance request.");

        return new GetMaintenanceQueryByIdResult(mQuery == null ? null : new MaintenanceRequestDto(
            mQuery.RequestNumber,
            mQuery.Title,
            mQuery.Description,
            mQuery.AircraftId,
            mQuery.RequestedBy,
            mQuery.MaintenancePriority,
            mQuery.MaintenanceRequestStatus,
            mQuery.DueDate,
            mQuery.RequestedDate));
    }
}
