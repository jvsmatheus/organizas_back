using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Organizas.Entities;

namespace Organizas.Infra.Email;

public sealed class IdentityEmailSender : IEmailSender<User>
{
    private readonly EmailOptions _options;

    public IdentityEmailSender(IOptions<EmailOptions> options) {
        _options = options.Value;
    }

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
    {
        return SendAsync(
            email,
            "Confirme seu e-mail no Organizas",
            $"""
                <p>Bem-vindo ao Organizas!</p>
                <p>
                    <a href="{confirmationLink}">Confirmar meu e-mail</a>
                </p>
            """);
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
    {
        return SendAsync(
            email,
            "Redefina sua senha no Organizas",
            $"""
                <p>Recebemos uma solicitação para redefinir sua senha.</p>
                <p>
                    <a href="{resetLink}">Redefinir minha senha</a>
                </p>
                <p>Se você não fez essa solicitação, ignore esta mensagem.</p>
            """);
    }

    public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
    {
        return SendAsync(
            email,
            "Código para redefinir sua senha no Organizas",
            $"""
                <p>Use este código para redefinir sua senha:</p>
                <p>{resetCode}</p>
                <p>Se você não fez essa solicitação, ignore esta mensagem.</p>
            """);
    }

    private async Task SendAsync(string destination, string subject, string htmlBody)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));

        message.To.Add(MailboxAddress.Parse(destination));
        message.Subject = subject;

        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new MailKit.Net.Smtp.SmtpClient();

        await client.ConnectAsync(_options.Host, _options.Port, SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(_options.Username, _options.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}