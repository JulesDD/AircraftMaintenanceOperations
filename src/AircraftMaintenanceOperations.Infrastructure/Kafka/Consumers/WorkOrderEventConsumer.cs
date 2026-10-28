namespace AircraftMaintenanceOperations.Infrastructure.Kafka.Consumers;

public class WorkOrderEventConsumer
{
    private readonly ConsumerConfig _config;

    public WorkOrderEventConsumer()
    {
        _config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "workOrder-group",
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

        consumer.Subscribe("work-order-events");

        Console.WriteLine("Work-order consumer started. Waiting for messages...");

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

                    ProcessWorkOrderEvent(envelope);

                    Console.WriteLine($"Topic: {result.Topic} | " + $"Partition: {result.Partition} | " + $"Offset: {result.Offset}");
                    Console.WriteLine($"Key: {result.Message.Key}");
                    Console.WriteLine($"Value: {result.Message.Value}");
                    Console.WriteLine("--------------------------------");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"Invalid JSON in work-order event: {ex.Message}");
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
            Console.WriteLine("Work-order consumer shut down cleanly.");
        }
    }


    private void ProcessWorkOrderEvent(KafkaEventEnvelope envelope)
    {
        switch (envelope.EventType)
        {
            case nameof(WorkOrderCreatedEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<WorkOrderCreatedEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize WorkOrderCreatedEvent.");
                        return;
                    }

                    Console.WriteLine("WorkOrderCreatedEvent processed.");
                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    break;
                }

            case nameof(TechnicianAssignedToWorkOrderEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<TechnicianAssignedToWorkOrderEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize TechnicianAssignedToWorkOrderEvent.");
                        return;
                    }

                    Console.WriteLine("TechnicianAssignedToWorkOrderEvent processed.");
                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    break;
                }

            case nameof(WorkOrderInProgressEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<WorkOrderInProgressEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize WorkOrderInProgressEvent.");
                        return;
                    }

                    Console.WriteLine("WorkOrderInProgressEvent processed.");
                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    break;
                }

            case nameof(WorkOrderConcludedEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<WorkOrderConcludedEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize WorkOrderConcludedEvent.");
                        return;
                    }

                    Console.WriteLine("WorkOrderConcludedEvent processed.");
                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    Console.WriteLine($"Concluded By: {workOrderEvent.ConcludedByUserId}");
                    Console.WriteLine($"Labor Notes: {workOrderEvent.LaborNotes}");
                    Console.WriteLine($"Occurred At: {workOrderEvent.OccurredAt}");
                    break;
                }

            case nameof(WorkOrderSentToInspectionEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<WorkOrderSentToInspectionEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize WorkOrderSentToInspectionEvent.");
                        return;
                    }

                    Console.WriteLine("WorkOrderSentToInspectionEvent processed.");
                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    break;
                }

            case nameof(WorkOrderCompletedEvent):
                {
                    var workOrderEvent = envelope.Payload.Deserialize<WorkOrderCompletedEvent>();

                    if (workOrderEvent is null)
                    {
                        Console.Error.WriteLine("Failed to deserialize WorkOrderCompletedEvent.");
                        return;
                    }

                    Console.WriteLine($"Work Order ID: {workOrderEvent.WorkOrderId}");
                    Console.WriteLine($"Aircraft ID: {workOrderEvent.AircraftId}");
                    Console.WriteLine($"Completed By: {workOrderEvent.CompletedByUserId}");
                    Console.WriteLine($"Occurred At: {workOrderEvent.OccurredAt}");
                    break;
                }

            default:
                Console.WriteLine($"Unsupported work-order event type: {envelope.EventType}");
                break;
        }
    }

}
