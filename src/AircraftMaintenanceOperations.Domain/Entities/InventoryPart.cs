namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryPart : BaseEntity
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InventoryPartType Type { get; set; }
    public int QuantityOnHand { get; set; }
    public int MinimumQuantity { get; set; } 
    public string Location { get; set; } = string.Empty;
    
    public static InventoryPart Create(
        string partNumber,
        string description,
        InventoryPartType type,
        int quantityOnHand,
        int minimumQuantity,
        string location
        )
    {
        return new InventoryPart
        {
            PartNumber = partNumber,
            Description = description,
            Type = type,
            QuantityOnHand = quantityOnHand,
            MinimumQuantity = minimumQuantity,
            Location = location
        };
    }

    public DomainResult ReceivedStock(int quantity)
    {
        if (quantity <= 0) return new(false, "Quantity received must be greater than zero.");

        QuantityOnHand += quantity;
        return new(true);
    }

    public DomainResult AdjustStock(int quantity)
    {
        if (quantity == 0) return new(false, "Adjustment quantity cannot be zero.");
        if (QuantityOnHand + quantity < 0) return new(false, "Adjustment cannot reduce inventory below zero.");

        QuantityOnHand += quantity;
        return new(true);
    }

    public bool NeedsRestock()
    {
        return QuantityOnHand < MinimumQuantity;
    }

    public DomainResult Consume(int quantity)
    {
        if (quantity <= 0) return new(false, "Quantity to consume must be greater than zero.");
        if (quantity > QuantityOnHand) return new(false, "Not enough quantity on hand to consume.");

        QuantityOnHand -= quantity;
        return new(true);
    }
}
