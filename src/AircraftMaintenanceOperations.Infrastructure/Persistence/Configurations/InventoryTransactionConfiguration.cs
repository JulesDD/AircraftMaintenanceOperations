namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class InventoryTransactionConfiguration
    : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> builder)
    {
        builder.HasKey(it => it.Id);

        builder.Property(it => it.TransactionType).IsRequired();
        builder.Property(it => it.Quantity).IsRequired();
        builder.Property(it => it.QuantityBefore).IsRequired();
        builder.Property(it => it.QuantityAfter).IsRequired();
        builder.Property(it => it.TransactionDate).IsRequired();
        builder.Property(it => it.Reason).HasMaxLength(500);

        builder.HasOne(it => it.InventoryPart)
            .WithMany()
            .HasForeignKey(it => it.InventoryPartId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(it => it.WorkOrder)
            .WithMany()
            .HasForeignKey(it => it.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(it => it.InventoryPartId);
        builder.HasIndex(it => it.WorkOrderId);
        builder.HasIndex(it => it.PerformedByUserId);
        builder.HasIndex(it => it.TransactionDate);
    }
}