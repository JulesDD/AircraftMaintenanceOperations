namespace AircraftMaintenanceOperations.Domain.Entities;

public class InventoryLocation : BaseEntity
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private InventoryLocation() { }

    public static InventoryLocation Create(
        string code,
        string name,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Location code is required.", nameof(code));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Location name is required.", nameof(name));

        return new InventoryLocation
        {
            Code = code.Trim(),
            Name = name.Trim(),
            Description = description?.Trim(),
            IsActive = true
        };
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Location name is required.", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}