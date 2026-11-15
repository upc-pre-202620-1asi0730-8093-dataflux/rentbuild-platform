using RentBuild.Platform.Shared.Domain.Model.Aggregates;
namespace RentBuild.Platform.Shared.Domain.Repositories;

public interface IBaseRepository<T> where T : AggregateRoot
{
    Task<T?> FindByIdAsync(int id);
    Task<T?> FindByIdAsNoTrackingAsync(int id);
    Task<List<T>> ListAsync();
    Task<List<T>> ListAsNoTrackingAsync();
    Task AddAsync(T entity);
    void Remove(T entity);
}
public interface IUnitOfWork
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> action);
    Task FlushAsync();
}