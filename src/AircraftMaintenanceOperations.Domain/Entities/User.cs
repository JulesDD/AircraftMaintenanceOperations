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

    public void ChangeRole(Role newRole)
    {
        if (Status == EmploymentStatus.Archived) throw new InvalidOperationException("An archived employee cannot change roles.");
        if (Status == EmploymentStatus.Retired) throw new InvalidOperationException("A retired employee cannot change roles.");
        if (Role == newRole) throw new InvalidOperationException($"Employee is already assigned the role '{newRole}'.");

        Role = newRole;
        ModifiedDate = DateTime.UtcNow;
    }

    public void StartBreak()
    {
        EnsureActive();

        Status = EmploymentStatus.OnBreak;
        ModifiedDate = DateTime.UtcNow;
    }

    public void EndBreak()
    {
        if (Status != EmploymentStatus.OnBreak) throw new InvalidOperationException("Employee is not currently on break.");

        Status = EmploymentStatus.Active;
        ModifiedDate = DateTime.UtcNow;
    }

    public void StartMedicalLeave()
    {
        EnsureActive();

        Status = EmploymentStatus.MedicalLeave;
        ModifiedDate = DateTime.UtcNow;
    }

    public void EndMedicalLeave()
    {
        if (Status != EmploymentStatus.MedicalLeave) throw new InvalidOperationException("Employee is not currently on medical leave.");

        Status = EmploymentStatus.Active;
        ModifiedDate = DateTime.UtcNow;
    }

    public void StartVacation()
    {
        EnsureActive();

        Status = EmploymentStatus.Vacation;
        ModifiedDate = DateTime.UtcNow;
    }

    public void EndVacation()
    {
        if (Status != EmploymentStatus.Vacation) throw new InvalidOperationException("Employee is not currently on vacation.");

        Status = EmploymentStatus.Active;
        ModifiedDate = DateTime.UtcNow;
    }

    public void StartTraining()
    {
        EnsureActive();

        Status = EmploymentStatus.Training;
        ModifiedDate = DateTime.UtcNow;
    }

    public void EndTraining()
    {
        if (Status != EmploymentStatus.Training) throw new InvalidOperationException("Employee is not currently in training.");

        Status = EmploymentStatus.Active;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Suspend()
    {
        if (Status == EmploymentStatus.Suspended) throw new InvalidOperationException("Employee is already suspended.");

        if (Status == EmploymentStatus.Retired || Status == EmploymentStatus.Archived) throw new InvalidOperationException("A retired or archived employee cannot be suspended.");

        Status = EmploymentStatus.Suspended;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Reinstate()
    {
        if (Status != EmploymentStatus.Suspended) throw new InvalidOperationException("Employee is not currently suspended.");

        Status = EmploymentStatus.Active;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Retire()
    {
        if (Status == EmploymentStatus.Retired) throw new InvalidOperationException("Employee is already retired.");
        if (Status == EmploymentStatus.Archived) throw new InvalidOperationException("An archived employee cannot be retired.");

        Status = EmploymentStatus.Retired;
        ModifiedDate = DateTime.UtcNow;
    }

    public void Archive()
    {
        if (Status == EmploymentStatus.Archived) throw new InvalidOperationException("Employee is already archived.");

        Status = EmploymentStatus.Archived;
        ModifiedDate = DateTime.UtcNow;
    }

    private void EnsureActive()
    {
        if (Status != EmploymentStatus.Active) throw new InvalidOperationException($"Employee must be Active to perform this operation. Current status: {Status}.");
    }
}