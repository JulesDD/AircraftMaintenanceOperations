namespace AircraftMaintenanceOperations.Domain.Entities;

public class User : BaseEntity
{
    public string? EmployeeNumber { get; protected set; } = string.Empty;
    public string? FirstName { get; protected set; } = string.Empty;
    public string? LastName { get; protected set; } = string.Empty;
    public string? Email { get; protected set; } = string.Empty;
    public string? PhoneNumber { get; protected set; } = string.Empty;
    public Role Role { get; protected set; }
    public EmploymentStatus Status { get; protected set; }

    public static User Create(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        Role role)
    {
        return new User
        {
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = role,
            Status = EmploymentStatus.Active
        };
    }
}