namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventory;

public record GetInventoryQuery : IQuery<GetInventoryQueryResult>;
public record GetInventoryQueryResult(IEnumerable<InventoryDto> Items);