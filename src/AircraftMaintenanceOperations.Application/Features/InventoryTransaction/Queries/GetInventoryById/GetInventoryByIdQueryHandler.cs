namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Queries.GetInventoryById;

public class GetInventoryByIdQueryHandler(IAircraftMaintenanceDbContext DbContext) : IQueryHandler<GetInventoryByIdQuery, GetInventoryByIdQueryResult>
{
    public async Task<GetInventoryByIdQueryResult> Handle(GetInventoryByIdQuery request, CancellationToken cancellationToken)
    {
        var inventoryItem = await DbContext.InventoryParts
            .AsNoTracking()
            .Where(i => i.Id == request.InventoryId)
            .OrderBy(i => i.PartNumber)
            .FirstOrDefaultAsync(cancellationToken);
        
        return new GetInventoryByIdQueryResult(inventoryItem == null ? null : new InventoryDto(
            inventoryItem.PartNumber,
            inventoryItem.Description,
            inventoryItem.Type,
            inventoryItem.QuantityOnHand,
            inventoryItem.MinimumQuantity,
            inventoryItem.Location
        ));
    }
}
