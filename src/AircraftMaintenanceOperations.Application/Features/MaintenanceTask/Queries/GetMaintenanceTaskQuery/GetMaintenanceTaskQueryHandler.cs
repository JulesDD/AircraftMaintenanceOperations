namespace AircraftMaintenanceOperations.Application.Features.MaintenanceTask.Queries.GetMaintenanceTaskQuery;

public record GetMaintenanceTaskQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<GetMaintenanceTaskQuery, GetMaintenanceTaskQueryResult>
{
    public async Task<GetMaintenanceTaskQueryResult> Handle(GetMaintenanceTaskQuery query, CancellationToken cancellationToken)
    {
        // Fetch from two different sources
        var mRequest = DbContext.MaintenanceRequests.Where(mr => !DbContext.WorkOrders.Any(wo => wo.MaintenanceRequestId == mr.Id
        &&(mr.MaintenanceRequestStatus == MaintenanceRequestStatus.Open
        || mr.MaintenanceRequestStatus == MaintenanceRequestStatus.InProgress
        || mr.MaintenanceRequestStatus == MaintenanceRequestStatus.AwaitingParts))).OrderBy(mr => mr.RequestNumber).AsNoTracking();

        var wOrder = DbContext.WorkOrders.Where(wo =>
        wo.WorkOrderStatus == WorkOrderStatus.Open
        || wo.WorkOrderStatus == WorkOrderStatus.Assigned
        || wo.WorkOrderStatus == WorkOrderStatus.InProgress
        || wo.WorkOrderStatus == WorkOrderStatus.Inspection).OrderBy(wo => wo.Id).AsNoTracking();

        //map sources
        var requestTasks = await mRequest.Select(r => new MaintenanceTaskDto(
            MaintenanceTaskType.MR,
            r.RequestNumber,
            r.Title,
            r.MaintenancePriority,
            r.MaintenanceRequestStatus.ToString(),
            r.DueDate,
            null,
            new AircraftSummaryDto(
                r.Aircraft.Id,
                r.Aircraft.TailNumber,
                r.Aircraft.Manufacturer,
                r.Aircraft.Year,
                r.Aircraft.SerialNumber,
                r.Aircraft.Model,
                r.Aircraft.FlightHours,
                r.Aircraft.LastMaintenanceDate,
                r.Aircraft.NextMaintenanceDate))).ToListAsync(cancellationToken);

        var orderTasks = await wOrder.Select(o => new MaintenanceTaskDto(
            MaintenanceTaskType.WO,
            o.WorkOrderNumber,
            o.Title,
            o.WorkOrderPriority,
            o.WorkOrderStatus.ToString(),
            o.EstimatedCompletionDate,    
            o.AssignedTechnicianId,
            new AircraftSummaryDto(
                o.Aircraft.Id,
                o.Aircraft.TailNumber,
                o.Aircraft.Manufacturer,
                o.Aircraft.Year,
                o.Aircraft.SerialNumber,
                o.Aircraft.Model,
                o.Aircraft.FlightHours,
                o.Aircraft.LastMaintenanceDate,
                o.Aircraft.NextMaintenanceDate))).ToListAsync(cancellationToken); 

        //combine the sources into a collection of DTOs
        var maintenanceTasks = requestTasks.Concat(orderTasks);
        //filter the collection of DTOs
        if (query.Type.HasValue) maintenanceTasks = maintenanceTasks.Where(mt => mt.Tag == query.Type.Value);
        if (query.TechnicianId.HasValue) maintenanceTasks = maintenanceTasks.Where(mt => mt.TechnicianId == query.TechnicianId.Value);
        if (query.Priority.HasValue) maintenanceTasks = maintenanceTasks.Where(mt => mt.Priority == query.Priority.Value);
        if (!string.IsNullOrWhiteSpace(query.Status)) maintenanceTasks = maintenanceTasks.Where(mt =>mt.Status == query.Status);
        maintenanceTasks = maintenanceTasks.OrderByDescending(mt => mt.Priority).ThenBy(mt => mt.DueDate).ThenBy(mt => mt.ReferenceNumber);

        return new GetMaintenanceTaskQueryResult(maintenanceTasks);    
    }
}