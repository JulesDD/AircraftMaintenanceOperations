using AircraftMaintenanceOperations.Domain.Common;

namespace AircraftMaintenanceOperations.Infrastructure.Persistence.Context;

public class AircraftMaintenanceDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IAircraftMaintenanceDbContext
{
    public AircraftMaintenanceDbContext(DbContextOptions<AircraftMaintenanceDbContext> options) : base(options)
    {
    }

    public DbSet<Aircraft> Aircrafts => Set<Aircraft>();
    public DbSet<Pilot> Pilots => Set<Pilot>();
    public DbSet<Technician> Technicians => Set<Technician>();
    public DbSet<MaintenanceRequest> MaintenanceRequests => Set<MaintenanceRequest>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<InventoryPart> InventoryParts => Set<InventoryPart>();
    public DbSet<InventoryUsage> InventoryUsages => Set<InventoryUsage>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
    public DbSet<User> Users => Set<User>();
    public DbSet<WorkOrderCounter> WorkOrderCounters => Set<WorkOrderCounter>();
    public DbSet<MaintenanceRequestCounter> MaintenanceRequestCounters => Set<MaintenanceRequestCounter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AircraftMaintenanceDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine(">>> CUSTOM SaveChangesAsync CALLED <<<");

        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {

            if (entry.State == EntityState.Added) entry.Entity.SetCreatedDate(now);
            if (entry.State == EntityState.Modified) entry.Entity.SetModifiedDate(now);
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
