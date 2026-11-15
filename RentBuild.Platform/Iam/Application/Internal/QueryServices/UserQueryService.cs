using RentBuild.Platform.Iam.Application.QueryServices;
using RentBuild.Platform.Iam.Domain.Model.Aggregates;
using RentBuild.Platform.Iam.Domain.Repositories;
using RentBuild.Platform.Shared.Application.OutboundServices;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Iam.Application.Internal.QueryServices;

public sealed class UserQueryService(IUserRepository users, ICurrentActor actor) : IUserQueryService
{
    public async Task<User> Current() => Rule.Found(await users.FindByIdAsNoTrackingAsync(actor.UserId), "Account");
}