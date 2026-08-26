namespace AircraftMaintenanceOperations.API.Endpoints.ReceiveInventory;

public class ReceiveInventoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/inventory").WithTags("Inventory").RequireAuthorization("InventoryManagement");

        group.MapPost("/{inventoryPartId}/receive", async(Guid inventoryPartId, ReceiveInventoryCommand command, ISender sender) =>
        {
            var rInventory = new ReceiveInventoryCommand(inventoryPartId, command.Quantity, command.Reason);
            var result = await sender.Send(rInventory);
            return Results.Created($"/api/inventory/{inventoryPartId}/transactions/{result.Id}", result);
        })
            .WithName("ReceiveInventory")
            .Produces<ReceiveInventoryCommandResult>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Receieve Inventory.")
            .WithDescription("Receive stock for an inventory part and record the inventory transaction.");

        group.MapPost("/{inventoryPartId}/issue", async () => { 
        })
            .WithName("IssueInventory")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Issue Inventory")
            .WithDescription("Issue Inventory");

        group.MapPost("/{inventoryPartId}/adjust", async () => {
        })
            .WithName("AdjustInventory")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Adjust Inventory")
            .WithDescription("Adjust Inventory");

        group.MapGet("/", async () => {
        })
            .WithName("GetInventory")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Inventory.")
            .WithDescription("Retrieve inventory parts.");

        group.MapGet("/{inventoryPartId}", async () => {
        })
            .WithName("GetInventoryPart")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get inventory part.")
            .WithDescription("Retrieve a specific inventory part.");

        group.MapGet("/{inventoryPartId}/transactions", async () => {
        })
            .WithName("GetInventoryTransactions")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get inventory transactions.")
            .WithDescription("Retrieve the transaction history for an inventory part.");
    }
}
