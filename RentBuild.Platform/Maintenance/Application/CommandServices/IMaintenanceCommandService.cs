using RentBuild.Platform.Maintenance.Domain.Model.Commands;
using RentBuild.Platform.Maintenance.Domain.Model.Aggregates;
using RentBuild.Platform.Shared.Application.Model;
namespace RentBuild.Platform.Maintenance.Application.CommandServices;

public interface IMaintenanceCommandService
{
    Task<Result<MaintenanceRecord>> Handle(RegisterMaintenanceCommand c); Task<Result<MaintenanceRecord>> Handle(AdvanceMaintenanceCommand c); Task<Result<Incident>> Handle(ReportIncidentCommand c); Task<Result<Incident>> Handle(PatchIncidentCommand c); Task<Result<bool>> Handle(ReactivateEquipmentCommand c);
}
