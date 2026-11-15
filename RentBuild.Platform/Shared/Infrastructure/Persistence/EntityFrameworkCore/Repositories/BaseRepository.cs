using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class BaseRepository<T>(AppDbContext db) : IBaseRepository<T> where T : AggregateRoot
{
    private IQueryable<T> Query(bool tracked)
    {
        IQueryable<T> query = tracked ? db.Set<T>() : db.Set<T>().AsNoTracking();
        if (typeof(T) == typeof(Equipment))
            query = query.Include("AvailabilityBlocks");
        return query;
    }
    public Task<T?> FindByIdAsync(int id) => Query(true).FirstOrDefaultAsync(x => x.Id == id);
    public Task<T?> FindByIdAsNoTrackingAsync(int id) => Query(false).FirstOrDefaultAsync(x => x.Id == id);
    public Task<List<T>> ListAsync() => Query(true).ToListAsync();
    public Task<List<T>> ListAsNoTrackingAsync() => Query(false).ToListAsync();
    public async Task AddAsync(T entity) => await db.Set<T>().AddAsync(entity);
    public void Remove(T entity) => db.Set<T>().Remove(entity);
}
