namespace AircraftMaintenanceOperations.NotificationWorker.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;

    public SmtpEmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var host = _configuration["Email:SmtpHost"];
        var portValue = _configuration["Email:SmtpPort"];
        var fromAddress = _configuration["Email:FromAddress"];

        if (string.IsNullOrWhiteSpace(host)) throw new InvalidOperationException("Email SMTP host is not configured.");
        if (!int.TryParse(portValue, out var port) || port <= 0) throw new InvalidOperationException("Email SMTP port is not configured correctly.");
        if (string.IsNullOrWhiteSpace(fromAddress)) throw new InvalidOperationException("Email sender address is not configured.");

        var email = new MimeMessage();

        email.From.Add(MailboxAddress.Parse(fromAddress));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;
        email.Body = new TextPart("plain")
        {
            Text = message.Body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(host, port, SecureSocketOptions.None, cancellationToken);
        await smtp.SendAsync(email, cancellationToken);
        await smtp.DisconnectAsync(true, cancellationToken);
    }
}

