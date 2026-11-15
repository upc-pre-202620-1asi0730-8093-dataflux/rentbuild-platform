using RentBuild.Platform.Profiles.Application.Acl;
using RentBuild.Platform.Profiles.Domain.Repositories;
using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
namespace RentBuild.Platform.Profiles.Interfaces.Acl;

public sealed class ProfilesContextFacade(ICompanyProfileRepository profiles) : IProfilesContextFacade
{
    public Task Register(int userId, string first, string last, string email, string company) => profiles.AddAsync(new CompanyProfile(userId, first, last, email, company));
}
