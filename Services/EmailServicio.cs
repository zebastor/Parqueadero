using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MailKit.Security;
using MimeKit;
using Parqueadero.Config;
using Parqueadero.Services.Interfaces;

namespace Parqueadero.Services;

public class EmailServicio : IEmailServicio
{
    private readonly EmailConfig _emailConfig;

    public EmailServicio(IOptions<EmailConfig> emailConfig)
    {
        _emailConfig = emailConfig.Value;
    }

    public async Task Enviar(string correo, string asunto, string cuerpo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailConfig.DisplayName, _emailConfig.Email));
        message.To.Add(MailboxAddress.Parse(correo));
        message.Subject = asunto;

        var cuerpobuilder = new BodyBuilder
        {
            HtmlBody = cuerpo
        };
        message.Body = cuerpobuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_emailConfig.Host, _emailConfig.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_emailConfig.Email, _emailConfig.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}