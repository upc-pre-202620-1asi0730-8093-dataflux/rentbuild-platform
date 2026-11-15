using RentBuild.Platform.Rentals.Application.Internal.OutboundServices.Acl;
using RentBuild.Platform.Maintenance.Application.Acl;
namespace RentBuild.Platform.Rentals.Infrastructure.Acl.Maintenance;

public sealed class RentalMaintenanceAclAdapter(IMaintenanceContextFacade maintenance) : IRentalMaintenance
{
    public Task<bool> BlockingIncident(int equipmentId) => maintenance.HasBlockingIncident(equipmentId); public Task ScheduleReturnWork(int equipmentId, int owner) => maintenance.ScheduleReturnWork(equipmentId, owner);
}
