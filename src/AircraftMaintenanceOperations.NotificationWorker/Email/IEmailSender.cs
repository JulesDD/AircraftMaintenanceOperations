namespace AircraftMaintenanceOperations.NotificationWorker.Email;

public interface IEmailSender
{
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);    
}
