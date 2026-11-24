
namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

internal class InventoryLocationConfiguration : IEntityTypeConfiguration<InventoryLocation>
{
    public void Configure(EntityTypeBuilder<InventoryLocation> builder)
    {
        builder.HasKey(il => il.Id);

        builder.Property(il => il.Code).IsRequired().HasMaxLength(20);
        builder.HasIndex(il => il.Code).IsUnique();
        builder.Property(il => il.Name).IsRequired().HasMaxLength(100);
        builder.Property(il => il.Description).HasMaxLength(200);
        builder.Property(il => il.IsActive).IsRequired();
    }
}
