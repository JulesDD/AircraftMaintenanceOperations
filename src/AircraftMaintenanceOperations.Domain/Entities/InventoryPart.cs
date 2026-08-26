namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryPart : BaseEntity
{
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int QuantityOnHand { get; set; }
    public int MinimumQuantity { get; set; } 
    public string Location { get; set; } = string.Empty;

    public void ReserveStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity to reserve must be greater than zero.");
        }
        if (quantity > QuantityOnHand)
        {
            throw new InvalidOperationException("Not enough quantity on hand to reserve.");
        }
        QuantityOnHand -= quantity;
    }

    public void ReceivedStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException("Quantity received must be greater than zero.");
        }
        QuantityOnHand += quantity;
    }

    public bool NeedsRestock()
    {
        return QuantityOnHand <= MinimumQuantity;
    }

    public void Consume(int quantity)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity to consume must be greater than zero.");

        if (quantity > QuantityOnHand) throw new InvalidOperationException("Not enough quantity on hand to consume.");

        QuantityOnHand -= quantity;
    }
}
