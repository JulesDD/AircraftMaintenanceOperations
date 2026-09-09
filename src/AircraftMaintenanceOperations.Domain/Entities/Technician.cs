namespace AircraftMaintenanceOperations.Domain.Entities;

public class Technician : User
{
    public CertificationLevel CertificationLevel { get; private set; }
    public int YearsOfExperience { get; private set; }

    public static Technician Create(
        string employeeNumber,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        CertificationLevel certificationLevel,
        int yearsOfExperience)
    {
        return new Technician
        {
            Id = Guid.NewGuid(),
            EmployeeNumber = employeeNumber,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            PhoneNumber = phoneNumber,
            Role = Role.Technician,
            CertificationLevel = certificationLevel,
            YearsOfExperience = yearsOfExperience,
            Status = EmploymentStatus.Active
        };
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        CertificationLevel certificationLevel,
        int yearsOfExperience)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        CertificationLevel = certificationLevel;
        YearsOfExperience = yearsOfExperience;
    }
}
