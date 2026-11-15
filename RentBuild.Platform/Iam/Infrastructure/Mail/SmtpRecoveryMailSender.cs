using System.Net;
using System.Net.Mail;
using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Infrastructure.Mail;

public sealed class SmtpRecoveryMailSender(IConfiguration configuration) : IRecoveryMailSender
{
    public bool IsConfigured => !string.IsNullOrWhiteSpace(configuration["Smtp:Host"]) && !string.IsNullOrWhiteSpace(configuration["Smtp:From"]);
    public async Task Send(string email, string token)
    {
        var url = (configuration["Smtp:ResetUrl"] ?? "http://127.0.0.1:5173/iam/reset-password") + "?token=" + Uri.EscapeDataString(token);
        using var smtp = new SmtpClient(configuration["Smtp:Host"], configuration.GetValue("Smtp:Port", 587)) { EnableSsl = configuration.GetValue("Smtp:EnableSsl", true) };
        if (!string.IsNullOrWhiteSpace(configuration["Smtp:Username"]))
            smtp.Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]);
        using var message = new MailMessage(configuration["Smtp:From"]!, email, "RentBuild - Password recovery", "Use this single-use link within 15 minutes: " + url);
        try
        {
            await smtp.SendMailAsync(message);
        }
        catch (SmtpException) { throw new DomainRuleException("MAIL_DELIVERY_FAILED", "Recovery mail could not be delivered", ErrorKind.Conflict); }
    }
}
