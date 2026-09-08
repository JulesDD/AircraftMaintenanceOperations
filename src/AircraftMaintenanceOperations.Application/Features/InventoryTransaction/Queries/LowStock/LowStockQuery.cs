namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.LowStock;

public record LowStockQuery : IQuery<LowStockQueryResult>;
public record LowStockQueryResult(IEnumerable<InventoryDto> Items);