using RentBuild.Platform.Iam.Domain.Model.Aggregates;
namespace RentBuild.Platform.Iam.Application.Internal.OutboundServices;

public interface IHashingService
{
    string Hash(string password); bool Verify(string password, string hash);
}
public interface ITokenService
{
    string Issue(User user);
}
public interface IProfileRegistration
{
    Task Create(int userId, string first, string last, string email, string company);
}
public interface IRecoveryMailSender
{
    bool IsConfigured
    {
        get;
    }
    Task Send(string email, string token);
}
