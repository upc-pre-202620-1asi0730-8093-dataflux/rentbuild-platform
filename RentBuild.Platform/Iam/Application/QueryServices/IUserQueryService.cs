using RentBuild.Platform.Iam.Domain.Model.Aggregates;
namespace RentBuild.Platform.Iam.Application.QueryServices;

public interface IUserQueryService
{
    Task<User> Current();
}
