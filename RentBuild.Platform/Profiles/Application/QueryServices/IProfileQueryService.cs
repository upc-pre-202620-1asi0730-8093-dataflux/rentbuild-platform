using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
namespace RentBuild.Platform.Profiles.Application.QueryServices;

public interface IProfileQueryService
{
    Task<List<CompanyProfile>> List(int? userId); Task<CompanyProfile> Find(int id);
}
