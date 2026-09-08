namespace AircraftMaintenanceOperations.Application.Features.DTOs;

public record InventoryDto
(
    string PartNumber,
    string Description,
    InventoryPartType InventoryPartType,
    int Quantity,
    string Location
);
