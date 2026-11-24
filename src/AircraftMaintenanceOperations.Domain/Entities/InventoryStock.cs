namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryStock : BaseEntity
{
    public Guid InventoryPartId { get; private set; }
    public InventoryPart Part { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Guid InventoryLocationId { get; private set; }
    public InventoryLocation Location { get; private set; } = null!;
    public InventoryCondition Condition { get; private set; }

    private InventoryStock() { }
    public static InventoryStock Create(InventoryPart part, int quantity, InventoryLocation location, InventoryCondition condition)
    {
        if (part == null) throw new ArgumentNullException(nameof(part));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (location == null) throw new ArgumentNullException(nameof(location));
        if (!location.IsActive) throw new InvalidOperationException("Cannot create stock in an inactive location.");
        if (!Enum.IsDefined(condition)) throw new ArgumentOutOfRangeException(nameof(condition));
        return new InventoryStock
        {
            InventoryPartId = part.Id,
            Part = part,
            Quantity = quantity,
            InventoryLocationId = location.Id,
            Location = location,
            Condition = condition
        };
    }

    public void IncreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        Quantity += quantity;
    }

    public void DecreaseQuantity(int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (quantity > Quantity) throw new InvalidOperationException("Insufficient stock quantity.");
        Quantity -= quantity;
    }
}
