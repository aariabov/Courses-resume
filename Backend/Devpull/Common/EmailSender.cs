using System.Net;
using System.Net.Mail;
using Devpull.Common;

namespace Devpull.Controllers;

public interface IEmailSender
{
    Task SendEmailAsync(string email, string subject, string message);
}

public class FakeEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}

public class GoogleEmailSender : IEmailSender
{
    public GoogleEmailSender(AppConfig _) { }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        // От кого
        const string fromAddress = "ryabovandrey1989@gmail.com";
        const string appPassword = "zknisnunelafnlxc"; // Пароль приложения

        // Настройка SMTP
        using var smtp = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587, // Или 465 если используете SSL напрямую
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromAddress, appPassword)
        };

        // Письмо
        using var mailMessage = new MailMessage(fromAddress, email)
        {
            Subject = subject,
            Body = message
        };

        try
        {
            await smtp.SendMailAsync(mailMessage);
            Console.WriteLine("Письмо успешно отправлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при отправке: " + ex.Message);
        }
    }
}

public class EmailSender : IEmailSender
{
    private readonly AppConfig _config;

    public EmailSender(AppConfig config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        using var smtpClient = new SmtpClient
        {
            Host = _config.Smtp.Host!,
            Port = _config.Smtp.Port,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = true,
            EnableSsl = false,
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_config.Smtp.Username!, _config.Smtp.DisplayName),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);
        // TODO: добавить таймаут
        await smtpClient.SendMailAsync(mailMessage);
    }
}
