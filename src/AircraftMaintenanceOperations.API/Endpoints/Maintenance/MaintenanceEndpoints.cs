using AircraftMaintenanceOperations.Application.Features.MaintenanceRequests.Commands.StartMaintenanceRequest;
using AircraftMaintenanceOperations.Application.Features.MaintenanceTask.Queries.GetMaintenanceTaskQuery;

namespace AircraftMaintenanceOperations.API.Endpoints.Maintenance;

public record GetMaintenanceParameters(
    string? RequestNumber,
    string? RequestedBy,
    MaintenanceRequestStatus? Status,
    MaintenancePriority? Priority);

public record GetMaintenanceByIdCommand(Guid Id);
public class MaintenanceEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/maintenance").WithTags("Maintenance").RequireAuthorization("Supervisor");

        group.MapPost("/", async(CreateMaintenanceCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/maintenance/{result.Id}", result);
        })
            .WithName("CreateMaintenance")
            .Produces<CreatedMaintenanceResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new maintenance record.")
            .WithDescription("Create Maintenance");

        group.MapGet("/", async(ISender sender, [AsParameters] GetMaintenanceParameters command) =>
        {
            var result = await sender.Send(new GetMaintenanceRequestsQuery(
                command.RequestedBy,
                command.Status,
                command.Priority));
            return Results.Ok(result);
        })
            .WithName("GetMaintenance")
            .Produces<GetMaintenanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieves all maintenance records.")
            .WithDescription("Get Maintenance");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetMaintenanceQueryById(id));
            return Results.Ok(result);
        })
            .WithName("GetMaintenanceById")
            .Produces<GetMaintenanceQueryByIdResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieves a specific maintenance record.")
            .WithDescription("Get Maintenance by ID");

        group.MapPatch("/{id:guid}", async(Guid id, UpdateMaintenanceCommand command, ISender sender) =>
        {
            var updateCommand = new UpdateMaintenanceCommand(
                id,
                command.Title,
                command.Description,
                command.DueDate);
            return Results.Ok(await sender.Send(updateCommand));
        })
            .WithName("UpdateMaintenance")
            .Produces<UpdateMaintenanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Updates an existing maintenance record.")
            .WithDescription("Update Maintenance");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new ArchiveMaintenanceCommand(id));
            return Results.Ok(result);
        })
            .WithName("ArchiveMaintenance")
            .Produces<ArchiveMaintenanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Archives an existing maintenance record.")
            .WithDescription("Archive Maintenance");

        group.MapPost("/{id:guid}/start", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new StartMaintenanceCommand(id));
            return Results.Ok(result);
        })
            .WithName("StartMaintenance")
            .Produces<StartMaintenanceResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Starts an existing maintenance record.")
            .WithDescription("Start Maintenance");

        group.MapGet("/tasks", async (ISender sender, [AsParameters] GetMaintenanceTaskQuery query) =>
        {
            var result = await sender.Send(query);

            return Results.Ok(result);
        })
            .WithName("GetMaintenanceTasks")
            .Produces<GetMaintenanceTaskQueryResult>(StatusCodes.Status200OK)
            .WithSummary("Retrieves the maintenance task queue.")
            .WithDescription("Retrieves active maintenance requests and work orders.");
    }
}