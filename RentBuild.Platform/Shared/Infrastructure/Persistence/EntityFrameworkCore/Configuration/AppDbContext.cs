using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Infrastructure.Configuration;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public sealed class DomainEventLog
{
    public long Id { get; set; }
    public string AggregateType { get; set; } = "";
    public int AggregateId { get; set; }
    public string Name { get; set; } = "";
    public DateTimeOffset OccurredAt { get; set; }
}
public sealed class AppDbContext(DbContextOptions<AppDbContext> options, PlatformModuleCatalog modules) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<DomainEventLog>().ToTable("shared_domain_events");
        foreach (var module in modules.Modules) module.ConfigureModel(model);
        foreach (var entity in model.Model.GetEntityTypes().Where(t => typeof(AggregateRoot).IsAssignableFrom(t.ClrType)).ToArray())
            model.Entity(entity.ClrType).Ignore(nameof(AggregateRoot.DomainEvents));
        foreach (var property in model.Model.GetEntityTypes().SelectMany(type => type.GetProperties()))
        {
            if (property.ClrType == typeof(DateTimeOffset))
                property.SetValueConverter(new ValueConverter<DateTimeOffset, long>(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero)));
            if (property.ClrType == typeof(DateTimeOffset?))
                property.SetValueConverter(new ValueConverter<DateTimeOffset?, long?>(v => v.HasValue ? v.Value.UtcTicks : null, v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null));
        }
    }
}
