namespace AircraftMaintenanceOperations.Application.Abstractions.Services;

public interface INumberGenerator
{
    Task<string> GenerateWorkOrderNumberAsync();
    Task<string> GenerateMaintenanceRequestNumberAsync();
}
