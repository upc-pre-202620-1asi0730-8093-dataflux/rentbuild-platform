using RentBuild.Platform.Iam.Application.Internal.OutboundServices;
using RentBuild.Platform.Profiles.Application.Acl;
namespace RentBuild.Platform.Iam.Infrastructure.Acl.Profiles;

public sealed class ProfileRegistrationAclAdapter(IProfilesContextFacade profiles) : IProfileRegistration
{
    public Task Create(int userId, string first, string last, string email, string company) => profiles.Register(userId, first, last, email, company);
}