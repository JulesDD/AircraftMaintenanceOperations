namespace AircraftMaintenanceOperations.Application.Features.InventoryTransaction.Commands.CreateInventory;

public class CreateInventoryCommandHandler(IAircraftMaintenanceDbContext DbContext, ICurrentUserService CurrentUser) : ICommandHandler<CreateInventoryCommand, CreateInventoryCommandResult>
{
    public async Task<CreateInventoryCommandResult> Handle(CreateInventoryCommand command, CancellationToken cancellationToken)
    {
        var isAuthorized = CurrentUser.Roles.Any(role => string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase) || 
            string.Equals(role, "InventoryClerk", StringComparison.OrdinalIgnoreCase));

        if (!isAuthorized) throw new UnauthorizedAccessException("You are not authorized to create inventory items.");
        var partExists = await DbContext.InventoryParts.AnyAsync(i => i.PartNumber == command.PartNumber, cancellationToken);

        if (partExists) throw new InvalidOperationException($"Inventory part with part number {command.PartNumber} already exists.");

        var inventoryPart = Domain.Entities.InventoryPart.Create(
            command.PartNumber,
            command.Description,
            command.Type,
            command.MinimumQuantity);

        DbContext.InventoryParts.Add(inventoryPart);
        await DbContext.SaveChangesAsync(cancellationToken);
        return new CreateInventoryCommandResult(inventoryPart.Id);
    }
}
