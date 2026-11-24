namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryPart : BaseEntity
{
    public string PartNumber { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public InventoryPartType Type { get; private set; }
    public int MinimumQuantity { get; private set; }

    private InventoryPart()
    {
    }

    public static InventoryPart Create(
        string partNumber,
        string description,
        InventoryPartType type,
        int minimumQuantity)
    {
        if (string.IsNullOrWhiteSpace(partNumber)) throw new ArgumentException("Part number is required.", nameof(partNumber));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (minimumQuantity < 0) throw new ArgumentOutOfRangeException(nameof(minimumQuantity));

        return new InventoryPart
        {
            PartNumber = partNumber.Trim(),
            Description = description.Trim(),
            Type = type,
            MinimumQuantity = minimumQuantity
        };
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        Description = description.Trim();
    }

    public void UpdateMinimumQuantity(int minimumQuantity)
    {
        if (minimumQuantity < 0) throw new ArgumentOutOfRangeException(nameof(minimumQuantity));
        MinimumQuantity = minimumQuantity;
    }
}
