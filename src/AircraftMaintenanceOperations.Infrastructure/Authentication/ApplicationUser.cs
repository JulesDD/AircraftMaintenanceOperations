using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Principal;

namespace AircraftMaintenanceOperations.Infrastructure.Authentication;

public class ApplicationUser : IdentityUser<Guid>
{
    public Guid DomainUserId { get; set; }

    //It should know things such as:
    //Identity ID
    //Username
    //Password credentials
    //Email confirmation
    //Identity claims / roles
    //Authentication-related information
}
