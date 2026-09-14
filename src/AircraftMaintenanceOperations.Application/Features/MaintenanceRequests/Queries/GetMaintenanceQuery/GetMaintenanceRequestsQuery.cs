namespace AircraftMaintenanceOperations.Application.Features.MaintenanceRequests.Queries.GetMaintenanceQuery;

public record GetMaintenanceRequestsQuery(
    Guid? RequestedBy,
    MaintenanceRequestStatus? Status,
    MaintenancePriority? Priority) : IQuery<GetMaintenanceResult>;
public record GetMaintenanceResult(IEnumerable<MaintenanceRequestDto> MaintenanceRequests);
