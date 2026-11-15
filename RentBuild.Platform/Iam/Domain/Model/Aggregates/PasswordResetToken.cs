using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Domain.Model.Aggregates;

public sealed class PasswordResetToken : AggregateRoot
{
    public int UserId
    {
        get; private set;
    }
    public string Hash { get; private set; } = ""; public DateTimeOffset ExpiresAt
    {
        get; private set;
    }
    public bool Used
    {
        get; private set;
    }
    private PasswordResetToken()
    {
    }
    public PasswordResetToken(int userId, string hash, DateTimeOffset expires)
    {
        UserId = userId;
        Hash = hash;
        ExpiresAt = expires;
    }
    public void Consume(DateTimeOffset now)
    {
        Rule.Require(!Used && now < ExpiresAt, "Invalid or expired recovery token");
        Used = true;
    }
}