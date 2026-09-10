namespace AircraftMaintenanceOperations.Infrastructure.Authentication;

public class IdentityRoleService( UserManager<ApplicationUser> userManager) : IIdentityRoleService
{
    public async Task UpdateUserRoleAsync(Guid domainUserId, Role newRole, CancellationToken cancellationToken)
    {
        var applicationUser = await userManager.Users.FirstOrDefaultAsync(x => x.DomainUserId == domainUserId, cancellationToken);
        if (applicationUser is null) throw new InvalidOperationException("Application user was not found.");

        var currentRoles = await userManager.GetRolesAsync(applicationUser);
        if (currentRoles.Count > 0)
        {
            var removeResult = await userManager.RemoveFromRolesAsync(applicationUser, currentRoles);
            if (!removeResult.Succeeded) throw new InvalidOperationException("Failed to remove the user's current Identity role.");
        }

        var addResult = await userManager.AddToRoleAsync(applicationUser, newRole.ToString());
        if (!addResult.Succeeded) throw new InvalidOperationException("Failed to assign the user's new Identity role.");
    }
}
