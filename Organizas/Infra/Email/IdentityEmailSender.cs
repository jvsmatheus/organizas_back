using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using Organizas.Entities;

namespace Organizas.Infra.Email;

public sealed class IdentityEmailSender : IEmailSender<User>
{
    private readonly EmailOptions _options;
    private readonly IConfiguration _config;

    public IdentityEmailSender(
        IOptions<EmailOptions> options,
        IConfiguration config
    ) {
        _options = options.Value;
        _config = config;
    }

    public async Task SendConfirmationLinkAsync(User user, string email, string token)
    {
        var baseUrl = _config.GetSection("ApplicationUrl");
        var confirmationLink = $"{baseUrl.Value}/confirm-email/{token}";

        await SendAsync(
            email,
            "Confirme seu e-mail no Organizas",
            $"""
                <!DOCTYPE html>
                <html lang="pt-BR">
                <body style="margin:0; padding:0; background-color:#f4f6f8; font-family:Arial,Helvetica,sans-serif; color:#263238;">
                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color:#f4f6f8; padding:32px 16px;">
                        <tr>
                            <td align="center">
                                <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width:560px; background-color:#ffffff; border-radius:12px;">
                                    <tr>
                                        <td style="padding:36px 32px; text-align:center;">
                                            <h1 style="margin:0 0 16px; color:#2563eb; font-size:26px;">Organizas</h1>

                                            <h2 style="margin:0 0 12px; color:#263238; font-size:20px;">
                                                Confirme seu e-mail
                                            </h2>

                                            <p style="margin:0 0 28px; color:#546e7a; font-size:15px; line-height:1.6;">
                                                Bem-vindo ao Organizas! Confirme seu endereço de e-mail para concluir seu cadastro.
                                            </p>

                                            <a href="{confirmationLink}"
                                               style="display:inline-block; padding:14px 24px; background-color:#2563eb; color:#ffffff; text-decoration:none; border-radius:6px; font-size:15px; font-weight:bold;">
                                                Confirmar meu e-mail
                                            </a>

                                            <p style="margin:28px 0 8px; color:#78909c; font-size:13px; line-height:1.5;">
                                                Se o botão não funcionar, copie e cole este link no navegador:
                                            </p>

                                            <p style="margin:0; font-size:13px; line-height:1.5; word-break:break-all;">
                                                <a href="{confirmationLink}" style="color:#2563eb;">{confirmationLink}</a>
                                            </p>
                                        </td>
                                    </tr>
                                </table>

                                <p style="margin:20px 0 0; color:#90a4ae; font-size:12px;">
                                    Se você não criou uma conta no Organizas, pode ignorar esta mensagem.
                                </p>
                            </td>
                        </tr>
                    </table>
                </body>
                </html>
            """
        );
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