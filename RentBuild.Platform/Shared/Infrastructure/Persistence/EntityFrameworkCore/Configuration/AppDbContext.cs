using RentBuild.Platform.Iam.Domain.Model.Aggregates;
using RentBuild.Platform.Profiles.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Rentals.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentBuild.Platform.Shared.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Model.Entities;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public sealed class DomainEventLog
{
    public long Id
    {
        get; set;
    }
    public string AggregateType { get; set; } = ""; public int AggregateId
    {
        get; set;
    }
    public string Name { get; set; } = ""; public DateTimeOffset OccurredAt
    {
        get; set;
    }
}
public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<User>().ToTable("iam_users").HasIndex(x => x.Email).IsUnique();
        model.Entity<User>().Property(x => x.Email).HasMaxLength(254);
        model.Entity<PasswordResetToken>().ToTable("iam_password_reset_tokens").HasIndex(x => x.Hash).IsUnique();
        model.Entity<CompanyProfile>().ToTable("profiles_company_profiles").HasIndex(x => x.UserId).IsUnique();
        model.Entity<CompanyProfile>().OwnsOne(x => x.Address);
        model.Entity<Equipment>().ToTable("inventory_equipment").HasIndex(x => new { x.UserId, x.Code }).IsUnique();
        model.Entity<Equipment>().Property(x => x.Code).HasMaxLength(60);
        model.Entity<Equipment>().Property(x => x.Status).HasConversion<string>();
        model.Entity<Equipment>().OwnsOne(x => x.RentalRate, rate => { rate.Property(x => x.DailyRate).HasPrecision(18, 2); rate.Property(x => x.WeeklyRate).HasPrecision(18, 2); rate.Property(x => x.Currency).HasMaxLength(3); });
        model.Entity<Equipment>().HasMany(x => x.AvailabilityBlocks).WithOne().HasForeignKey(x => x.EquipmentId).OnDelete(DeleteBehavior.Cascade);
        model.Entity<Equipment>().Navigation(x => x.AvailabilityBlocks).HasField("_blocks").UsePropertyAccessMode(PropertyAccessMode.Field);
        model.Entity<Equipment>().Ignore(x => x.CanDelete);
        model.Entity<AvailabilityBlock>().ToTable("inventory_availability_blocks");
        model.Entity<EquipmentCategory>().ToTable("inventory_equipment_categories");
        model.Entity<SubscriptionPlan>().ToTable("subscriptions_plans").Property(x => x.PriceAmount).HasPrecision(18, 2);
        model.Entity<UserSubscription>().ToTable("subscriptions_user_subscriptions");
        model.Entity<RentalRequest>().ToTable("rentals_requests");
        model.Entity<RentalRequest>().OwnsOne(x => x.Period, p => { p.Property(x => x.StartDate); p.Property(x => x.EndDate); });
        model.Entity<RentalRequest>().Property(x => x.Status).HasConversion<string>();
        model.Entity<Rental>().ToTable("rentals_rentals").HasIndex(x => x.RentalRequestId).IsUnique();
        model.Entity<Rental>().OwnsOne(x => x.Period, p => { p.Property(x => x.StartDate); p.Property(x => x.EndDate); });
        model.Entity<Rental>().Property(x => x.Status).HasConversion<string>();
        model.Entity<Delivery>().ToTable("rentals_deliveries").HasIndex(x => x.RentalId).IsUnique();
        model.Entity<EquipmentReturn>().ToTable("rentals_returns").HasIndex(x => x.RentalId).IsUnique();
        model.Entity<MaintenanceRecord>().ToTable("maintenance_records");
        model.Entity<Incident>().ToTable("maintenance_incidents");
        model.Entity<MaintenanceActivity>().ToTable("maintenance_activities");
        model.Entity<DomainEventLog>().ToTable("shared_domain_events");
        foreach (var entity in model.Model.GetEntityTypes().Where(t => typeof(AggregateRoot).IsAssignableFrom(t.ClrType)))
        {
            model.Entity(entity.ClrType).Ignore(nameof(AggregateRoot.DomainEvents));
        }
        // One UTC representation works with both SQLite and MySQL providers.
        foreach (var property in model.Model.GetEntityTypes().SelectMany(t => t.GetProperties()))
        {
            if (property.ClrType == typeof(DateTimeOffset))
                property.SetValueConverter(new ValueConverter<DateTimeOffset, long>(v => v.UtcTicks, v => new DateTimeOffset(v, TimeSpan.Zero)));
            if (property.ClrType == typeof(DateTimeOffset?))
                property.SetValueConverter(new ValueConverter<DateTimeOffset?, long?>(v => v.HasValue ? v.Value.UtcTicks : null, v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : null));
        }
    }
}
