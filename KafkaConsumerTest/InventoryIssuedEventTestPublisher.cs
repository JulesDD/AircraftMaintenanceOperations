using AircraftMaintenanceOperations.Domain.Events;
using AircraftMaintenanceOperations.Infrastructure.Kafka;

public static class InventoryIssuedEventTestPublisher
{
    public static async Task PublishTestEventAsync()
    {
        var testEvent = new InventoryIssuedForWorkOrderEvent
        {
            EventId = Guid.NewGuid(),
            WorkOrderId = Guid.NewGuid(),
            IssuedByUserId = Guid.NewGuid(),
            InventoryPartId = Guid.NewGuid(),
            Quantity = 2,
            Reason = "TEST ONLY - Kafka consumer deserialization test",
            OccurredAt = DateTimeOffset.UtcNow
        };

        var publisher = new KafkaEventPublisher();

        await publisher.PublishAsync(
            testEvent,
            CancellationToken.None);

        Console.WriteLine("Test inventory-issued event published.");
        Console.WriteLine($"Test Work Order ID: {testEvent.WorkOrderId}");
    }
}
