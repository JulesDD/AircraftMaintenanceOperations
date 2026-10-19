using AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

var workOrderConsumer = new WorkOrderEventConsumer();
var inventoryConsumer = new InventoryEventConsumer();

Console.WriteLine("Both consumers are starting.");
Console.WriteLine("Press Ctrl+C to shut down gracefully.");

var workOrderTask = Task.Run(() => workOrderConsumer.Start(cts.Token));
var inventoryTask = Task.Run(() => inventoryConsumer.Start(cts.Token));

await Task.Delay(1000);

Console.WriteLine("About to publish test inventory event...");

try
{
    await InventoryIssuedEventTestPublisher.PublishTestEventAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Test publisher failed: {ex}");
}

await Task.WhenAll(workOrderTask, inventoryTask);

Console.WriteLine("All consumers have stopped.");
