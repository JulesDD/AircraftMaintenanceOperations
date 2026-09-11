namespace AircraftMaintenanceOperations.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    Task<Guid> GetDomainUserIdAsync(CancellationToken cancellationToken);
    IReadOnlyCollection<string> Roles { get; }
}
