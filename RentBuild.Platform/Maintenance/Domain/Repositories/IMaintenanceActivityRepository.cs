using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Domain.Repositories;
namespace RentBuild.Platform.Maintenance.Domain.Repositories;

public interface IMaintenanceActivityRepository : IBaseRepository<MaintenanceActivity>
{
}
