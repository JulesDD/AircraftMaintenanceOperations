namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class InventoryPartConfiguration : IEntityTypeConfiguration<InventoryPart>
{
    public void Configure(EntityTypeBuilder<InventoryPart> builder)
    {
        builder.HasKey(ip => ip.Id);

        builder.Property(ip => ip.PartNumber).IsRequired().HasMaxLength(50);
        builder.HasIndex(ip => ip.PartNumber).IsUnique();
        builder.Property(ip => ip.Description).IsRequired().HasMaxLength(200);
        builder.Property(ip => ip.Type).IsRequired();
        builder.Property(ip => ip.MinimumQuantity).IsRequired();
    }
}
