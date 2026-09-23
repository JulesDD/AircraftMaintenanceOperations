namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class MaintenanceRequestConfiguration : IEntityTypeConfiguration<MaintenanceRequest>
{
    public void Configure(EntityTypeBuilder<MaintenanceRequest> builder)
    {
        builder.HasKey(mr => mr.Id);

        builder.HasOne<WorkOrder>()
            .WithOne(wo => wo.MaintenanceRequest)
            .HasForeignKey<WorkOrder>(wo => wo.MaintenanceRequestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(mr => mr.AircraftId);

        builder.Property(mr => mr.RequestedBy)
            .IsRequired();

        builder.HasIndex(mr => mr.RequestedBy);

        builder.Property(mr => mr.Description).IsRequired().HasMaxLength(1000);

        builder.Property(mr => mr.Title)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(mr => mr.MaintenancePriority)
            .HasConversion<string>();

        builder.Property(mr => mr.MaintenanceRequestStatus)
            .HasConversion<string>();

        builder.Property(mr => mr.DueDate)
            .IsRequired();

        builder.Property(mr => mr.CreatedDate)
            .IsRequired();

        builder.Property(mr => mr.RequestedDate)
            .IsRequired();

        builder.HasOne(mr => mr.Aircraft)
            .WithMany(a => a.MaintenanceRequests)
            .HasForeignKey(mr => mr.AircraftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(mr => mr.MaintenanceRequestStatus);

        builder.HasIndex(mr => mr.MaintenancePriority);
    }
}
