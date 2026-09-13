namespace AircraftMaintenanceOperations.API.Endpoints.Aircraft;

public record CreateAircraftResponse(Guid Id);
public record GetAircraftResult(IEnumerable<AircraftDto> Aircraft);
public record GetAircraftByIdResponse(AircraftDto Aircraft);
public record ArchiveAircraftResponse(Guid Id, bool IsArchived);
public record ArchiveAircraftRequest(Guid Id, bool IsArchived);



public class AircraftEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/aircraft").WithTags("Aircraft");

        group.MapPost("/", async(CreateAircraftCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);

            return Results.Created($"/{result.AircraftId}", new CreateAircraftResponse(result.AircraftId));
        })
            .WithName("CreateAircraft")
            .Produces<CreateAircraftResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new aircraft.")
            .WithDescription("Create Aircraft");

        group.MapGet("/", async(ISender sender) =>
        { 
            var query = new GetAircraftQuery();
            var result = await sender.Send(query);
            return Results.Ok(new GetAircraftResult(result.Aircrafts));
        })
            .WithName("GetAircraft")
            .Produces<GetAircraftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets a list of aircrafts.")
            .WithDescription("Get Aircrafts");

        group.MapGet("/{id:guid}", async (Guid id, ISender sender) =>
        {
            var query = new GetAircraftByIdQuery(id);
            var result = await sender.Send(query);
            var response = result.Adapt<GetAircraftByIdResponse>();
            return Results.Ok(response);
        })
            .WithName("GetAircraftById")
            .Produces<GetAircraftByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Gets aircraft by Id.")
            .WithDescription("Get aircraft by Id.");

        group.MapPatch("/{id:guid}", async(Guid id, UpdateAircraftCommand command, ISender sender) =>
        {
            var update = command with { Id = id };
            return Results.Ok(await sender.Send(update));
        })
            .WithName("UpdateAircraft")
            .Produces<UpdateAircraftResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Updated Aircraft.")
            .WithDescription("Updated Aircraft.");

        group.MapPatch("/{id:guid}/archive", async (Guid id, ISender sender) =>
        {
            var command = new ArchiveAircraftCommand(id);
            var result = await sender.Send(command);
            return Results.Ok(result);
        })
            .WithName("ArchiveAircraft")
            .Produces<ArchiveAircraftResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Archive Aircraft.")
            .WithDescription("Archive Aircraft");

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
            .WithDescription("Assign Pilot");
    }
}
