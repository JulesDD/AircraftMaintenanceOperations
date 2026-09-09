namespace AircraftMaintenanceOperations.Domain.Entities;

public class Pilot : User   
{
    public string? Rank { get; set; } = string.Empty;
    public string? LicenseNumber { get; set; } = string.Empty;

    public static Pilot Create(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string rank,
        string licenseNumber)
    {
        return new Pilot
        {
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = Role.Pilot,
            Rank = rank,
            LicenseNumber = licenseNumber,
            Status = EmploymentStatus.Active
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string rank,
        string licenseNumber)
    {
        if (firstName is not null)
            FirstName = firstName;

        if (lastName is not null)
            LastName = lastName;

        if (email is not null)
            Email = email;

        if (phoneNumber is not null)
            PhoneNumber = phoneNumber;

        if (rank is not null)
            Rank = rank;

        if (licenseNumber is not null)
            LicenseNumber = licenseNumber;
    }
}
