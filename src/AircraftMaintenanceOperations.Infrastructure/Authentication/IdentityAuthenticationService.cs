namespace AircraftMaintenanceOperations.Infrastructure.Authentication;

public class IdentityAuthenticationService(UserManager<ApplicationUser> userManager) : IAuthenticationService
{
    public async Task<AuthenticationResult?> AuthenticateAsync(string userName, string password)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user is null || await userManager.IsLockedOutAsync(user))
            return null;

        var passwordValid = await userManager.CheckPasswordAsync(user, password);
        if (!passwordValid)
        {
            await userManager.AccessFailedAsync(user);
            return null;
        }

        await userManager.ResetAccessFailedCountAsync(user);

        var roles = await userManager.GetRolesAsync(user);

        return new AuthenticationResult(
            user.Id,
            user.UserName!,
            roles);
    }
}
