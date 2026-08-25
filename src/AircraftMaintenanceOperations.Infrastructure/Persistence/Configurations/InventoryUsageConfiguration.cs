namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class InventoryUsageConfiguration : IEntityTypeConfiguration<InventoryUsage>
{
    public void Configure(EntityTypeBuilder<InventoryUsage> builder)
    {
        builder.HasOne(iu => iu.InventoryPart).WithMany().HasForeignKey(iu => iu.InventoryPartId);
        builder.HasOne(iu => iu.WorkOrder).WithMany().HasForeignKey(iu => iu.WorkOrderId);

        builder.HasIndex(iu => iu.InventoryPartId);
        builder.HasIndex(iu => iu.WorkOrderId);
    }
}
