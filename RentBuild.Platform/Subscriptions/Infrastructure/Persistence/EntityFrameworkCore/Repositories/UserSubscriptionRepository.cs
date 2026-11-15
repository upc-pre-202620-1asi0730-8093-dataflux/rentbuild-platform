using RentBuild.Platform.Subscriptions.Domain.Model.Aggregates;
using RentBuild.Platform.Subscriptions.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Subscriptions.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class UserSubscriptionRepository(AppDbContext db) : BaseRepository<UserSubscription>(db), IUserSubscriptionRepository;
