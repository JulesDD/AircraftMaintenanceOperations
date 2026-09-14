namespace AircraftMaintenanceOperations.Application.Features.DTOs;

public record MaintenanceRequestDto
(
    string RequestNumber,
    string Title,
    string Description,
    Guid AircraftId,
    Guid RequestedBy,
    MaintenancePriority MaintenancePriority,
    MaintenanceRequestStatus MaintenanceStatus,
    DateTime DueDate,
    DateTime RequestedDate
);