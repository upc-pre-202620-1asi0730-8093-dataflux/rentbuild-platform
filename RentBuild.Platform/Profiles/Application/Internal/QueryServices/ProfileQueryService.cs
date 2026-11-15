using RentBuild.Platform.Profiles.Application.QueryServices;
using RentBuild.Platform.Profiles.Domain.Repositories;
using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Profiles.Application.Internal.QueryServices;

public sealed class ProfileQueryService(ICompanyProfileRepository profiles) : IProfileQueryService
{
    public async Task<List<CompanyProfile>> List(int? userId) => (await profiles.ListAsNoTrackingAsync()).Where(p => userId is null || p.UserId == userId).ToList(); public async Task<CompanyProfile> Find(int id) => Rule.Found(await profiles.FindByIdAsNoTrackingAsync(id), "Profile");
}
