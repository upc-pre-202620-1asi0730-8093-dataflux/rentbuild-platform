using RentBuild.Platform.Iam.Application.Acl;
using RentBuild.Platform.Iam.Domain.Repositories;
namespace RentBuild.Platform.Iam.Interfaces.Acl;

public sealed class IdentityContextFacade(IUserRepository users) : IIdentityContextFacade
{
    public async Task<AccountSnapshot?> Find(int userId)
    {
        var user = await users.FindByIdAsNoTrackingAsync(userId);
        return user is null ? null : new(user.Id, user.Role, user.Status);
    }
}
