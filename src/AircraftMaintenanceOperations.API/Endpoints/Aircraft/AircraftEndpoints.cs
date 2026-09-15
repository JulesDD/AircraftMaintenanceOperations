namespace AircraftMaintenanceOperations.API.Endpoints.Aircraft;

public class AircraftEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/aircraft").WithTags("Aircraft").RequireAuthorization("Supervisor");

        group.MapPost("/", async(CreateAircraftCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return Results.Created($"/{result.AircraftId}", new CreateAircraftCommandResult(result.AircraftId));
        })
            .WithName("CreateAircraft")
            .Produces<CreateAircraftCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new aircraft.")
            .WithDescription("Creates a new aircraft.");

        group.MapGet("/", async(ISender sender) =>
        { 
            var query = new GetAircraftQuery();
            var result = await sender.Send(query);
            return Results.Ok(new GetAircraftResult(result.Aircrafts));
        })
            .WithName("GetAircraft")
            .Produces<GetAircraftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a list of aircraft.")
            .WithDescription("Retrieves all aircraft.");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetAircraftByIdQuery(id);
            var result = await sender.Send(query);
            var response = result.Adapt<GetAircraftByIdQueryResult>();
            return Results.Ok(response);
        })
            .WithName("GetAircraftById")
            .Produces<GetAircraftByIdQueryResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets aircraft by Id.")
            .WithDescription("Get aircraft by its Id.");

        group.MapPatch("/{id:guid}", async(Guid id, UpdateAircraftCommand command, ISender sender) =>
        {
            var update = command with { Id = id };
            return Results.Ok(await sender.Send(update));
        })
            .WithName("UpdateAircraft")
            .Produces<UpdateAircraftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updates Aircraft.")
            .WithDescription("Updates the details of an existing aircraft.");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var command = new ArchiveAircraftCommand(id);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ArchiveAircraft")
            .Produces<ArchiveAircraftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Archives Aircraft.")
            .WithDescription("Archives an existing aircraft.");

        group.MapPatch("/{id:guid}/assign-pilot", async (Guid id, AssignPilotCommand command, ISender sender) =>
        {
            var assign = command with { AircraftId = id };
            var result = await sender.Send(assign);
            return Results.Ok(result);
        })
            .WithName("AssignPilot")
            .Produces<AssignPilotResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Assign Pilot.")
            .WithDescription("Assigns a pilot to an aircraft.");
    }
}
