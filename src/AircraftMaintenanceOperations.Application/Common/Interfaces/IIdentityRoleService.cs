namespace AircraftMaintenanceOperations.Application.Common.Interfaces;

public interface IIdentityRoleService
{
    Task UpdateUserRoleAsync(
        Guid userId,
        Role newRole,
        CancellationToken cancellationToken
    );
}
