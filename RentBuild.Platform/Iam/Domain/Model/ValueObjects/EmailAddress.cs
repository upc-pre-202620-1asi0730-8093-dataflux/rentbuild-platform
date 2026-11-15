using System.Net.Mail;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Domain.Model.ValueObjects;

public sealed record EmailAddress
{
    public string Value
    {
        get;
    }
    public EmailAddress(string value)
    {
        value = Rule.Text(value, "Email", 254).ToLowerInvariant();
        Rule.Require(MailAddress.TryCreate(value, out var parsed) && parsed.Address == value, "Invalid email");
        Value = value;
    }
}