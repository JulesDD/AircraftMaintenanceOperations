namespace AircraftMaintenanceOperations.API.Endpoints.Technician;

public class TechnicianEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/technicians").WithTags("Technicians").RequireAuthorization("Supervisor");

        group.MapPost("/", async (CreateTechnicianCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/{result.Id}", result);
        })
            .WithName("CreateTechnician")
            .Produces<CreateTechnicianCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Created a new Technician.")
            .WithDescription("Create Technician");

        group.MapGet("/", async (ISender sender) =>
        {
            var technicians = await sender.Send(new GetTechnicianQuery());
            return Results.Ok(technicians);
        })
            .WithName("GetTechnicians")
            .Produces<List<TechnicianDto>>(StatusCodes.Status200OK)
            .WithSummary("Retrieves all Technicians.")
            .WithDescription("Get all Technicians");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var technician = await sender.Send(new GetTechnicianByIdQuery(id));
            if (technician == null)
            {
                return Results.NotFound();
            }
            return Results.Ok(technician);
        })
            .WithName("GetTechnicianById")
            .Produces<TechnicianDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Retrieves a Technician by Id.")
            .WithDescription("Get Technician by Id");

        group.MapPatch("/{id:guid}", async (Guid id, UpdateTechnicianCommand command, ISender sender) =>
        {
            var update = command with { TechnicianId = id };

            return Results.Ok(await sender.Send(update));
        })
            .WithName("UpdateTechnician")
            .Produces<UpdateTechnicianCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Updates an existing Technician.")
            .WithDescription("Update Technician");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var command = new ArchiveTechnicianCommand(id);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ArchiveTechnician")
            .Produces<ArchiveTechnicianCommandResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Archives an existing Technician.")
            .WithDescription("Archive Technician");
    }
}
