namespace AircraftMaintenanceOperations.Infrastructure.Authentication;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, AircraftMaintenanceDbContext dbContext) : ICurrentUserService
{
    public Guid UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userId, out var id)) throw new UnauthorizedAccessException("UserID is not avalible");

            return id;
        }
    }

    public Guid DomainUserId
    {
        get
        {
            var applicationUser = dbContext.Set<ApplicationUser>().FirstOrDefault(u => u.Id == UserId);
            if (applicationUser is null) throw new UnauthorizedAccessException("Application user was not found");

            return applicationUser.DomainUserId;
        }
    }

    public IReadOnlyCollection<string> Roles => httpContextAccessor.HttpContext?.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray() ?? Array.Empty<string>();
}
