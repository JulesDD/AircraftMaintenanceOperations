namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Queries.GetWorkOrderByIdQuery;

public record GetWorkOrderByIdQueryHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : IQueryHandler<GetWorkOrderQueryByIdQuery, GetWorkOrderByIdQueryResult>
{
    public async Task<GetWorkOrderByIdQueryResult> Handle(GetWorkOrderQueryByIdQuery query, CancellationToken cancellationToken)
    {
        var domainUserId = await CurrentUser.GetDomainUserIdAsync(cancellationToken);

        var order = await DbContext.WorkOrders
            .Where(wo => wo.Id == query.WorkOrderId)
            .Where(wo => CurrentUser.Roles.Contains("Admin") || CurrentUser.Roles.Contains("MaintenanceSupervisor") || wo.AssignedTechnicianId == domainUserId)
            .FirstOrDefaultAsync(cancellationToken);
        if (order is null) return new GetWorkOrderByIdQueryResult(null);

        var isSupervisor = CurrentUser.Roles.Contains("Admin") || CurrentUser.Roles.Contains("MaintenanceSupervisor");
        if (!isSupervisor && order.AssignedTechnicianId != domainUserId) throw new ForbiddenException("You are not authorized to access this work order!");

        return new GetWorkOrderByIdQueryResult(order == null ? null : new WorkOrderDto(
            order.Id,
            order.WorkOrderNumber,
            order.MaintenanceRequestId,
            order.AircraftId,
            order.AssignedTechnicianId,
            order.WorkOrderPriority,
            order.WorkOrderStatus,
            order.EstimatedCompletionDate));
    }
}
