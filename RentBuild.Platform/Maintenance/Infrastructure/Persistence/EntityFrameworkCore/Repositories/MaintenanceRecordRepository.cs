using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Maintenance.Domain.Repositories;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using RentBuild.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
namespace RentBuild.Platform.Maintenance.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class MaintenanceRecordRepository(AppDbContext db) : BaseRepository<MaintenanceRecord>(db), IMaintenanceRecordRepository;
