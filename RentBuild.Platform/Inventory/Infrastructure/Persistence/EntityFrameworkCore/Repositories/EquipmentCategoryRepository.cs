using RentBuild.Platform.Inventory.Domain.Model.Aggregates;
using RentBuild.Platform.Inventory.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Inventory.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class EquipmentCategoryRepository(AppDbContext db) : BaseRepository<EquipmentCategory>(db), IEquipmentCategoryRepository;
