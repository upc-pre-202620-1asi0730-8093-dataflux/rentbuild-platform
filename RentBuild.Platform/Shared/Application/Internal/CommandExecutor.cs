using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Domain.Model.Errors;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Shared.Application.Internal;

public sealed class CommandExecutor(IUnitOfWork work)
{
    public async Task<Result<T>> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return Result<T>.Success(await work.ExecuteAsync(operation));
        }
        catch (DomainRuleException error) { return Result<T>.Failure(error); }
    }
}
