namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public class CreateInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<CreateInventoryCommand, CreateInventoryCommandResult>
{
    public async Task<CreateInventoryCommandResult> Handle(CreateInventoryCommand command, CancellationToken cancellationToken)
    {
        var inventory = await DbContext.InventoryParts.FirstOrDefaultAsync(i => i.PartNumber == command.PartNumber, cancellationToken);
        if (inventory is not null) throw new InvalidOperationException($"Inventory part with part number {command.PartNumber} already exists.");

        var inventoryTransaction = Domain.Entities.InventoryPart.Create(
            command.PartNumber,
            command.Description,
            command.Type,
            command.QuantityOnHand,
            command.MinimumQuantity,
            command.Location
        );

        DbContext.InventoryParts.Add(inventoryTransaction);
        await DbContext.SaveChangesAsync(cancellationToken);

        return new CreateInventoryCommandResult(inventoryTransaction.Id);
    }
}
