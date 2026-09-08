namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.LowStock;

public record LowStockQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<LowStockQuery, LowStockQueryResult>
{
    public async Task<LowStockQueryResult> Handle(LowStockQuery request, CancellationToken cancellationToken)
    {
        var lowStockItems = await DbContext.InventoryParts
            .AsNoTracking()
            .Where(i => i.QuantityOnHand <= i.MinimumQuantity)
            .OrderBy(i => i.PartNumber)
            .Select(i => new InventoryDto(
                i.PartNumber,
                i.Description,
                i.Type,
                i.QuantityOnHand,
                i.MinimumQuantity,
                i.Location))
            .ToListAsync(cancellationToken);

        return new LowStockQueryResult(lowStockItems);
    }
}
