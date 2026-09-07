namespace AircraftMaintenanceOperations.Domain.Entities;

public class WorkOrder : BaseEntity
{
    public string WorkOrderNumber { get; private set; } = string.Empty;
    public Guid MaintenanceRequestId { get; private set; }
    public MaintenanceRequest? MaintenanceRequest { get; private set; } = null!;
    public string? Title { get; private set; }
    public string? Description { get; private set; }    
    public Guid AircraftId { get; private set; }
    public Aircraft Aircraft { get; private set; }
    public Guid AssignedTechnicianId { get; private set; }
    public MaintenancePriority WorkOrderPriority { get; private set; }
    public WorkOrderStatus WorkOrderStatus { get; private set; }
    public DateTime EstimatedCompletionDate { get; private set; }
    public DateTime? ActualCompletionDate { get; private set; }
    public decimal LaborHours { get; private set; }
    public string? LaborNotes { get; private set; }
    public ICollection<InventoryUsage> InventoryUsages { get; private set; }
    = new List<InventoryUsage>();

    public static WorkOrder Create(
        string workOrderNumber,
        Guid maintenanceRequestId,
        Guid aircraftId,
        MaintenancePriority workOrderPriority,
        DateTime estimatedCompletionDate,
        string laborNotes)
    {
        return new WorkOrder
        {
            WorkOrderNumber = workOrderNumber,
            MaintenanceRequestId = maintenanceRequestId,
            AircraftId = aircraftId,
            WorkOrderPriority = workOrderPriority,
            WorkOrderStatus = WorkOrderStatus.Open,
            EstimatedCompletionDate = estimatedCompletionDate,
            LaborNotes = laborNotes
        };
    }

    public void UpdateDetails(
       string title,
       string description,
       MaintenancePriority priority,
       DateTime estimatedCompletionDate)
    {
        if (WorkOrderStatus == WorkOrderStatus.Archived) throw new InvalidOperationException("Archived work orders cannot be updated.");

        Title = title;
        Description = description;
        WorkOrderPriority = priority;
        EstimatedCompletionDate = estimatedCompletionDate;
    }

    public void MediumPriority()
    {
        WorkOrderPriority = MaintenancePriority.Medium;
    }

    public void HighPriority()
    {
        WorkOrderPriority = MaintenancePriority.High;
    }

    public void CriticalPriority()
    {
        WorkOrderPriority = MaintenancePriority.Critical;
    }

    public DomainResult AssignTechnician(Technician technician, string laborNotes)
    {
        if (technician is null) return new(false, "Invalid technician.");
        if (WorkOrderStatus == WorkOrderStatus.Archived) return new(false, "Archived work orders cannot be assigned to a technician.");
        if (technician.Status != EmploymentStatus.Active) return new(false, "Only active technicians can be assigned to work orders.");
        if(string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");

        AssignedTechnicianId = technician.Id;
        WorkOrderStatus = WorkOrderStatus.Assigned;
        LaborNotes = laborNotes;
        return new(true);
    }

    public DomainResult InProgress(string laborNotes)
    {
        if (string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");
        if (WorkOrderStatus != WorkOrderStatus.Assigned && WorkOrderStatus != WorkOrderStatus.WaitingForParts) return new(false, "Only assigned or waiting-for-parts work orders can be moved to in progress.");
        WorkOrderStatus = WorkOrderStatus.InProgress;
        LaborNotes = laborNotes;
        return new(true);
    }

    public DomainResult WaitingForParts(string laborNotes)
    {
        if (string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");
        if (WorkOrderStatus != WorkOrderStatus.InProgress) return new(false, "Only work orders in progress can be moved to waiting for parts.");
        WorkOrderStatus = WorkOrderStatus.WaitingForParts;
        LaborNotes = laborNotes;
        return new(true);
    }

    public DomainResult Inspection(string laborNotes)
    {
        if (string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");
        if (WorkOrderStatus != WorkOrderStatus.InProgress) return new(false, "Only work orders in progress can be moved to inspection.");
        WorkOrderStatus = WorkOrderStatus.Inspection;
        LaborNotes = laborNotes;
        return new(true);
    }

    public DomainResult Completed(string laborNotes, decimal laborHours)
    {
        if (string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");
        if(laborHours <= 0) return new(false, "Labor hours must be greater than zero.");
        if (WorkOrderStatus != WorkOrderStatus.Inspection) return new(false, "Only work orders in inspection can be completed.");
        WorkOrderStatus = WorkOrderStatus.Completed;
        ActualCompletionDate = DateTime.UtcNow;
        LaborNotes = laborNotes;
        LaborHours = laborHours;

        return new(true);
    }

    public DomainResult ArchiveWorkOrder(string laborNotes)
    {
        if (string.IsNullOrWhiteSpace(laborNotes)) return new(false, "A status note is required.");
        if (WorkOrderStatus != WorkOrderStatus.Completed) return new(false, "Only completed work orders can be archived.");

        WorkOrderStatus = WorkOrderStatus.Archived;
        LaborNotes = laborNotes;
        return new(true);
    }
}