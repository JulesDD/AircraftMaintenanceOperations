namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventory;

public record GetInventoryQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<GetInventoryQuery, GetInventoryQueryResult>
{
    public async Task<GetInventoryQueryResult> Handle(GetInventoryQuery query, CancellationToken cancellationToken)
    {
        var inventoryItems = await DbContext.InventoryParts
            .OrderBy(i => i.PartNumber)
            .ToListAsync(cancellationToken);
        
        return new GetInventoryQueryResult(inventoryItems.Select(i => new InventoryDto(
            i.PartNumber,
            i.Description,
            i.Type,
            i.QuantityOnHand,
            i.Location
        )));
    }
}
