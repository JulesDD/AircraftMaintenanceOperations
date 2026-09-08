namespace AircraftMaintenanceOperations.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime? CreatedDate { get; protected set; }
    public DateTime? ModifiedDate { get; protected set; }
}
