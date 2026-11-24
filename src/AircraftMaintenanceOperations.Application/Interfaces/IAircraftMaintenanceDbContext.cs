namespace AircraftMaintenanceOperations.Application.Interfaces;

public interface IAircraftMaintenanceDbContext
{
    DbSet<Aircraft> Aircrafts { get; }
    DbSet<Pilot> Pilots { get; }
    DbSet<Technician> Technicians { get; }
    DbSet<MaintenanceRequest> MaintenanceRequests { get; }
    DbSet<WorkOrder> WorkOrders { get; }
    DbSet<InventoryPart> InventoryParts { get; }
    DbSet<InventoryUsage> InventoryUsages { get; }
    DbSet<InventoryTransaction> InventoryTransactions { get; }
    DbSet<InventoryStock> InventoryStocks { get; }
    DbSet<InventoryLocation> InventoryLocations { get; }
    DbSet<User> Users { get; }
    DbSet<WorkOrderCounter> WorkOrderCounters { get; }
    DbSet<MaintenanceRequestCounter> MaintenanceRequestCounters { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
