namespace AircraftMaintenanceOperations.Application.Features.WorkOrders.Queries.GetWorkOrderQuery;

public record GetWorkOrderQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<GetWorkOrderQuery, GetWorkOrderQueryResult>
{
    public async Task<GetWorkOrderQueryResult> Handle(GetWorkOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await DbContext.WorkOrders
            .AsNoTracking()
            .OrderBy(wo => wo.WorkOrderNumber)
            .ToListAsync(cancellationToken);

        return new GetWorkOrderQueryResult(order.Select(wo => new WorkOrderDto
        (
            wo.Id,
            wo.WorkOrderNumber,
            wo.MaintenanceRequestId,
            wo.AircraftId,
            wo.AssignedTechnicianId,
            wo.WorkOrderPriority,
            wo.WorkOrderStatus,
            wo.EstimatedCompletionDate
        )));
    }
}
