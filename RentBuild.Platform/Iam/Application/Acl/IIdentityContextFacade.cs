namespace RentBuild.Platform.Iam.Application.Acl;

public sealed record AccountSnapshot(int UserId, string Role, string Status);
public interface IIdentityContextFacade
{
    Task<AccountSnapshot?> Find(int userId);
}