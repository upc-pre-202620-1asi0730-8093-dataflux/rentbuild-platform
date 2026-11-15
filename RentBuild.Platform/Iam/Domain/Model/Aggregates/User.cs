using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Iam.Domain.Model.ValueObjects;
namespace RentBuild.Platform.Iam.Domain.Model.Aggregates;

public sealed class User : AggregateRoot
{
    public string Email { get; private set; } = "";
    public string PasswordHash { get; private set; } = "";
    public string Role { get; private set; } = "";
    public string Status { get; private set; } = "active";
    public int TokenVersion
    {
        get; private set;
    }
    private User()
    {
    }
    public User(string email, string hash, string role)
    {
        Email = new EmailAddress(email).Value;
        Rule.Require(role is "rental_company" or "construction_company", "Invalid account role");
        Role = role;
        PasswordHash = Rule.Text(hash, "Password hash", 300);
        Raise("UserRegistered");
    }
    public void ChangePassword(string hash)
    {
        PasswordHash = Rule.Text(hash, "Password hash", 300);
        TokenVersion++;
        Raise("PasswordChanged");
    }
}
