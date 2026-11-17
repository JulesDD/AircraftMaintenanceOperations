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

                    if (envelope is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize Kafka event envelope.");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(envelope.EventType))
                    {
                        Console.Error.WriteLine(
                            $"Legacy or invalid event format detected. " +
                            $"Topic: {result.Topic}, Offset: {result.Offset}");

                        continue;
                    }

                    ProcessInventoryEvent(envelope);

                    Console.WriteLine(
                        $"Processed inventory event. Topic: {result.Topic}, " +
                        $"Partition: {result.Partition}, Offset: {result.Offset}, " +
                        $"EventType: {envelope.EventType}");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"Invalid JSON in inventory event: {ex.Message}");
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

    private static void ProcessInventoryEvent(KafkaEventEnvelope envelope)
    {
        switch (envelope.EventType)
        {
            case nameof(InventoryReceivedEvent):
                {
                    var inventoryEvent = envelope.Payload.Deserialize<InventoryReceivedEvent>();

                    if (inventoryEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize InventoryReceivedEvent.");
                        return;
                    }
                    break;
                }

            case nameof(InventoryAdjustedEvent):
                {
                    var inventoryEvent = envelope.Payload.Deserialize<InventoryAdjustedEvent>();

                    if (inventoryEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize InventoryAdjustedEvent.");
                        return;
                    }
                    break;
                }

            case nameof(InventoryIssuedForWorkOrderEvent):
                {
                    var inventoryEvent = envelope.Payload.Deserialize<InventoryIssuedForWorkOrderEvent>();

                    if (inventoryEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize InventoryIssuedForWorkOrderEvent.");
                        return;
                    }
                    break;
                }

            default:
                Console.Error.WriteLine($"Unsupported inventory event type: {envelope.EventType}");
                break;
        }
    }
}