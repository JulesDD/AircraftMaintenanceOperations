namespace AircraftMaintenanceOperations.Application.Features.DTOs;

public record UserDto
(
    string EmployeeNumber,
    string FirstName,
    string LastName,
    string Email,
    Role Role
);
