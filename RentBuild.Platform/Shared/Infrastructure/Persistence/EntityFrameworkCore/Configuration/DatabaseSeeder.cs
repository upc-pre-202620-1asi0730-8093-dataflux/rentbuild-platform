using Microsoft.EntityFrameworkCore;
using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
namespace RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

public static class DatabaseSeeder
{
    public static async Task Seed(AppDbContext db)
    {
        if (!await db.Set<EquipmentCategory>().AnyAsync())
            db.AddRange(new EquipmentCategory(1, "Excavadoras"), new EquipmentCategory(2, "Cargadores"), new EquipmentCategory(3, "Grúas"), new EquipmentCategory(4, "Compactadores"));
        if (!await db.Set<SubscriptionPlan>().AnyAsync())
            db.AddRange(new SubscriptionPlan(1, "Essential", 79), new SubscriptionPlan(2, "Professional", 149), new SubscriptionPlan(3, "Growth", 249));
        await db.SaveChangesAsync();
    }
}
