
namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class InventoryStockConfiguration : IEntityTypeConfiguration<InventoryStock>
{
    public void Configure(EntityTypeBuilder<InventoryStock> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.Condition).IsRequired();
        builder.HasOne(i => i.Part).WithMany().HasForeignKey(i => i.InventoryPartId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.Location).WithMany().HasForeignKey(i => i.InventoryLocationId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => new
        {
            i.InventoryPartId,
            i.InventoryLocationId,
            i.Condition
        }).IsUnique();
    }
}

