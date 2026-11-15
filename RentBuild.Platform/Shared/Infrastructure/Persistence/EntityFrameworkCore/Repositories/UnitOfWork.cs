using System.Data;
using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class CommandGate
{
    public SemaphoreSlim Semaphore { get; } = new(1, 1);
}
public sealed class UnitOfWork(AppDbContext db, CommandGate gate, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    public Task FlushAsync() => db.SaveChangesAsync();
    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        await gate.Semaphore.WaitAsync();
        try
        {
            await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                var result = await action();
                await db.SaveChangesAsync();
                var aggregates = db.ChangeTracker.Entries<AggregateRoot>().Select(e => e.Entity).Where(e => e.DomainEvents.Count > 0).ToList();
                foreach (var root in aggregates)
                    foreach (var e in root.DomainEvents)
                        db.Add(new DomainEventLog { AggregateType = root.GetType().Name, AggregateId = root.Id, Name = e.Name, OccurredAt = e.OccurredAt });
                await db.SaveChangesAsync();
                await transaction.CommitAsync();
                foreach (var root in aggregates)
                {
                    foreach (var e in root.DomainEvents)
                        logger.LogInformation("Committed domain event {Name} for {Aggregate}/{Id}", e.Name, root.GetType().Name, root.Id);
                    root.ClearDomainEvents();
                }
                return result;
            }
            catch { await transaction.RollbackAsync(); db.ChangeTracker.Clear(); throw; }
        }
        finally { gate.Semaphore.Release(); }
    }
}
