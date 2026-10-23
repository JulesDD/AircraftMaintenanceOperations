namespace AircraftMaintenanceOperations.NotificationWorker.Messaging;

public class WorkOrderNotificationConsumer : BackgroundService
{
    private readonly ConsumerConfig _config;
    private readonly ILogger<WorkOrderNotificationConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEmailSender _emailSender;

    public WorkOrderNotificationConsumer(IConfiguration configuration, ILogger<WorkOrderNotificationConsumer> logger, IServiceScopeFactory scopeFactory, IEmailSender emailSender)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
        _emailSender = emailSender;
        _config = new ConsumerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            GroupId = configuration["Kafka:ConsumerGroupId"],
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler((_, error) =>
                _logger.LogError(
                    "Kafka error {Code}: {Reason}. Fatal: {IsFatal}",
                    error.Code,
                    error.Reason,
                    error.IsFatal))
            .Build();

        consumer.Subscribe("work-order-events");

        _logger.LogInformation("Work-order notification consumer started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = consumer.Consume(stoppingToken);
                if (result is null) continue;

                KafkaEventEnvelope? envelope;

                try
                {
                    envelope = JsonSerializer.Deserialize<KafkaEventEnvelope>(result.Message.Value);
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Invalid JSON message at {TopicPartitionOffset}. Skipping message.",
                        result.TopicPartitionOffset);

                    consumer.Commit(result);
                    continue;
                }

                if (envelope is null || string.IsNullOrWhiteSpace(envelope.EventType))
                {
                    _logger.LogWarning("Invalid Kafka envelope at {TopicPartitionOffset}. Skipping message.", result.TopicPartitionOffset);

                    consumer.Commit(result);
                    continue;
                }

                _logger.LogInformation("Received event {EventType} at {TopicPartitionOffset}", envelope.EventType, result.TopicPartitionOffset);
                await ProcessEventAsync(envelope, stoppingToken);
                consumer.Commit(result);
                _logger.LogInformation("Committed Kafka offset at {TopicPartitionOffset}", result.TopicPartitionOffset);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Work-order notification consumer stopping.");
        }
        finally
        {
            consumer.Close();
        }

        await Task.CompletedTask;
    }

    private async Task ProcessEventAsync(KafkaEventEnvelope envelope, CancellationToken cancellationToken)
    {
        switch (envelope.EventType)
        {
            case nameof(TechnicianAssignedToWorkOrderEvent): await ProcessTechnicianAssignedAsync(envelope, cancellationToken);
                break;

            case nameof(WorkOrderCompletedEvent): await ProcessWorkOrderCompletedAsync(envelope, cancellationToken);
                break;

            default:
                _logger.LogDebug(
                    "Skipping unsupported event type {EventType}.",
                    envelope.EventType);
                break;
        }      
    }

    private async Task ProcessTechnicianAssignedAsync(KafkaEventEnvelope envelope, CancellationToken cancellationToken)
    {
        var assignedEvent = envelope.Payload.Deserialize<TechnicianAssignedToWorkOrderEvent>();

        if (assignedEvent is null)
        {
            _logger.LogWarning("Could not deserialize technician assignment event.");

            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IAircraftMaintenanceDbContext>();
        var technician = await dbContext.Technicians.FindAsync([assignedEvent.TechnicianId], cancellationToken);
        if (technician is null)
        {
            _logger.LogWarning(
                "Technician {TechnicianId} was not found for work order {WorkOrderId}.",
                assignedEvent.TechnicianId,
                assignedEvent.WorkOrderId);

            return;
        }

        if (string.IsNullOrWhiteSpace(technician.Email))
        {
            _logger.LogWarning("Technician {TechnicianId} has no email address.", technician.Id);
            return;
        }

        var email = new EmailMessage(
            technician.Email,
            "You've been assigned a work order",
            $"""
        Hello {technician.FirstName},
        You have been assigned to work order {assignedEvent.WorkOrderId}.
        Please review the work order in the Airport Operations Platform.
        """);

        await _emailSender.SendAsync(email, cancellationToken);

        _logger.LogInformation("Sent assignment notification to technician {TechnicianId} for work order {WorkOrderId}.", technician.Id, assignedEvent.WorkOrderId);
    }


    private async Task ProcessWorkOrderCompletedAsync(KafkaEventEnvelope envelope, CancellationToken cancellationToken)
    {
        var completedEvent = envelope.Payload.Deserialize<WorkOrderCompletedEvent>();
        if (completedEvent is null)
        {
            _logger.LogWarning("Could not deserialize work order completion event.");
            return;
        }

        using var scope = _scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<IAircraftMaintenanceDbContext>();

        var workOrder = await dbContext.WorkOrders.FindAsync([completedEvent.WorkOrderId], cancellationToken);
        if (workOrder is null)
        {
            _logger.LogWarning("Work order {WorkOrderId} was not found for completion notification.", completedEvent.WorkOrderId);
            return;
        }

        if (workOrder.AssignedByUserId is not Guid assignedByUserId)
        {
            _logger.LogWarning("Work order {WorkOrderId} has no recorded assigning user.", workOrder.Id);
            return;
        }

        var technician = await dbContext.Technicians.FindAsync([workOrder.AssignedTechnicianId], cancellationToken);

        if (technician is null)
        {
            _logger.LogWarning("Assigned technician {TechnicianId} was not found for work order {WorkOrderId}.", workOrder.AssignedTechnicianId, workOrder.Id);
            return;
        }

        if (string.IsNullOrWhiteSpace(technician.Email))
        {
            _logger.LogWarning("Technician {TechnicianId} has no email address.", technician.Id);
            return;
        }

        var email = new EmailMessage(
            technician.Email,
            $"Work order {workOrder.Id} completed",
            $"""
            Hello {technician.FirstName},

            Work order {workOrder.Id} has been completed.
            Aircraft: {completedEvent.AircraftId}

            Thank you,
            Airport Operations Platform
            """);

        await _emailSender.SendAsync(email, cancellationToken);

        _logger.LogInformation("Sent completion notification to technician {TechnicianId} for work order {WorkOrderId}.", technician.Id, workOrder.Id);
    }

}