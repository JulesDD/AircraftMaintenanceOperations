namespace AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

public class InventoryEventConsumer
{
    private readonly ConsumerConfig _config;

    public InventoryEventConsumer()
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "inventory-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    public void Start(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, error) =>
            {
                Console.Error.WriteLine(
                    $"Kafka error: {error.Code} - {error.Reason} " +
                    $"(Fatal: {error.IsFatal})");
            })
            .Build();

        consumer.Subscribe("inventory-events");

        Console.WriteLine("Inventory consumer started. Waiting for messages...");

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(cancellationToken);

                    var envelope = JsonSerializer.Deserialize<KafkaEventEnvelope>(result.Message.Value);

                    if (envelope.EventType == nameof(InventoryReceivedEvent))
                    {
                        var inventoryEvent = envelope.Payload.Deserialize<InventoryReceivedEvent>();

                        if (inventoryEvent is null)
                        {
                            Console.Error.WriteLine("Failed to deserialize InventoryReceivedEvent.");
                            continue;
                        }

                        Console.WriteLine($"Inventory Part ID: {inventoryEvent.InventoryPartId}");
                        Console.WriteLine($"Received By: {inventoryEvent.ReceivedByUserId}");
                        Console.WriteLine($"Quantity: {inventoryEvent.Quantity}");
                        Console.WriteLine($"Reason: {inventoryEvent.Reason}");
                        Console.WriteLine($"Occurred At: {inventoryEvent.OccurredAt}");
                    }

                    if (envelope.EventType == nameof(InventoryAdjustedEvent))
                    {
                        var inventoryEvent = envelope.Payload.Deserialize<InventoryAdjustedEvent>();

                        if (inventoryEvent is null)
                        {
                            Console.Error.WriteLine("Failed to deserialize InventoryAdjustedEvent.");
                            continue;
                        }

                        Console.WriteLine($"Inventory Part ID: {inventoryEvent.InventoryPartId}");
                        Console.WriteLine($"Adjusted By: {inventoryEvent.AdjustedByUserId}");
                        Console.WriteLine($"Quantity Change: {inventoryEvent.Quantity}");
                        Console.WriteLine($"Reason: {inventoryEvent.Reason}");
                        Console.WriteLine($"Occurred At: {inventoryEvent.OccurredAt}");
                    }

                    if (envelope.EventType == nameof(InventoryIssuedForWorkOrderEvent))
                    {
                        var inventoryEvent =  envelope.Payload.Deserialize<InventoryIssuedForWorkOrderEvent>();

                        if (inventoryEvent is null)
                        {
                            Console.Error.WriteLine("Failed to deserialize InventoryIssuedForWorkOrderEvent.");
                            continue;
                        }

                        Console.WriteLine($"Work Order ID: {inventoryEvent.WorkOrderId}");
                        Console.WriteLine($"Inventory Part ID: {inventoryEvent.InventoryPartId}");
                        Console.WriteLine($"Issued By: {inventoryEvent.IssuedByUserId}");
                        Console.WriteLine($"Quantity Issued: {inventoryEvent.Quantity}");
                        Console.WriteLine($"Occurred At: {inventoryEvent.OccurredAt}");
                    }

                    Console.WriteLine($"Topic: {result.Topic} | " + $"Partition: {result.Partition} | " + $"Offset: {result.Offset}");
                    Console.WriteLine($"Key: {result.Message.Key}");
                    Console.WriteLine($"Value: {result.Message.Value}");
                    Console.WriteLine("--------------------------------");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException ex)
                {
                    Console.Error.WriteLine($"Failed to consume message: {ex.Error.Code} - {ex.Error.Reason}");

                    if (ex.Error.IsFatal)
                    {
                        throw;
                    }
                }
            }
        }
        finally
        {
            consumer.Close();
            Console.WriteLine("Inventory consumer shut down cleanly.");
        }
    }
}