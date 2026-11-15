using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public class BaseRepository<T>(AppDbContext db) : IBaseRepository<T> where T : AggregateRoot
{
    protected virtual IQueryable<T> Query(bool tracked) => tracked ? db.Set<T>() : db.Set<T>().AsNoTracking();
    public Task<T?> FindByIdAsync(int id) => Query(true).FirstOrDefaultAsync(x => x.Id == id);
    public Task<T?> FindByIdAsNoTrackingAsync(int id) => Query(false).FirstOrDefaultAsync(x => x.Id == id);
    public Task<List<T>> ListAsync() => Query(true).ToListAsync();
    public Task<List<T>> ListAsNoTrackingAsync() => Query(false).ToListAsync();
    public async Task AddAsync(T entity) => await db.Set<T>().AddAsync(entity);
    public void Remove(T entity) => db.Set<T>().Remove(entity);
}
