namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventoryById;

public record GetInventoryByIdQuery(Guid InventoryId) : IQuery<GetInventoryByIdQueryResult>;
public record GetInventoryByIdQueryResult(InventoryDto? Item);