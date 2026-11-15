using Microsoft.Extensions.DependencyInjection.Extensions;
using RentBuild.Platform.Inventory.Application.Acl;
using RentBuild.Platform.Rentals.Application.Acl;
using RentBuild.Platform.Subscriptions.Application.Acl;
using RentBuild.Platform.Shared.Domain.Model.Errors;
namespace RentBuild.Platform.Shared.Infrastructure.Integration;
/// <summary>Explicit integration boundary until each teammate supplies the real facade.</summary>
public static class PendingContextFacades
{
    public static void AddPendingContextFacades(this IServiceCollection services)
    {
        services.TryAddScoped<IInventoryContextFacade, PendingInventory>();
        services.TryAddScoped<IRentalsContextFacade, PendingRentals>();
        services.TryAddScoped<ISubscriptionsContextFacade, PendingSubscriptions>();
    }
    internal static DomainRuleException Missing(string context) => new("CONTEXT_PENDING", context + " is pending integration from its feature branch", ErrorKind.Unavailable);
}
internal sealed class PendingInventory : IInventoryContextFacade
{
    public Task<EquipmentSnapshot> Find(int id) => throw PendingContextFacades.Missing("Inventory");
    public Task<bool> IsAvailable(int id, DateOnly start, DateOnly end) => throw PendingContextFacades.Missing("Inventory");
    public Task<int> Reserve(int id, DateOnly start, DateOnly end) => throw PendingContextFacades.Missing("Inventory");
    public Task MarkRented(int id) => throw PendingContextFacades.Missing("Inventory");
    public Task FinishRental(int id, int blockId, bool maintenance) => throw PendingContextFacades.Missing("Inventory");
    public Task Restrict(int id) => throw PendingContextFacades.Missing("Inventory");
    public Task Reactivate(int id) => throw PendingContextFacades.Missing("Inventory");
}
internal sealed class PendingRentals : IRentalsContextFacade
{
    public Task<bool> HasActiveRental(int id) => throw PendingContextFacades.Missing("Rentals");
    public Task<List<RentalHistoryItem>> History(int id) => throw PendingContextFacades.Missing("Rentals");
}
internal sealed class PendingSubscriptions : ISubscriptionsContextFacade
{
    public Task<bool> HasActive(int id, int minimumPlan = 1) => throw PendingContextFacades.Missing("Subscriptions");
}
