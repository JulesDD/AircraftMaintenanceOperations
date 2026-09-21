namespace AircraftMaintenanceOperations.Application.Common.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken);
}
