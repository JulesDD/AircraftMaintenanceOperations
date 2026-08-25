namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Configurations;

public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
{
    public void Configure(EntityTypeBuilder<Technician> builder)
    {
        builder.Property(x => x.CertificationLevel).HasConversion<int>();
        builder.Property(x => x.YearsOfExperience).HasColumnType("int");
    }
}
