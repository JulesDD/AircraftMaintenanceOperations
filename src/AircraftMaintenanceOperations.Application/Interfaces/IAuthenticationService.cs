namespace AircraftMaintenanceOperations.Application.Interfaces;

public interface IAuthenticationService
{
    Task<AuthenticationResult?> AuthenticateAsync(string userName, string password );
}
