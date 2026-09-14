namespace AircraftMaintenanceOperations.Domain.Entities;

public class MaintenanceRequest : BaseEntity
{
    public string RequestNumber { get; private set; }
    public Guid AircraftId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; set; } = string.Empty;
    public MaintenancePriority MaintenancePriority { get; set; }
    public MaintenanceRequestStatus MaintenanceRequestStatus { get; private set; }
    public Aircraft Aircraft { get; private set; }
    public Guid RequestedBy { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime ClosedDate { get; set; }

    public static MaintenanceRequest Create(
        string requestNumber,
        string title,
        Guid aircraftId,
        string description,
        Guid requestedBy,
        DateTime dueDate)
    {
        if (dueDate < DateTime.UtcNow) throw new InvalidOperationException("Due date cannot be in the past.");
        return new MaintenanceRequest
        {
            RequestNumber = requestNumber,
            Title = title,
            Description = description,
            AircraftId = aircraftId,
            MaintenancePriority = MaintenancePriority.Low,
            MaintenanceRequestStatus = MaintenanceRequestStatus.Open,
            RequestedBy = requestedBy,
            RequestedDate = DateTime.UtcNow,
            DueDate = dueDate
        };
    }

    public void Update(
        string title,
        string description,
        DateTime dueDate
        )
    {
        if (title is not null) Title = title;
        if (description is not null) Description = description;
        if (dueDate < DateTime.UtcNow) DueDate = DateTime.UtcNow;
    }

    public void MediumPriority()
    {
        MaintenancePriority = MaintenancePriority.Medium;
    }

    public void HighPriority()
    {
        MaintenancePriority = MaintenancePriority.High;
    }

    public void CriticalPriority()
    {
        MaintenancePriority = MaintenancePriority.Critical;
    }

    public void AwaitingParts()
    {
        MaintenanceRequestStatus = MaintenanceRequestStatus.AwaitingParts;
    }

    public void Complete()
    {
        MaintenanceRequestStatus = MaintenanceRequestStatus.Completed;
    }

    public void Start()
    {
        if (MaintenanceRequestStatus != MaintenanceRequestStatus.Open) throw new InvalidOperationException("Maintenance Request can only be started from Open.");
        MaintenanceRequestStatus = MaintenanceRequestStatus.InProgress;
    }

    public void Closed()
    {
        MaintenanceRequestStatus = MaintenanceRequestStatus.Closed;
        ClosedDate = DateTime.UtcNow;
    }

    public void Archive()
    {
        MaintenanceRequestStatus = MaintenanceRequestStatus.Archived;
    }
}
