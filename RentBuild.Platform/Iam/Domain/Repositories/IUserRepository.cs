using RentBuild.Platform.Iam.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
namespace RentBuild.Platform.Iam.Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
}