namespace AircraftMaintenanceOperations.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Guid DomainUserId { get; }
    IReadOnlyCollection<string> Roles { get; }
}
